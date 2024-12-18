using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
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
        public IActionResult Addtr()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddTourOperator(TourOperator model)
        {
            // Check if user exists based on the provided first and last name
            var user = _context.Users.FirstOrDefault(u => u.Firstname.Equals(model.User.Firstname) && u.Lastname.Equals(model.User.Lastname));
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return View(model);
            }
            if (model.BusinessPhone != null && model.BusinessPhone.Length > 20)
            {
                ModelState.AddModelError("BusinessPhone", "Business phone cannot exceed 20 characters.");
                return View(model);
            }
            // If user exists, proceed to add a new Tour Operator
            model.UserId = user.UserId; // Assign the found user's ID to the model
            _context.TourOperators.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tour Operator added successfully.";
            return RedirectToAction("Index"); // Adjust redirection as needed
        }

      
        [HttpGet("Admin/Operator/ProfileU/{userId}")]
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




       /* [HttpGet]
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
*/


     /*   public IActionResult Dashboard()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return View();
        }
*/

    }
}
