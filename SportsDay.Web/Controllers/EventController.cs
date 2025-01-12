using Microsoft.AspNetCore.Mvc;

namespace SportsDay.Web.Controllers
{
    public class EventController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            return View();
        }
    }
}
