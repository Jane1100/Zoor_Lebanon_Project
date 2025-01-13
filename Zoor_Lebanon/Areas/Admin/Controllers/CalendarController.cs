using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zoor_Lebanon.Models;  // Ensure your namespace matches the one used in your project
using System.Linq;
using System.Threading.Tasks;

namespace Zoor_Lebanon.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CalendarController : Controller
    {
        private readonly zoor_lebanonContext _context;  // Adjust the context name to match yours

        public CalendarController(zoor_lebanonContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetPackageEvents()
        {
            var packages = _context.Packages
                .Where(p => p.StartDate.HasValue && p.EndDate.HasValue) // Ensure dates are not null
                .Select(p => new {
                    title = p.PackageName, // 'title' should be lowercase as JavaScript is case-sensitive
                    start = p.StartDate, // Convert DateOnly to DateTime
                    end = p.EndDate, // Convert DateOnly to DateTime
                    backgroundColor = "#00a65a", // Example: green for all
                    borderColor = "#00a65a"
                })
                .ToList();

            return Json(packages);
        }



        // This action returns the calendar view with the packages
        public async Task<IActionResult> Calendar()
        {
            var packages = await _context.Packages
                .Where(p => p.StartDate.HasValue)  // Ensure only packages with a start date are included
                .Select(p => new
                {
                    p.PackageName,
                    StartDate = p.StartDate.HasValue ? p.StartDate : (DateTime?)null
                })
                .ToListAsync();

            ViewBag.Packages = packages;  // Passing the package data to the view through ViewBag
            return View();
        }
    }
}
