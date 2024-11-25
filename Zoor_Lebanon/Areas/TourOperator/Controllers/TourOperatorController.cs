using Microsoft.AspNetCore.Mvc;

namespace Zoor_Lebanon_Booking_Platform.Areas.TourOperator.Controllers
{
    [Area("TourOperator")]
    public class TourOperatorController : Controller
    {
        // POST: Handle "Send for Review" submission
        [HttpPost]
        public IActionResult SendForReview(string packageName, string description, int locationId, decimal unitPrice, int availableSpots, int totalSpots, DateTime startDate, DateTime endDate, decimal? averageDuration)
        {
            // Placeholder logic to send a notification to the admin
            // In the future, you'll use a notification service to alert the Admin

            TempData["Message"] = $"Package '{packageName}' has been sent for review. The admin will confirm it soon.";
            return RedirectToAction("ManagePackages");
        }

        // GET: Manage Packages (placeholder)
        public IActionResult ManagePackages()
        {
            // Logic to display all packages (confirmed/unconfirmed) will be added here
            return View();
        }
        public IActionResult Dashboard()
        {
            // Render the dashboard page
            return View();
        }

    }
}
