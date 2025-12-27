using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SportsDay.Lib.Services.Interfaces;
using System.Security.Claims;

namespace SportsDay.Web.Filters;

/// <summary>
/// Filter to ensure house leaders have selected a house before accessing house leader area
/// </summary>
public class HouseLeaderHouseFilter : IAsyncActionFilter
{
    private readonly IHouseLeaderService _houseLeaderService;
    private readonly ILogger<HouseLeaderHouseFilter> _logger;

    public HouseLeaderHouseFilter(
        IHouseLeaderService houseLeaderService,
        ILogger<HouseLeaderHouseFilter> logger)
    {
        _houseLeaderService = houseLeaderService;
        _logger = logger;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Get action and controller names
        var actionName = context.RouteData.Values["action"]?.ToString();
        var controllerName = context.RouteData.Values["controller"]?.ToString();

        // Skip filter for specific actions that don't require house selection
        var exemptActions = new[] { "Select", "NotAllowed", "Register", "Unregister" };
        if (controllerName == "Dashboard" && exemptActions.Contains(actionName, StringComparer.OrdinalIgnoreCase))
        {
            await next();
            return;
        }

        var user = context.HttpContext.User;
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("User ID not found in claims");
            context.Result = new RedirectToActionResult("Login", "Account", new { area = "" });
            return;
        } 

        // Check if HouseLeaderHouseId claim exists
        var houseIdClaim = user.FindFirstValue("HouseLeaderHouseId");

        if (!string.IsNullOrEmpty(houseIdClaim))
        {
            // Claim exists, proceed with the action
            await next();
            return;
        }

        // No claim found, check if user is an administrator or has house leader records
        var isAdmin = user.IsInRole("Administrator");
        var isHouseLeader = user.IsInRole("HouseLeader");
        var houseLeaders = await _houseLeaderService.GetByUserIdAsync(userId);

        if (houseLeaders == null && !isAdmin && !isHouseLeader)
        {
            // User is not a house leader at all
            _logger.LogWarning("User {UserId} attempted to access house leader area without being a house leader", userId);
            context.Result = new RedirectToActionResult("NotAllowed", "Dashboard", new { area = "HouseLeader" });
            return;
        }

        // User is a house leader, redirect to Select action
        _logger.LogInformation("User {UserId} needs to select a house", userId);
        context.Result = new RedirectToActionResult("Select", "Dashboard", new { area = "HouseLeader" });
    }
}