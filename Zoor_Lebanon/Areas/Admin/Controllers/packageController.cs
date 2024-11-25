
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Zoor_Lebanon_Booking_Platform.Models.Database;
using System.Linq;
using System.Threading.Tasks;
using Zoor_Lebanon_Booking_Platform;

namespace Zoor_Lebanon_Booking_Platform.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PackageController : Controller
    {
        private readonly ZoorLebanonContext _context;

        // Constructor to inject the database context
        public PackageController(ZoorLebanonContext context)
        {
            _context = context;
        }
        /* public IActionResult Loyalty()
         {
             var loyaltyData = _context.Users
                 .Where(u => u.Bookings.Any()) // Include only users with bookings
                 .Select(u => new Models.viewmodels.LoyaltyViewModel
                 {
                     UserName = $"{u.Firstname} {u.Lastname}",
                     NumberOfBookings = u.Bookings.Count,
                     LoyaltyPoints = u.Bookings.Count * 10 // Example: 10 points per booking
                 })
                 .ToList();

             return View(loyaltyData);
         }*/
        // GET: Calendar
        /*   public IActionResult Calendar()
           {
               // Fetch packages from the database
               var packages = _context.Package.ToList(); // Assuming _context is your DbContext
               return View(packages); // Pass packages to the view
           }*/
        /*        public IActionResult Index3()
                {
                    var viewModel = new TourismReportViewModel
                    {
                        VisitorCount = 820, // Example value
                        VisitorGrowthRate = 12.5m,
                        TopPackages = _context.Package
                            .Take(5) // Limit the results to top 5 packages
                            .Select(p => new PackageSummary
                            {
                                PackageName = p.PackageName,
                                UnitPrice = p.UnitPrice ?? 0, // Use default value if null
                                BookingsCount = p.Bookings.Count() // Count the number of bookings
                            }).ToList(),
                        TotalSales = _context.Bookings.Sum(b => b.TotalPrice ?? 0), // Total sales sum
                        SalesGrowthRate = 33.1m,
                        VisitorChartData = new ChartData
                        {
                            Dates = new List<string> { "18th", "20th", "22nd", "24th", "26th", "28th" },
                            Values = new List<int> { 100, 150, 180, 170, 160, 140 }
                        },
                        SalesChartData = new ChartData
                        {
                            Dates = new List<string> { "June", "July", "August", "September", "October", "November", "December" },
                            Values = new List<int> { 1000, 1500, 2000, 2500, 3000, 3500, 4000 }
                        }
                    };

                    return View(viewModel);
                }
        */
        public IActionResult Index3()
        {
            return View();
        }

        // GET: Add New Package

        [HttpGet]
        public IActionResult AddPackage()
        {
            // Fetch Package Types from the database
            var packageTypes = _context.PackageType.ToList();

            // Debugging: Check if package types are fetched correctly
            Console.WriteLine("Fetched Package Types:");
            foreach (var type in packageTypes)
            {
                Console.WriteLine($"ID: {type.PackageTypeId}, Name: {type.PackageType1}");
            }

            // Pass Package Types to the ViewBag for the dropdown
            ViewBag.PackageTypes = new SelectList(packageTypes, "PackageTypeId", "PackageType1");

            return View();
        }


        // POST: Add New Package
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPackage(Package package)
        {
            // Ensure dropdown is always repopulated
            ViewBag.PackageTypes = new SelectList(_context.PackageType.ToList(), "PackageTypeId", "PackageType1");

            if (ModelState.IsValid)
            {
                try
                {
                    // Add package to the database
                    _context.Package.Add(package);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("Package saved successfully.");
                    return RedirectToAction("Widgets");
                }
                catch (Exception ex)
                {
                    // Log exception and display error
                    Console.WriteLine($"Error saving package: {ex.Message}");
                    ModelState.AddModelError("", "An error occurred while saving the package. Please try again.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ModelState detected.");
                // Log validation errors
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine($"ModelState Error: {error.ErrorMessage}");
                }
            }

            // Return to the view with the model and repopulated dropdown
            return View(package);
        }

        // Optional: View All Packages (Widgets View)
        public IActionResult Widgets()
        {
            // Fetch all packages from the database
            var packages = _context.Package.ToList();
            ViewBag.TotalPackages = packages.Count;

            return View(packages);
        }
    }
}
