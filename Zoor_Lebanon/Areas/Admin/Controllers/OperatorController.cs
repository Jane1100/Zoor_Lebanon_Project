using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Zoor_Lebanon.Models;

namespace Zoor_Lebanon.Controllers
{
    [Area("Admin")]
    public class OperatorController : Controller
    {
        private readonly zoor_lebanonContext _context;

        public OperatorController(zoor_lebanonContext context)
        {
            _context = context;
        }

        public IActionResult Operators()
        {
            // Fetch TourOperators with their related User and City
            var operators = _context.TourOperators
                .Include(o => o.User) // Include User relationship
                .ThenInclude(u => u.City) // Include City relationship
                .Select(o => new
                {
                    o.OperatorId,
                    o.CompanyName,
                    o.BusinessPhone,
                    Firstname = o.User.Firstname,
                    Lastname = o.User.Lastname,
                    CityName = o.User.City.City1,

                })
                .ToList();

            return View(operators);
        }
    }
}
