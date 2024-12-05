using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;using Zoor_Lebanon.Models;using Zoor_Lebanon.Models.ViewModels;namespace Zoor_Lebanon.Areas.admin.Controllers{    [Area("admin")]    public class PackageController : Controller    {        private readonly zoor_lebanonContext _context;        public PackageController(zoor_lebanonContext context)        {            _context = context;        }

        // GET: Add Package
        [HttpGet]        public async Task<IActionResult> AddPackage()        {            var viewModel = new PackageViewModel            {                Package = new Package(),                PackageTypes = await _context.PackageTypes.ToListAsync(),                States = await _context.Locations.Select(l => l.State).Distinct().ToListAsync()            };            return View(viewModel);        }

        // POST: Add Package with Location
        [HttpPost]        public async Task<IActionResult> AddPackage(PackageViewModel model, string state, string city)        {
            // Validate dates: Ensure start_date is today or later
            if (model.Package.StartDate.HasValue &&
                model.Package.StartDate.Value.ToDateTime(TimeOnly.MinValue) < DateTime.Today)
            {
                ModelState.AddModelError("Package.StartDate", "Start date cannot be in the past.");
                model.PackageTypes = await _context.PackageTypes.ToListAsync();
                model.States = await _context.Locations.Select(l => l.State).Distinct().ToListAsync();
                return View(model);
            }

            if (model.Package.EndDate <= model.Package.StartDate)
            {
                ModelState.AddModelError("Package.EndDate", "End date must be after the start date.");
                model.PackageTypes = await _context.PackageTypes.ToListAsync();
                model.States = await _context.Locations.Select(l => l.State).Distinct().ToListAsync();
                return View(model);
            }

            // Fetch LocationId based on selected State and City
            var location = await _context.Locations
.FirstOrDefaultAsync(l => l.State == state && l.City == city);            if (location == null)            {                ModelState.AddModelError("", "Invalid Location selection.");                model.PackageTypes = await _context.PackageTypes.ToListAsync();                model.States = await _context.Locations.Select(l => l.State).Distinct().ToListAsync();                return View(model);            }

            // Create Package Object
            var package = new Package            {                PackageName = model.Package.PackageName,                Description = model.Package.Description,                UnitPrice = model.Package.UnitPrice,                TotalSpots = model.Package.TotalSpots,                AverageDuration = model.Package.AverageDuration,                StartDate = model.Package.StartDate,                EndDate = model.Package.EndDate,                LocationId = location.LocationId, // Foreign key for Location
                PackageTypeId = model.Package.PackageTypeId // Foreign key for PackageType
            };            _context.Packages.Add(package);            await _context.SaveChangesAsync();
            return RedirectToAction("Index2", "Home", new { area = "admin" });        }

        // GET: Fetch Cities by State (Ajax)
        [HttpGet]        public async Task<IActionResult> GetCitiesByState(string state)        {            var cities = await _context.Locations                .Where(l => l.State == state)                .Select(l => l.City)                .Distinct()                .ToListAsync();            return Json(cities);        }

        // GET: Fetch LocationId (Ajax)
        [HttpGet]        public async Task<IActionResult> GetLocationId(string state, string city)        {            var locationId = await _context.Locations                .Where(l => l.State == state && l.City == city)                .Select(l => l.LocationId)                .FirstOrDefaultAsync();            return Json(locationId);        }

        // GET: View Widgets (Packages)
        [HttpGet]        public async Task<IActionResult> Widgets()        {            var packages = await _context.Packages                .Include(p => p.Location)                .Include(p => p.PackageType)                .OrderByDescending(p => p.PackageId)                .ToListAsync();            return View(packages); // Ensure the view file is named "Widgets.cshtml"
        }

        // GET: Manage Packages
       /* [HttpGet]        public async Task<IActionResult> ManagePackages()        {            var packages = await _context.Packages                .Include(p => p.Location)                .Include(p => p.PackageType)                .OrderByDescending(p => p.PackageId)                .ToListAsync();            return View(packages);        }*/

        // GET: Search Package
        [HttpGet]        public async Task<IActionResult> SearchPackage(string packageName)        {            if (string.IsNullOrWhiteSpace(packageName))            {                TempData["ErrorMessage"] = "Please enter a valid package name.";                return RedirectToAction(nameof(Widgets));            }            var package = await _context.Packages                .Include(p => p.Location)                .Include(p => p.PackageType)                .FirstOrDefaultAsync(p => EF.Functions.Like(p.PackageName, $"{packageName}%"));            if (package == null)            {                TempData["ErrorMessage"] = $"No package found with the name '{packageName}'.";                return RedirectToAction(nameof(Widgets));            }            return View("PackageDetails", package); // Ensure you have a view named "PackageDetails.cshtml"
        }

        // GET: Package Details
        [HttpGet]        public async Task<IActionResult> PackageDetails(int id)        {            var package = await _context.Packages                .Include(p => p.Location)                .Include(p => p.PackageType)                .FirstOrDefaultAsync(p => p.PackageId == id);            if (package == null)            {                TempData["ErrorMessage"] = "Package not found.";                return RedirectToAction(nameof(Widgets));            }            return View(package);        }

        // POST: Edit Package
        [HttpPost]        [ValidateAntiForgeryToken]        public async Task<IActionResult> EditPackage(int id, PackageViewModel model)        {            if (id != model.Package.PackageId)            {                return NotFound();            }            if (ModelState.IsValid)            {                try                {                    _context.Update(model.Package);                    await _context.SaveChangesAsync();                }                catch (DbUpdateConcurrencyException)                {                    if (!PackageExists(id))                    {                        return NotFound();                    }                    throw;                }                return RedirectToAction(nameof(Widgets));            }            model.PackageTypes = await _context.PackageTypes.ToListAsync();            model.States = await _context.Locations.Select(l => l.State).Distinct().ToListAsync();            return View(model);        }

        // POST: Delete Package
        [HttpPost]        [ValidateAntiForgeryToken]        public async Task<IActionResult> DeletePackage(int id)        {            var package = await _context.Packages.FindAsync(id);            if (package == null)            {                return NotFound();            }            _context.Packages.Remove(package);            await _context.SaveChangesAsync();            return RedirectToAction(nameof(Widgets));        }        private bool PackageExists(int id)        {            return _context.Packages.Any(e => e.PackageId == id);        }    }}