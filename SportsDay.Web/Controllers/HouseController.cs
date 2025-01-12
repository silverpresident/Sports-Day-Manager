using Microsoft.AspNetCore.Mvc;

namespace SportsDay.Web.Controllers
{
    public class HouseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Participants(int id)
        {
            return View();
        }
    }
}
