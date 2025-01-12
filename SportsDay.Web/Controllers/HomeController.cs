using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SportsDay.Web.Models;

namespace SportsDay.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult AnnouncementsPartial()
    {
        return PartialView("_AnnouncementsPartial");
    }

    public IActionResult LeaderboardPartial()
    {
        return PartialView("_LeaderboardPartial");
    }

    public IActionResult UpdateStreamPartial()
    {
        return PartialView("_UpdateStreamPartial");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
