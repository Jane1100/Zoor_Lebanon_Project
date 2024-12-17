using Microsoft.AspNetCore.Mvc;
using Zoor_Lebanon.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Zoor_Lebanon.Models.UserProfileViewModel;

namespace Zoor_Lebanon.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly zoor_lebanonContext _context;

        public UserController(zoor_lebanonContext context)
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
            // Fetch all users and determine their status based on presence in the booking table
            var users = _context.Users
                .Select(user => new
                {
                    user.UserId,
                    FullName = $"{user.Firstname} {user.Lastname}",
                    user.Email,
                    // Check if there are any bookings associated with the user to determine status
                    Status = _context.Bookings.Any(b => b.UserId == user.UserId) ? "Active" : "Not Active"
                }).ToList();

            // Pass the user data to the view
            ViewBag.Users = users;

            return View();
        }

        [HttpGet]
        public IActionResult MyProfile()
        {
            // Get the logged-in user's ID from the session
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login"); // Redirect to login if the user is not logged in
            }

            // Fetch user details from the database
            var user = _context.Users
                .Include(u => u.City)
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                return RedirectToAction("Login"); // Redirect to login if the user is not found
            }

            ViewBag.Cities = _context.Cities.ToList();

            var model = new UserProfileViewModel
            {
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                Dob = user.Dob?.ToString("yyyy-MM-dd"),
                CityName = user.City?.City1,
                CityId = user.CityId
            };

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> MyProfile(UserProfileViewModel model, DateTime dateTime)
        {
            if (!ModelState.IsValid)
            {
                // Reload city dropdown or any other data if required
                model.CityName = _context.Cities.FirstOrDefault(c => c.CityId == model.CityId)?.City1;
                return View(model);
            }

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            // Update user information
            user.Firstname = model.Firstname;
            user.Lastname = model.Lastname;
            user.Email = model.Email;
            user.Dob = !string.IsNullOrEmpty(model.Dob) ? DateOnly.FromDateTime(DateTime.Parse(model.Dob)) : (DateOnly?)null;

            user.CityId = model.CityId;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("MyProfile");
        }


    }
}
