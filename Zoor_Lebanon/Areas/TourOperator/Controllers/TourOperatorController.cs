using Microsoft.AspNetCore.Mvc;
using Zoor_Lebanon.Models;
using System.Linq;
using Zoor_Lebanon.Models;
using Zoor_Lebanon.Models.ViewModels;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;


namespace Zoor_Lebanon.Areas.TourOperator.Controllers
{
    [Area("TourOperator")]
    public class TourOperatorController : Controller
    {
        private readonly zoor_lebanonContext _context;

        public TourOperatorController(zoor_lebanonContext context)
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
        public IActionResult tourists()
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

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(User model)
        {
            if (model == null || model.UserId <= 0)
            {
                TempData["ErrorMessage"] = "Invalid user data.";
                return RedirectToAction("ProfileU", new { userId = model.UserId });
            }

            var user = await _context.Users.FindAsync(model.UserId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("ProfileU", new { userId = model.UserId });
            }

            try
            {
                // Check if values are updated
                if (user.Firstname != model.Firstname ||
                    user.Lastname != model.Lastname ||
                    user.Email != model.Email ||
                    user.PhoneNumber != model.PhoneNumber ||
                    user.Dob != model.Dob)
                {
                    user.Firstname = model.Firstname;
                    user.Lastname = model.Lastname;
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Dob = model.Dob;

                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Profile updated successfully.";
                }
                else
                {
                    TempData["InfoMessage"] = "No changes detected.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating profile: {ex.Message}";
            }

            return RedirectToAction("ProfileU", new { userId = model.UserId });
        }




        [HttpGet]
        public async Task<IActionResult> UpcomingBookings()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var bookings = await _context.Bookings
                .Include(b => b.Package)
                .ToListAsync();

            // Separate into Upcoming and Previous Packages
            var allPackages = bookings
                .GroupBy(b => new { b.PackageId, b.Package.PackageName })
                .Select(group => new
                {
                    PackageId = group.Key.PackageId,
                    PackageName = group.Key.PackageName,
                    BookingCount = group.Count(),
                    EarliestTravelDate = group.Where(b => b.TravelDate >= today)
                                              .Min(b => b.TravelDate),
                    LatestTravelDate = group.Where(b => b.TravelDate < today)
                                            .Max(b => b.TravelDate)
                })
                .ToList();

            // Pass data directly as a model
            return View(allPackages);
        }



        public IActionResult Dashboard()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return View();
        }



        public async Task<IActionResult> Notifications()
        {
            // Get packages that have been booked
            var notifications = await _context.Bookings
                .Include(b => b.Package)
                .Select(b => new
                {
                    b.Package.PackageName,
                    Message = $"{b.Package.PackageName} has been booked."
                })
                .Distinct()
                .ToListAsync();

            ViewBag.Notifications = notifications;
            return View();
        }


    }
}
