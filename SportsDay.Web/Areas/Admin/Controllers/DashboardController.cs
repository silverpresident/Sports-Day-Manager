using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Web.Areas.Admin.Controllers;

[Area("Admin")]
//[Authorize(Roles = "Administrator")]
public class DashboardController : Controller
{
    private readonly IDashboardService _dashboardService;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IDashboardService dashboardService,
        ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            _logger.LogInformation("Loading admin dashboard");
            var viewModel = await _dashboardService.GetAdminDashboardAsync();
            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin dashboard");
            TempData["Error"] = "An error occurred while loading the dashboard.";
            return View(new SportsDay.Lib.ViewModels.AdminDashboardViewModel());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RefreshCache()
    {
        _logger.LogInformation("Manually invalidating dashboard cache");
        _dashboardService.InvalidateCache();
        TempData["Success"] = "Dashboard cache has been refreshed.";
        return RedirectToAction(nameof(Index));
    }
}
