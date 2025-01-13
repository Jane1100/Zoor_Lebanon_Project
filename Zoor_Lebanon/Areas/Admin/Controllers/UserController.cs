using Microsoft.AspNetCore.Mvc;

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Zoor_Lebanon.Models;
using System.Text.RegularExpressions;
using Zoor_Lebanon.Models.Helper;
using Zoor_Lebanon.Models.ViewModels;

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
/*
        [HttpGet]
        public IActionResult RegisterU()
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
        public async Task<IActionResult> RegisterU(RegisterViewModel model)
        {
            // Ensure the form is valid
            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns
                model.Countries = _context.Countries.ToList();
                model.States = model.CountryId.HasValue
                    ? _context.States.Where(s => s.CountryId == model.CountryId.Value).ToList()
                    : new List<State>();
                model.Cities = model.StateId.HasValue
                    ? _context.Cities.Where(c => c.StateId == model.StateId.Value).ToList()
                    : new List<City>();
                model.Preferences = _context.Preferences.ToList();

                return View(model);
            }

            if (_context.Users.Any(u => u.Email == model.User.Email))
            {
                ModelState.AddModelError("", "This email is already in use."); // General error
                                                                               // Repopulate dropdowns
                model.Countries = _context.Countries.ToList();
                model.States = model.CountryId.HasValue
                    ? _context.States.Where(s => s.CountryId == model.CountryId.Value).ToList()
                    : new List<State>();
                model.Cities = model.StateId.HasValue
                    ? _context.Cities.Where(c => c.StateId == model.StateId.Value).ToList()
                    : new List<City>();
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

            HttpContext.Session.SetInt32("UserId", model.User.UserId);
            HttpContext.Session.SetString("UserName", model.User.Firstname + " " + model.User.Lastname);


            return RedirectToAction("Index", "Home");
        }*/

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





        /*  [HttpGet]
          public IActionResult RegisterU()
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
          public async Task<IActionResult> RegisterU(RegisterViewModel model)
          {
              // Ensure the form is valid
              if (!ModelState.IsValid)
              {
                  // Repopulate dropdowns
                  model.Countries = _context.Countries.ToList();
                  model.States = model.CountryId.HasValue
                      ? _context.States.Where(s => s.CountryId == model.CountryId.Value).ToList()
                      : new List<State>();
                  model.Cities = model.StateId.HasValue
                      ? _context.Cities.Where(c => c.StateId == model.StateId.Value).ToList()
                      : new List<City>();
                  model.Preferences = _context.Preferences.ToList();

                  return View(model);
              }

              if (_context.Users.Any(u => u.Email == model.User.Email))
              {
                  ModelState.AddModelError("", "This email is already in use."); // General error
                                                                                 // Repopulate dropdowns
                  model.Countries = _context.Countries.ToList();
                  model.States = model.CountryId.HasValue
                      ? _context.States.Where(s => s.CountryId == model.CountryId.Value).ToList()
                      : new List<State>();
                  model.Cities = model.StateId.HasValue
                      ? _context.Cities.Where(c => c.StateId == model.StateId.Value).ToList()
                      : new List<City>();
                  model.Preferences = _context.Preferences.ToList();

                  return View(model);
              }


              // Validate the password requirements
              var passwordRegex = new Regex(@"^.{8,}$");  // Allows any characters with a minimum length of 8
              if (!passwordRegex.IsMatch(model.User.PasswordHash))
              {
                  ModelState.AddModelError("User.PasswordHash", "Password must be at least 8 characters long.");
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

             *//* // Save preferences in UserPreferences table
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
              }*//*

              HttpContext.Session.SetInt32("UserId", model.User.UserId);
              HttpContext.Session.SetString("UserName", model.User.Firstname + " " + model.User.Lastname);


              return RedirectToAction("Index3", "index");
          }
  */

        /*
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
        */
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
    }
}
        //[HttpGet]
     /*   public async Task<IActionResult> MyProfile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if not logged in
            }

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null)
            {
                // Handle the user not found
                return View("Error"); // Create an Error view to handle this gracefully
            }

            var model = new UserProfileViewModel
            {
                UserId = user.UserId,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                Dob = user.Dob.HasValue ? user.Dob.Value.ToString("yyyy-MM-dd") : null, // Format DateOnly to string
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }
*/
        // POST: Admin/User/MyProfile
     /*   [HttpPost]
        public async Task<IActionResult> MyProfile(UserProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Return the view with validation errors
            }

            var user = await _context.Users.FindAsync(model.UserId);
            if (user == null)
            {
                // Handle the user not found
                return View("Error"); // Again, consider having a proper error view
            }

            // Update the properties
            user.Firstname = model.Firstname;
            user.Lastname = model.Lastname;
            user.Email = model.Email;
            user.Dob = DateOnly.Parse(model.Dob);
            user.PhoneNumber = model.PhoneNumber;

            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("ProfileUpdated"); // Redirect to a confirmation page or back to the profile
            }
            catch (DbUpdateException ex)
            {
                // Log the error (uncomment ex variable name and write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(model);
        }
    }*/
    /*      [HttpGet]
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
                .Include(u => u.Rewards)
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                return RedirectToAction("Login"); // Redirect to login if the user is not found
            }

            ViewBag.Cities = _context.Cities.ToList();

            var mostRecentReward = _context.Rewards
        .Where(r => r.UserId == userId)
        .OrderByDescending(r => r.RewardsId)
        .FirstOrDefault();

            // Get redeemed rewards (if needed)
            var redeemedRewards = _context.UserCoupons
                .Include(uc => uc.Coupon)
                .Where(uc => uc.UserId == userId && uc.IsRedeemed == true)
                .Select(uc => new
                {
                    uc.Coupon.Description,
                    uc.UnlockDate
                })
                .ToList();

            // Prepare the model
            var model = new UserProfileViewModel
            {
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                Dob = user.Dob?.ToString("yyyy-MM-dd"),
                CityName = user.City?.City1,
                CityId = user.CityId,
                PointsBalance = mostRecentReward?.CurrentBalance ?? 0, // Use the most recent balance
                RedeemedRewards = redeemedRewards.Select(rr => new RedeemedReward
                {
                    Description = rr.Description,
                    UnlockDate = rr.UnlockDate?.ToString("yyyy-MM-dd")
                }).ToList()
            };

            return View(model);
        }



        [HttpPost]
        public async Task<IActionResult> MyProfile(UserProfileViewModel model)
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
            user.Dob = !string.IsNullOrEmpty(model.Dob) ? DateOnly.FromDateTime(DateTime.Parse(model.Dob)) : null;
            user.CityId = model.CityId;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("MyProfile");
        }*/


    /*    [HttpGet]
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
        }*/


  /*  [HttpPost]
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
*/

    
