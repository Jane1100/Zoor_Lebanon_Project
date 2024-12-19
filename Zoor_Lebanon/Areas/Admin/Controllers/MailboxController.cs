using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zoor_Lebanon.Models;
using Zoor_Lebanon.Models.ViewModels;

namespace Zoor_Lebanon.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MailboxController : Controller
    {
        private readonly zoor_lebanonContext _context;

        public MailboxController(zoor_lebanonContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> ApprovePackage(int packageId)
        {
            try
            {
                // Find the package by ID
                var package = await _context.Packages.FindAsync(packageId);
                if (package == null)
                {
                    return Json(new { success = false, message = "Package not found." });
                }

                // Update the package status to "Active"
                package.Status = "Active";
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Package approved successfully." });
            }
            catch (Exception ex)
            {
                // Return error message in case of exception
                return Json(new { success = false, message = $"Error approving package: {ex.Message}" });
            }
        }



        // GET: Display pending packages
        public async Task<IActionResult> mailbox()
        {
            var pendingPackages = await _context.Packages
                .Where(p => p.Status == "Pending")
                .OrderBy(p => p.PackageId)
                .Select(p => new
                {
                    p.PackageId,
                    p.PackageName,
                    p.Description,
                    // Additional fields can be included here
                })
                .ToListAsync();

            return View(pendingPackages);
        }

        // GET: Display a specific package for editing
        // Adjust the HTTP Get method to match the URL pattern you intend to use
        [HttpGet("~/Admin/Mailbox/OptRequest/{packageId}")]

        public async Task<IActionResult> OptRequest(int? packageId)
        {
            if (packageId == null)
            {
                return NotFound();
            }

            var package = await _context.Packages
                .Include(p => p.Location)
                .FirstOrDefaultAsync(p => p.PackageId == packageId.Value);

            if (package == null)
            {
                return NotFound();
            }

            var viewModel = new PackageViewModel
            {
                Package = package,
                PackageTypes = await _context.PackageTypes.ToListAsync(),
                States = await _context.Locations.Select(l => l.State).Distinct().ToListAsync()
            };

            return View(viewModel);
        }


        // POST: Submit the edited package
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OptRequest(int packageId, PackageViewModel model, string state, string city)
        {
            if (packageId != model.Package.PackageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var location = await _context.Locations
.FirstOrDefaultAsync(l => l.State == state && l.City == city);


                    if (location == null)
                    {
                        ModelState.AddModelError("", "Invalid Location selection.");
                        model.PackageTypes = await _context.PackageTypes.ToListAsync();
                        model.States = await _context.Locations.Select(l => l.State).Distinct().ToListAsync();
                        return View(model);
                    }


                    var packageToUpdate = await _context.Packages.FindAsync(packageId);

                    if (packageToUpdate != null)
                    {
                        packageToUpdate.PackageName = model.Package.PackageName;
                        packageToUpdate.Description = model.Package.Description;
                        packageToUpdate.UnitPrice = model.Package.UnitPrice;
                        packageToUpdate.StartDate = model.Package.StartDate;
                        packageToUpdate.EndDate = model.Package.EndDate;
                        packageToUpdate.Status = "Active"; // Change status to Active
                        packageToUpdate.LocationId = location.LocationId; // Update the location

                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PackageExists(packageId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); // Redirect to the index or appropriate view
            }

            // Reload the necessary data if model state is not valid
            model.PackageTypes = await _context.PackageTypes.ToListAsync();
            model.States = await _context.Locations.Select(l => l.State).Distinct().ToListAsync();
            if (!string.IsNullOrEmpty(model.Package.Location.State))
            {
                model.Cities = await _context.Locations.Where(l => l.State == model.Package.Location.State).Select(l => l.City).Distinct().ToListAsync();
            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> GetCitiesByState(string state)
        {
            var cities = await _context.Locations
                .Where(l => l.State == state)
                .Select(l => l.City)
                .Distinct()
                .ToListAsync();

            return Json(cities);
        }

        // GET: Fetch LocationId (Ajax)
        [HttpGet]
        public async Task<IActionResult> GetLocationId(string state, string city)
        {
            var locationId = await _context.Locations
                .Where(l => l.State == state && l.City == city)
                .Select(l => l.LocationId)
                .FirstOrDefaultAsync();

            return Json(locationId);
        }

        private bool PackageExists(int id)
        {
            return _context.Packages.Any(e => e.PackageId == id);
        }
    }
}
