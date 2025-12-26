using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsDay.Lib.Data;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Web.Controllers;

//TODO [Authorize] Role=HouseLeader
public class XHouseLeaderController : Controller
{
    private readonly IHouseLeaderService _houseLeaderService;
    private readonly SportsDayDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<HouseLeaderController> _logger;

    public HouseLeaderController(
        IHouseLeaderService houseLeaderService,
        SportsDayDbContext context,
        UserManager<IdentityUser> userManager,
        ILogger<HouseLeaderController> logger)
    {
        _houseLeaderService = houseLeaderService;
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    // GET: HouseLeader/Register
    public async Task<IActionResult> Register()
    {
        var userId = _userManager.GetUserId(User);
        /* if (userId == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        } */

        // Check if user is already a house leader
        var existingLeader = await _houseLeaderService.GetByUserIdAsync(userId);
        if (existingLeader != null)
        {
            TempData["Info"] = $"You are already registered as a house leader for {existingLeader.House?.Name}.";
            return RedirectToAction(nameof(Dashboard));
        }

        ViewBag.Houses = new SelectList(
            await _context.Houses.OrderBy(h => h.Name).ToListAsync(),
            "Id",
            "Name"
        );

        return View();
    }

    // POST: HouseLeader/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(int houseId)
    {
        var userId = _userManager.GetUserId(User);
        /* if (userId == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        } */

        try
        {
            // Check if user is already a house leader
            var existingLeader = await _houseLeaderService.GetByUserIdAsync(userId);
            if (existingLeader != null)
            {
                TempData["Error"] = $"You are already registered as a house leader for {existingLeader.House?.Name}.";
                return RedirectToAction(nameof(Dashboard));
            }

            // Verify house exists
            var house = await _context.Houses.FindAsync(houseId);
            if (house == null)
            {
                TempData["Error"] = "Selected house not found.";
                return RedirectToAction(nameof(Register));
            }

            var houseLeader = new HouseLeader
            {
                HouseId = houseId,
                UserId = userId,
                CreatedBy = userId
            };

            await _houseLeaderService.CreateAsync(houseLeader);

            TempData["Success"] = $"You have successfully registered as a house leader for {house.Name}!";
            _logger.LogInformation("User {UserId} registered as house leader for {HouseName}", userId, house.Name);

            return RedirectToAction(nameof(Dashboard));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user {UserId} as house leader", userId);
            TempData["Error"] = "An error occurred while registering. Please try again.";

            ViewBag.Houses = new SelectList(
                await _context.Houses.OrderBy(h => h.Name).ToListAsync(),
                "Id",
                "Name"
            );

            return View();
        }
    }

    // GET: HouseLeader/Dashboard
    public async Task<IActionResult> Dashboard()
    {
        var userId = _userManager.GetUserId(User);
        /* if (userId == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        } */

        var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
        if (houseLeader == null)
        {
            TempData["Info"] = "You are not registered as a house leader. Please register first.";
            return RedirectToAction(nameof(Register));
        }

        // Get active tournament
        var activeTournament = await _context.Tournaments
            .FirstOrDefaultAsync(t => t.IsActive);

        if (activeTournament == null)
        {
            ViewBag.Message = "No active tournament at this time.";
            ViewBag.HouseLeader = houseLeader;
            return View();
        }

        // Get participants for this house in the active tournament
        var participants = await _context.Participants
            .Include(p => p.House)
            .Include(p => p.Results)
            .Where(p => p.HouseId == houseLeader.HouseId && p.TournamentId == activeTournament.Id)
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();

        ViewBag.HouseLeader = houseLeader;
        ViewBag.ActiveTournament = activeTournament;
        ViewBag.Participants = participants;

        return View();
    }

    // GET: HouseLeader/Unregister
    public async Task<IActionResult> Unregister()
    {
        var userId = _userManager.GetUserId(User);
        /* if (userId == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        } */

        var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
        if (houseLeader == null)
        {
            TempData["Error"] = "You are not registered as a house leader.";
            return RedirectToAction("Index", "Dashboard");
        }

        return View(houseLeader);
    }

    // POST: HouseLeader/Unregister
    [HttpPost, ActionName("Unregister")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnregisterConfirmed()
    {
        var userId = _userManager.GetUserId(User);
        /* if (userId == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        } */

        try
        {
            var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
            if (houseLeader == null)
            {
                TempData["Error"] = "You are not registered as a house leader.";
                return RedirectToAction("Index", "Dashboard");
            }

            var houseName = houseLeader.House?.Name;
            await _houseLeaderService.DeleteAsync(houseLeader.Id);

            TempData["Success"] = $"You have been unregistered as house leader for {houseName}.";
            _logger.LogInformation("User {UserId} unregistered as house leader", userId);

            return RedirectToAction("Index", "Dashboard");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unregistering user {UserId} as house leader", userId);
            TempData["Error"] = "An error occurred while unregistering. Please try again.";
            return RedirectToAction(nameof(Dashboard));
        }
    }
}