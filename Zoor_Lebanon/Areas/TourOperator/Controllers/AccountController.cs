using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
/*using Zoor_Lebanon_Booking_Platform.Models.Database;
using Zoor_Lebanon_Booking_Platform.Models.Helper;*/

namespace Zoor_Lebanon_Booking_Platform.Areas.TourOperator.Controllers
{
    [Area("TourOperator")]
    public class AccountController : Controller
    {
        //private readonly DbContex _context;
        //private readonly JwtHelper _jwtHelper;

       // public AccountController(ZoorLebanonContext context, JwtHelper jwtHelper)
        //{
          //  _context = context;
           // _jwtHelper = jwtHelper;
        //}

        // GET: Login Page
        [HttpGet]
        public IActionResult Login() => View();

        // POST: Handle Login Form Submission
        [HttpPost]
        public async Task<IActionResult> Login(string companyName, string businessPhone)
        {
            // Validate the inputs
            if (string.IsNullOrEmpty(companyName) || string.IsNullOrEmpty(businessPhone))
            {
                ModelState.AddModelError("", "Company name and business phone are required.");
                return View();
            }

            // Check if the tour operator exists in the database
            /*var tourOperator = await _context.TourOperators
                .Include(t => t.User) // Include related User entity
                .SingleOrDefaultAsync(t => t.CompanyName == companyName && t.BusinessPhone == businessPhone);*/

            /*if (tourOperator == null)
            {
                ModelState.AddModelError("", "Invalid company name or business phone.");
                return View();
            }*/

            // Generate a JWT token for the tour operator
            //var token = _jwtHelper.GenerateJwtToken(tourOperator.User);

            // Add the token to a secure cookie for session management
            HttpContext.Response.Cookies.Append("Authorization", "Bearer " , new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Ensure cookies are secure in production
                SameSite = SameSiteMode.Strict
            });

            // Redirect to the Dashboard in the TourOperator area
            return Redirect("~/TourOperator/TourOperator/Dashboard");

        }


        // GET: Forgot Password Page
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        // POST: Handle Forgot Password Request
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string companyName, string businessPhone)
        {
            // Validate the inputs
            if (string.IsNullOrEmpty(companyName) || string.IsNullOrEmpty(businessPhone))
            {
                ModelState.AddModelError("", "Company name and business phone are required.");
                return View();
            }

            // Find the tour operator by company name and business phone
            /*var tourOperator = await _context.TourOperators
                .Include(t => t.User) // Include related User entity
                .SingleOrDefaultAsync(t => t.CompanyName == companyName && t.BusinessPhone == businessPhone);*/

           /* if (tourOperator == null)
            {
                ModelState.AddModelError("", "No matching company name and business phone found.");
                return View();
            }

            // Generate a JWT token for password reset with a short expiration
            var resetToken = _jwtHelper.GenerateJwtToken(tourOperator.User, expireMinutes: 15);
           */

            // Create a reset link with the token
            //var resetLink = Url.Action("ResetPassword", "Account", new { token = resetToken, email = tourOperator.User.Email }, Request.Scheme);

            // Assuming you have an email service, you would send the reset link like this:
            // await _emailService.SendPasswordResetEmail(tourOperator.User.Email, resetLink);

            return View("ForgotPasswordConfirmation");
        }
    }
}
