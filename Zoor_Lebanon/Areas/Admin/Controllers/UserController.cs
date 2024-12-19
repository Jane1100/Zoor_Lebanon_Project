using Microsoft.AspNetCore.Mvc;
using Zoor_Lebanon.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Zoor_Lebanon.Models.UserProfileViewModel;
using Zoor_Lebanon.Models.RegisterViewModel;
using System.Text.RegularExpressions;
using Zoor_Lebanon.Models.Helper;

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

        /*     public IActionResult ProfileU(int userId)
             {
                 var user = _context.Users.FirstOrDefault(u => u.UserId == userId);
                 if (user == null)
                 {
                     return NotFound("User not found");
                 }
                 return View(user);
             }

     */
        [HttpPost("Admin/User/Delete/{userId}")]
        public async Task<IActionResult> Delete(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Tourists"); // Redirect to the tourists listing page or wherever appropriate
            }

            try
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as per your error handling policy
                TempData["ErrorMessage"] = $"Error deleting user: {ex.Message}";
            }

            return RedirectToAction("Tourists"); // Redirect back to the tourists listing page
        }





        [HttpGet]
        public IActionResult GetStates(int countryId)
        {
            var states = _context.States.Where(s => s.CountryId == countryId).ToList();
            return Json(states);
        }

        [HttpGet]
        public IActionResult GetCities(int stateId)
        {
            var cities = _context.Cities.Where(c => c.StateId == stateId).ToList();
            return Json(cities);
        }

        public IActionResult Tourists()
        {
            var users = _context.Users
                .Select(user => new
                {
                    user.UserId,
                    FullName = $"{user.Firstname} {user.Lastname}",
                    user.Email,
                    Status = _context.Bookings.Any(b => b.UserId == user.UserId) ? "Active" : "Not Active"
                }).ToList();

            ViewBag.Users = users;
            return View();
        }

        [HttpGet]
        public IActionResult MyProfile(int? userId)
        {
            userId = userId ?? HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                TempData["Error"] = "User identification is required to access this page.";
                return RedirectToAction("Tourists");
            }

            var user = _context.Users
                               .Include(u => u.City)
                               .FirstOrDefault(u => u.UserId == userId.Value);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Tourists");
            }

            var model = new UserProfileViewModel
            {
                UserId = user.UserId,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                Dob = user.Dob?.ToString("yyyy-MM-dd"),
                CityId = user.CityId
            };

            ViewBag.Cities = _context.Cities.ToList();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> MyProfile(UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Cities = _context.Cities.ToList();
                return View(model);
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                TempData["Error"] = "Session expired or invalid user.";
                return RedirectToAction("Tourists");
            }

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Tourists");
            }

            user.Firstname = model.Firstname;
            user.Lastname = model.Lastname;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber; // Concatenate country code and phone number
            user.Dob = !string.IsNullOrEmpty(model.Dob) ? DateOnly.Parse(model.Dob) : user.Dob;
            user.CityId = model.CityId;

            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profile updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error updating profile: " + ex.Message;
                return View(model); // Stay on the page to display error
            }

            return RedirectToAction("Tourists"); // Redirect to prevent resubmission
        }


        [HttpPost]
        public async Task<IActionResult> UpdateProfile(TourOperator model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Validation Error.";
                return View(model);
            }

            var existingOperator = await _context.TourOperators.FindAsync(model.OperatorId);
            if (existingOperator == null)
            {
                TempData["ErrorMessage"] = "Tour Operator not found.";
                return RedirectToAction("Operators");
            }

            existingOperator.CompanyName = model.CompanyName;
            existingOperator.BusinessPhone = model.BusinessPhone;

            try
            {
                _context.Update(existingOperator);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Profile updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating profile: {ex.Message}";
            }

            return RedirectToAction("Operators");
        }

       
        [HttpGet]
        public IActionResult AddU()
        {
            var model = new RegisterViewModel
            {
                User = new User(), // Initialize User to prevent null reference
                Preferences = _context.Preferences.ToList(),
                Countries = _context.Countries.ToList(),
                States = new List<State>(),
                Cities = new List<City>(),
                CountryCode = ""
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddU(RegisterViewModel model)
        {
            // Ensure the form is valid
            if (!ModelState.IsValid)
            {
                model.Countries = _context.Countries.ToList();
                model.Preferences = _context.Preferences.ToList();
                return View(model);
            }

            // Check if the email already exists
            if (_context.Users.Any(u => u.Email == model.User.Email))
            {
                ModelState.AddModelError("User.Email", "Email already in use.");
                model.Countries = _context.Countries.ToList();
                model.Preferences = _context.Preferences.ToList();
                return View(model);
            }

            // Validate the password requirements
            var passwordRegex = new Regex(@"^(?=.*[A-Z])(?=.*\d)(?=.*[@@$!%*?&])[A-Za-z\d@@$!%*?&]{8,16}$");
            if (!passwordRegex.IsMatch(model.User.PasswordHash))
            {
                ModelState.AddModelError("User.PasswordHash", "Password does not meet the requirements.");
                model.Countries = _context.Countries.ToList();
                model.Preferences = _context.Preferences.ToList();
                return View(model);
            }

            // Hash the password
            var (hash, salt) = PasswordHelper.HashPassword(model.User.PasswordHash);
            model.User.PasswordHash = hash;
            model.User.Salt = salt;

            // Save additional fields
            model.User.RoleId = 1;
            model.User.PhoneNumber = model.CountryCode + model.User.PhoneNumber;
            model.User.Active = true;
            model.User.CityId = model.CityId;

            // Save the user to the database
            _context.Users.Add(model.User);
            await _context.SaveChangesAsync();

            // Save preferences in UserPreferences table
            if (model.SelectedPreferences != null && model.SelectedPreferences.Count > 0)
            {
                foreach (var preferenceId in model.SelectedPreferences)
                {
                    var userPreference = new UserPreference
                    {
                        UserId = model.User.UserId,
                        PreferenceId = preferenceId
                    };
                    _context.UserPreferences.Add(userPreference);
                }
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("tourists", "User");
        }

      
       
    }
}
