using Microsoft.AspNetCore.Mvc;
using Zoor_Lebanon.Models;
using Zoor_Lebanon.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Zoor_Lebanon.Models.Helper;
using Microsoft.AspNetCore.Authentication;



namespace Zoor_Lebanon.Areas.TourOperator.Controllers
{
    [Area("TourOperator")]
    public class AccountController : Controller
    {
        private readonly zoor_lebanonContext _context;
        public AccountController(zoor_lebanonContext context)
        {
            _context = context;
        }
       
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string companyName, string businessPhone)
        {
            var tourOperator = await _context.TourOperators
                .SingleOrDefaultAsync(t => t.CompanyName == companyName && t.BusinessPhone == businessPhone);

            if (tourOperator == null)
            {
                ModelState.AddModelError("", "Invalid company name or business phone");
                return View();
            }

            // Redirect to Dashboard in UserController
            return RedirectToAction("Dashboard", "TourOperator", new { area = "TourOperator" });
        }

        // This action gets triggered when the user confirms logout
        public IActionResult Logout()
        {
            // Sign out the user (clear the authentication session)
            HttpContext.SignOutAsync(); // Assumes cookie authentication is being used

            // Redirect to the login page
            return RedirectToAction("Login", "Account");
        }




    }
}
