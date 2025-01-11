using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zoor_Lebanon.Models;

namespace Zoor_Lebanon.Areas.EndUser.Controllers
{
    [Area("EndUser")]
    public class SearchController : Controller
    {
        private readonly zoor_lebanonContext _context;

        public SearchController(zoor_lebanonContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return View(new List<Package>());
            }

            query = query.ToLower(); // Normalize query for case-insensitive comparison

            var results = _context.Packages
                .Include(p => p.ActivitySchedules) // Include related data if needed in the view
                .Where(p =>
                    EF.Functions.Like(p.PackageName.ToLower(), $"%{query}%") || // Search in PackageName
                    EF.Functions.Like(p.Description.ToLower(), $"%{query}%") || // Search in Description
                    p.ActivitySchedules.Any(a =>
                        a.Description != null && EF.Functions.Like(a.Description.ToLower(), $"%{query}%") // Search in related ActivitySchedules
                    )
                )
                .ToList();

            return View(results);
        }

    }
}
