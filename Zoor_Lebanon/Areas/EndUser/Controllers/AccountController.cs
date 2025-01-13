using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Zoor_Lebanon.Models;
using Zoor_Lebanon.Models.Helper;
using Zoor_Lebanon.Models.ViewModels;

namespace Zoor_Lebanon.Areas.EndUser.Controllers
{
    [Area("EndUser")]
    public class AccountController : Controller
    {
        private readonly zoor_lebanonContext _context;
        public AccountController(zoor_lebanonContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
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
        public async Task<IActionResult> Register(RegisterViewModel model)
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


        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == email);

            if (user == null || !PasswordHelper.VerifyPassword(password, user.PasswordHash, user.Salt))
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View();
            }
            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("UserName", user.Firstname + " " + user.Lastname);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == email);
            if (user == null)
            {
                ModelState.AddModelError("", "Email does not exist");
                return View();
            }


            // Send the reset link with the token via email
            var resetLink = Url.Action("ResetPassword", "Account", new { email = user.Email }, Request.Scheme);

            // Assuming you have an email service, you would send the reset link like this:
            // await _emailService.SendPasswordResetEmail(user.Email, resetLink);

            return View("ForgotPasswordConfirmation");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // Clear the user's session or token
            HttpContext.Session.Clear();

            // Redirect to homepage
            return RedirectToAction("Login");
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
            if (string.IsNullOrWhiteSpace(model.Firstname) || string.IsNullOrWhiteSpace(model.Lastname))
            {
                TempData["ErrorMessage"] = "Firstname and Lastname are required fields.";
                return RedirectToAction("MyProfile"); // Redirect back to the profile page
            }
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
            user.Dob = !string.IsNullOrEmpty(model.Dob) ? DateTime.Parse(model.Dob) : null;
            user.CityId = model.CityId;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("MyProfile");
        }


    }
}