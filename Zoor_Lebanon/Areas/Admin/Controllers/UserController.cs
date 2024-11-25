using Microsoft.AspNetCore.Mvc;
using Zoor_Lebanon_Booking_Platform.Models.Database;
using System.Linq;

namespace Zoor_Lebanon_Booking_Platform.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ZoorLebanonContext _context;

        public UserController(ZoorLebanonContext context)
        {
            _context = context;
        }
     
        public IActionResult ProfileU(int userId)
        {
            // Fetch user details from the database using the userId
            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Pass the user data to the view
            return View(user);
        }

        // GET: Manage Tourists
        public IActionResult Tourists()
        {
            // Fetch all users from the database
            var users = _context.Users.Select(user => new
            {
                user.UserId,
                FullName = $"{user.Firstname} {user.Lastname}",
                user.Email,
                Status = user.Active.HasValue && user.Active.Value ? "Active" : "Not Active"
            }).ToList();

            // Pass the user data to the view
            ViewBag.Users = users;

            return View();
        }
    }
}
