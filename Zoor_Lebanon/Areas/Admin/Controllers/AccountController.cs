using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Zoor_Lebanon.Models.Helper;
using Zoor_Lebanon.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Zoor_Lebanon.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {

        
        private readonly zoor_lebanonContext _context;

        public AccountController(zoor_lebanonContext context)
        {
            _context = context;
        }


        // Hardcoded admin credentials
          private readonly string adminFirstName = "Mariejose";
          private readonly string adminLastName = "Khalil";
          private readonly string adminEmail = "mariejosekhalil29@gmail.com";
          private readonly string adminPassword = "Mariejose.kh4";

          [HttpGet]
          public IActionResult Login()
          {
              // Ensure the error message is cleared on the initial load
              ViewBag.Error = null;
              return View();
          }

          [HttpPost]
          public IActionResult Login(string email, string password)
          {
              // Validate email and password against hardcoded values
              if (email == adminEmail && password == adminPassword)
              {
                  // Redirect to the Admin Home page upon successful login
                  return Redirect("~/admin/package/index2");
              }

              // If invalid, return an error message and reload the login page
              ViewBag.Error = "Invalid email or password.";
              return View();
          }


       /* [HttpGet]
        public IActionResult Login() => View();


        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Email and password are required.";
                return View();
            }

            var user = _context.Users.SingleOrDefault(u => u.Email == email);

            if (user == null || !VerifyPasswordHash(password, user.PasswordHash, user.Salt))
            {
                ViewBag.Error = "Invalid login attempt.";
                return View();
            }

            if (user.RoleId.HasValue && user.Role != null && user.Role.RoleName == "admin" && user.Active == true)
            {
                // Set up your session or authentication mechanism here
                // For example, HttpContext.Session.SetString("User", user.UserId.ToString());
                return RedirectToAction("Index", "Home"); // Redirect to the admin home page or dashboard
            }

            ViewBag.Error = "You are not authorized to access this section.";
            return View();
        }*/

        private bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(storedSalt)))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return storedHash == Convert.ToBase64String(computedHash);
            }
        }

        // This action gets triggered when the user confirms logout
        public IActionResult Logout()
        {
            // Sign out the user (clear the authentication session)
            HttpContext.SignOutAsync(); // Assumes cookie authentication is being used

            // Redirect to the login page
            return RedirectToAction("Login", "Account");
        }


        [HttpGet]
        public IActionResult Registration()
        {
            // Registration is disabled
            TempData["Error"] = "Registration is not allowed.";
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Register()
        {
            // Registration functionality is disabled
            TempData["Error"] = "Registration is not allowed.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            TempData["Error"] = "Password recovery is not supported.";
            return RedirectToAction("Login");
        }
    }
}

/*
namespace Zoor_Lebanon_Booking_Platform.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        *//*  private readonly ZoorLebanonContext _context;
          private readonly JwtHelper _jwtHelper;*//*
        // Hardcoded admin credentials
        private readonly string adminFirstName = "Mariejose";
        private readonly string adminLastName = "Khalil";
        private readonly string adminEmail = "mariejosekhalil29@gmail.com";
        private readonly string adminPassword = "Mariejose.kh4";

      *//*  public AccountController(ZoorLebanonContext context, JwtHelper jwtHelper)
        {
            _context = context;
            _jwtHelper = jwtHelper;
        }
*//*
        [HttpGet]
        public IActionResult Login() => View();


        [HttpPost]
        *//*      public async Task<IActionResult> Login(string email, string password)
              {
                  var user = _context.Users.SingleOrDefault(u => u.Email == email && u.RoleId == 2); // Assuming RoleId = 2 is for Admin

                  if (user == null || !PasswordHelper.VerifyPassword(password, user.PasswordHash, user.Salt))
                  {
                      ModelState.AddModelError("", "Invalid email or password");
                      return View();
                  }

                  // Generate JWT token for the admin user
                  var token = _jwtHelper.GenerateJwtToken(user);

                  // Add the token to the response headers
                  HttpContext.Response.Headers.Add("Authorization", "Bearer " + token);

                  return RedirectToAction("Dashboard", "Home");
              }*//*

        // Login POST action to handle login submissions
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            // Validate email and password against hardcoded values
            if (email == adminEmail && password == adminPassword)
            {
                // Redirect to the Admin Home page upon successful login
                return Redirect("~/admin/home/index2");
            }

            // If invalid, return an error message and reload the login page
            ViewBag.Error = "Invalid email or password.";
            return View();
        }

        // Registration GET action (redirect to login since registration is not allowed)
        [HttpGet]
        public IActionResult Registration()
        {
            return RedirectToAction("Login");
        }

        *//* [HttpGet]
         public IActionResult Registration() => View();*/

/* [HttpPost]
 [ValidateAntiForgeryToken]
 public async Task<IActionResult> Registration(User user, string password)
 {
     if (_context.Users.Any(u => u.Email == user.Email))
     {
         ModelState.AddModelError("Email", "Email already in use");
         return View(user);
     }

     var (hash, salt) = PasswordHelper.HashPassword(password);
     user.PasswordHash = hash;
     user.Salt = salt;
     user.RoleId = 2; // Assuming RoleId = 2 is for Admin
     _context.Users.Add(user);
     await _context.SaveChangesAsync();

     return RedirectToAction("Login");
 }*//*

// Registration POST action (disable functionality)
[HttpPost]
public IActionResult Register()
{
    // Registration functionality is disabled
    TempData["Error"] = "Registration is not allowed.";
    return RedirectToAction("Login");
}

// Forgot Password GET action (redirect to login since password recovery is not supported)
[HttpGet]
public IActionResult ForgotPassword()
{
    TempData["Error"] = "Password recovery is not supported.";
    return RedirectToAction("Login");
}

*//*
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = _context.Users.SingleOrDefault(u => u.Email == email && u.RoleId == 2); // Only Admin users

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
        }*//*
}
}
*/















