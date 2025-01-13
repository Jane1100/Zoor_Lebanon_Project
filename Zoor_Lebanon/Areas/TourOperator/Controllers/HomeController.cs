using Microsoft.AspNetCore.Mvc;
using Zoor_Lebanon.Models.ViewModels;


namespace Zoor_Lebanon.Areas.TourOperator.Controllers
{
    [Area("TourOperator")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            Console.WriteLine("HomeController > Index route accessed");
            return View();
        }

    }
}
