using Microsoft.AspNetCore.Mvc;
using Zoor_Lebanon.Models;

namespace Zoor_Lebanon.Areas.EndUser.Controllers
{
    [Area("EndUser")]
    public class HomeController : Controller
    {
        private readonly zoor_lebanonContext _context;

        private readonly ILogger<HomeController> _logger;

        public HomeController(zoor_lebanonContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }
        public IActionResult Index()
        {
            ViewData["ShowCarousel"] = true;

            var packages = _context.Packages
            .Select(p => new
            {
                p.PackageId,
                p.PackageName,
                p.Description,
                p.UnitPrice,
                p.TotalSpots,
                p.AverageDuration,
                p.Points,
                p.StartDate,
                p.EndDate,
                p.AvailableSpots,
                Location = p.Location != null ? p.Location.State + " - " + p.Location.City : "",
                PackageType = p.PackageType != null ? p.PackageType.PackageType1 : ""
            })
            .ToList();

            ViewBag.Packages = packages;

            return View();
        }

        public IActionResult About()
        {
            ViewData["ShowCarousel"] = false;
            return View();
        }

        public IActionResult Services()
        {
            ViewData["ShowCarousel"] = false;
            return View();
        }

        public IActionResult Packages(decimal? minPrice, decimal? maxPrice, int? packageTypeId, int? locationId, DateTime? startDate, DateTime? endDate)
        {
            ViewData["ShowCarousel"] = false;

            // Base query
            var packages = _context.Packages.AsQueryable();

            // Apply filters
            if (minPrice.HasValue)
                packages = packages.Where(p => p.UnitPrice >= minPrice.Value);
            if (maxPrice.HasValue)
                packages = packages.Where(p => p.UnitPrice <= maxPrice.Value);
            if (packageTypeId.HasValue)
                packages = packages.Where(p => p.PackageTypeId == packageTypeId.Value);
            if (locationId.HasValue)
                packages = packages.Where(p => p.LocationId == locationId.Value);
            if (startDate.HasValue)
            {
                var start = startDate.Value;
                packages = packages.Where(p => p.StartDate.HasValue && p.StartDate.Value >= start);
            }

            if (endDate.HasValue)
            {
                var end = endDate.Value;
                packages = packages.Where(p => p.EndDate.HasValue && p.EndDate.Value <= end);
            }

            // Execute query
            var filteredPackages = packages
                .Select(p => new
                {
                    p.PackageId,
                    p.PackageName,
                    p.Description,
                    p.UnitPrice,
                    p.TotalSpots,
                    p.AverageDuration,
                    p.Points,
                    p.StartDate,
                    p.EndDate,
                    p.AvailableSpots,
                    Location = p.Location != null ? p.Location.State + " - " + p.Location.City : "",
                    PackageType = p.PackageType != null ? p.PackageType.PackageType1 : ""
                })
                .Distinct()
                .ToList();

            // Debugging logs
            _logger.LogInformation($"Filtered Packages Count: {filteredPackages.Count}");

            // Return appropriate partial view for AJAX request
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                if (filteredPackages.Any())
                {
                    return PartialView("_PackagesPartial", filteredPackages);
                }
                else
                {
                    return PartialView("_NoPackagesFound");
                }
            }

            // Fallback for non-AJAX requests
            ViewBag.Packages = filteredPackages;
            ViewBag.PackageTypes = _context.PackageTypes.ToList();
            ViewBag.Locations = _context.Locations.ToList();
            return View();
        }

        public IActionResult Recommended()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            _logger.LogInformation($"Logged-in UserId: {userId}");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "EndUser" });
            }

            // Fetch user preferences
            var userPreferenceNames = _context.UserPreferences
                .Where(up => up.UserId == userId)
                .Join(
                    _context.Preferences,
                    up => up.PreferenceId,
                    pref => pref.PreferenceId,
                    (up, pref) => pref.PreferenceDescription
                )
                .ToList();

            _logger.LogInformation($"User Preferences: {string.Join(", ", userPreferenceNames)}");

            if (!userPreferenceNames.Any())
            {
                _logger.LogInformation("No preferences found for the user.");
                ViewBag.RecommendedPackages = new List<dynamic>();
                return View("Index");
            }

            // Fetch package types matching the preferences
            var matchingPackageTypeIds = _context.PackageTypes
                .Where(pt => userPreferenceNames.Contains(pt.PackageType1))
                .Select(pt => pt.PackageTypeId)
                .ToList();

            _logger.LogInformation($"Matching PackageTypeIds: {string.Join(", ", matchingPackageTypeIds)}");

            if (!matchingPackageTypeIds.Any())
            {
                _logger.LogInformation("No matching package types found for user preferences.");
                ViewBag.RecommendedPackages = new List<dynamic>();
                return View("Index");
            }

            // Fetch packages with matching package type IDs
            var recommendedPackages = _context.Packages
                .Where(p => p.PackageTypeId.HasValue && matchingPackageTypeIds.Contains(p.PackageTypeId.Value))
                .Select(p => new
                {
                    p.PackageId,
                    p.PackageName,
                    p.Description,
                    p.UnitPrice,
                    p.TotalSpots,
                    p.AverageDuration,
                    p.Points,
                    Location = p.Location != null ? p.Location.State + " - " + p.Location.City : "",
                    PackageType = p.PackageType != null ? p.PackageType.PackageType1 : ""
                })
                .ToList();

            _logger.LogInformation($"Recommended Packages Count: {recommendedPackages.Count}");
            foreach (var pkg in recommendedPackages)
            {
                _logger.LogInformation($"Package: {pkg.PackageName}, Type: {pkg.PackageType}, Price: {pkg.UnitPrice}");
            }

            ViewBag.RecommendedPackages = new List<dynamic>
            {
                new { PackageId = 1, PackageName = "Test Package 1", Description = "Description 1", UnitPrice = 100, TotalSpots = 10, AverageDuration = 2, Points = 50, Location = "Test City", PackageType = "Adventure" },
                new { PackageId = 2, PackageName = "Test Package 2", Description = "Description 2", UnitPrice = 200, TotalSpots = 20, AverageDuration = 3, Points = 100, Location = "Test City 2", PackageType = "Relaxation" }
            };


            return View("Index");
        }


        public IActionResult Blog()
        {
            ViewData["ShowCarousel"] = false;
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["ShowCarousel"] = false;
            return View();
        }
    }
}
