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
        public async Task<IActionResult> mailbox()
        {
            var pendingPackages = await _context.Packages
                .Where(p => p.Status == "Pending")
                .OrderBy(p => p.PackageId)  // Sorting by PackageId for lack of a submission date
                .Select(p => new
                {
                    p.PackageId,
                    p.PackageName,
                    p.Description,
                  //  TourOperatorName = p.TourOperator.CompanyName, // Assuming there's a navigation property
                })
                .ToListAsync();

            return View(pendingPackages);
        }
        [HttpGet]
        public async Task<IActionResult> OptRequest(int id)
        {
            var package = await _context.Packages
                                       .Include(p => p.Location)  // Ensure Location is included
                                       .FirstOrDefaultAsync(p => p.PackageId == id);

            if (package == null)
            {
                return NotFound("Package not found");
            }

            // Assuming State is a property of Location and is properly populated
            var packageViewModel = new PackageViewModel
            {
                Package = package,
                PackageTypes = await _context.PackageTypes.ToListAsync(), // Ensure this is loaded
                States = await _context.States.Select(s => s.State1).ToListAsync(), // Assuming States is loaded properly
                Cities = new List<string?>() // Placeholder for cities
            };
            return View(packageViewModel);
        }




        [HttpPost]
        public async Task<IActionResult> ApprovePackage(int id)
        {
            var package = await _context.Packages.FindAsync(id);
            if (package != null && package.Status == "Pending")
            {
                package.Status = "active"; // Change to 'Active' if that is the correct status you wish to set
                await _context.SaveChangesAsync();
                return Json(new { success = true }); // Send back a success status
            }
            return Json(new { success = false }); // Handle failure cases
        }


    }

}
