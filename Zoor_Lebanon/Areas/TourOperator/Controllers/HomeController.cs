using Microsoft.AspNetCore.Mvc;

namespace Zoor_Lebanon_Booking_Platform.Areas.TourOperator.Controllers
{
    [Area("TourOperator")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(); // This returns the Index.cshtml in the TourOperator area
        }
    }
}
