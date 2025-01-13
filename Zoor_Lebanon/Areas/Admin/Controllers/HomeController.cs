using Microsoft.AspNetCore.Mvc;
using Zoor_Lebanon.Models.ViewModels;

namespace Zoor_Lebanon.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
/*        public IActionResult Index2()
        {
            return View();
        }
*/
    /*    [HttpGet]
        public IActionResult Index2()
        {
            return View();
        }*/
      
        public IActionResult compose()
        {
            return View();
        }
       /* public IActionResult readmail()
        {
            return View();
        }*/
     /*   public IActionResult Calendar()
        {
            return View();
        }*/
    
   /*     public IActionResult gallery()
        {
            return View();
        }
        public IActionResult iframe()
        {
            return View();
        }
     
        public IActionResult starter()
        {
            return View();
        }
        public IActionResult kanban()
        {
            return View();
        }
        public IActionResult invoice()
        {
            return View();
        }*/
    
   
     
      /*  public IActionResult loyalty()
        {
            return View();
        }*/
     
     
        public IActionResult flot()
        {
            return View();
        }
  /*      public IActionResult login()
        {
            return View();
        }*/
 
        public IActionResult uplot()
        {
            return View();
        }
        public IActionResult inline()
        {
            return View();
        }
    /*    public IActionResult Registration()
        {
            return View();
        }*/
     /*   public IActionResult addpackage()
        {
            return View();
        }*/
     /*   public IActionResult editpackages()
        {
            return View();
        }*/
        public IActionResult lockscreen()
        {
            return View();
        }
      

    }
}
