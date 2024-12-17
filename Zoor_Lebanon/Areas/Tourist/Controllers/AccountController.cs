using Microsoft.AspNetCore.Mvc;
using Zoor_Lebanon.Models;
using Zoor_Lebanon.Models.Helper;
using Zoor_Lebanon.Models.RegisterViewModel;

namespace Zoor_Lebanon.Areas.Tourist.Controllers
{
    [Area("Tourist")]
    public class AccountController : Controller
    {
        private readonly zoor_lebanonContext _context;
        private readonly JwtHelper _jwtHelper;
        public AccountController(zoor_lebanonContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }

        [HttpGet]
        public IActionResult Register()
        {
            var viewModel = new RegisterViewModel
            {
                Countries = _context.Countries.ToList(), // Fetch the list of countries
                Preferences = _context.Preferences.ToList() // Fetch preferences if needed
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user, string password)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email already in use");
                return View(user);
            }

            var (hash, salt) = PasswordHelper.HashPassword(password);
            user.PasswordHash = hash;
            user.Salt = salt;
            user.RoleId = 1; // Assuming RoleId = 1 is for Tourist
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");
        }
        [HttpGet]
        public JsonResult GetStates(int countryId)
        {
            var states = _context.States
                .Where(s => s.CountryId == countryId)
                .Select(s => new { s.StateId, s.State1 })
                .ToList();
            return Json(states);
        }
        [HttpGet]
        public JsonResult GetCities(int stateId)
        {
            var cities = _context.Cities
                .Where(c => c.StateId == stateId)
                .Select(c => new { c.CityId, c.City1 })
                .ToList();
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
                ModelState.AddModelError("", "Invalid email or password");
                return View();
            }

            var token = _jwtHelper.GenerateJwtToken(user);

            HttpContext.Response.Headers.Add("Authorization", "Bearer " + token);

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

            // Generate a JWT token for password reset with a short expiration
            var resetToken = _jwtHelper.GenerateJwtToken(user, expireMinutes: 15);

            // Send the reset link with the token via email
            var resetLink = Url.Action("ResetPassword", "Account", new { token = resetToken, email = user.Email }, Request.Scheme);

            // Assuming you have an email service, you would send the reset link like this:
            // await _emailService.SendPasswordResetEmail(user.Email, resetLink);

            return View("ForgotPasswordConfirmation");
        }
    }
}
