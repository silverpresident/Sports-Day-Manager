using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;
using SportsDay.Lib.ViewModels;
using SportsDay.Web.Models;

namespace SportsDay.Web.Controllers;

public class DashboardController : Controller
{
    private readonly ILogger<DashboardController> _logger;
    private readonly IDashboardService _dashboardService;

    public DashboardController(
        ILogger<DashboardController> logger,
        IDashboardService dashboardService)
    {
        _logger = logger;
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            _logger.LogInformation("Loading public dashboard");
            var viewModel = await _dashboardService.GetPublicDashboardAsync();
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading public dashboard");
            return View(new PublicDashboardViewModel());
        }
    }

    public async Task<IActionResult> GetAnnouncements()
    {
        try
        {
            var viewModel = await _dashboardService.GetPublicDashboardAsync();
            return PartialView("_AnnouncementsPartial", viewModel.Announcements);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading announcements");
            return PartialView("_AnnouncementsPartial", new List<Announcement>());
        }
    }

    public async Task<IActionResult> GetUpdates()
    {
        try
        {
            var viewModel = await _dashboardService.GetPublicDashboardAsync();
            return PartialView("_UpdateStreamPartial", viewModel.Updates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading updates");
            return PartialView("_UpdateStreamPartial", new List<EventUpdate>());
        }
    }

    public async Task<IActionResult> GetLeaderboard()
    {
        try
        {
            var viewModel = await _dashboardService.GetPublicDashboardAsync();
            return PartialView("_LeaderboardPartial", viewModel.Summaries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading leaderboard");
            return PartialView("_LeaderboardPartial", new List<TournamentHouseSummary>());
        }
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
