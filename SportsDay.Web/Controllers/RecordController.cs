using Microsoft.AspNetCore.Mvc;

namespace SportsDay.Web.Controllers
{
    public class RecordController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
