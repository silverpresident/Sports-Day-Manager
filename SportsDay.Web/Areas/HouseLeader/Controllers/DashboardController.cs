using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsDay.Lib.Data;
using SportsDay.Lib.Enums;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;
using SportsDay.Lib.ViewModels;

namespace SportsDay.Web.Areas.HouseLeader.Controllers;

/// <summary>
/// Main dashboard controller for House Leaders
/// Provides functionality for managing house participants and event registrations
/// </summary>
public class DashboardController : HouseLeaderBaseController
{
    private readonly IHouseLeaderService _houseLeaderService;
    private readonly IHouseService _houseService;
    private readonly IParticipantService _participantService;
    private readonly IEventService _eventService;
    private readonly ITournamentService _tournamentService;
    private readonly SportsDayDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IHouseLeaderService houseLeaderService,
        IHouseService houseService,
        IParticipantService participantService,
        IEventService eventService,
        ITournamentService tournamentService,
        SportsDayDbContext context,
        UserManager<IdentityUser> userManager,
        ILogger<DashboardController> logger)
    {
        _houseLeaderService = houseLeaderService;
        _houseService = houseService;
        _participantService = participantService;
        _eventService = eventService;
        _tournamentService = tournamentService;
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// Main dashboard showing house overview, participants, and quick stats
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        /* if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        } */

        var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
        if (houseLeader == null)
        {
            TempData["Info"] = "You are not registered as a house leader. Please register first.";
            return RedirectToAction(nameof(Register));
        }

        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        
        var viewModel = new HouseLeaderDashboardViewModel
        {
            HouseLeader = houseLeader,
            House = houseLeader.House!
        };

        if (activeTournament != null)
        {
            viewModel.ActiveTournament = activeTournament;
            viewModel.Participants = (await _participantService.GetByTournamentAndHouseAsync(
                activeTournament.Id, houseLeader.HouseId)).ToList();
            
            var events = await _eventService.GetByTournamentIdAsync(activeTournament.Id);
            viewModel.TotalEvents = events.Count();
            viewModel.EventsWithParticipants = await _context.Results
                .Where(r => r.TournamentId == activeTournament.Id && r.HouseId == houseLeader.HouseId)
                .Select(r => r.EventId)
                .Distinct()
                .CountAsync();
        }

        return View(viewModel);
    }

    #region House Leader Registration

    /// <summary>
    /// GET: Register as a house leader
    /// </summary>
    public async Task<IActionResult> Register()
    {
        var userId = _userManager.GetUserId(User);
       /*  if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        } */

        // Check if user is already a house leader
        var existingLeader = await _houseLeaderService.GetByUserIdAsync(userId);
        if (existingLeader != null)
        {
            TempData["Info"] = $"You are already registered as a house leader for {existingLeader.House?.Name}.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.Houses = new SelectList(
            await _houseService.GetAllAsync(),
            "Id",
            "Name"
        );

        return View();
    }

    /// <summary>
    /// POST: Register as a house leader
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(int houseId)
    {
        var userId = _userManager.GetUserId(User);
        /* if (string.IsNullOrEmpty(userId))
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
                return RedirectToAction(nameof(Index));
            }

            // Verify house exists
            var house = await _houseService.GetByIdAsync(houseId);
            if (house == null)
            {
                TempData["Error"] = "Selected house not found.";
                return RedirectToAction(nameof(Register));
            }

            var houseLeader = new Lib.Models.HouseLeader
            {
                HouseId = houseId,
                UserId = userId,
                CreatedBy = userId
            };

            await _houseLeaderService.CreateAsync(houseLeader);

            TempData["Success"] = $"You have successfully registered as a house leader for {house.Name}!";
            _logger.LogInformation("User {UserId} registered as house leader for {HouseName}", userId, house.Name);

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user {UserId} as house leader", userId);
            TempData["Error"] = "An error occurred while registering. Please try again.";

            ViewBag.Houses = new SelectList(
                await _houseService.GetAllAsync(),
                "Id",
                "Name"
            );

            return View();
        }
    }

    /// <summary>
    /// GET: Unregister as a house leader
    /// </summary>
    public async Task<IActionResult> Unregister()
    {
        var userId = _userManager.GetUserId(User);
        /* if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        } */

        var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
        if (houseLeader == null)
        {
            TempData["Error"] = "You are not registered as a house leader.";
            return RedirectToAction("Index", "Dashboard", new { area = "" });
        }

        return View(houseLeader);
    }

    /// <summary>
    /// POST: Confirm unregistration as a house leader
    /// </summary>
    [HttpPost, ActionName("Unregister")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnregisterConfirmed()
    {
        var userId = _userManager.GetUserId(User);
        /* if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        } */

        try
        {
            var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
            if (houseLeader == null)
            {
                TempData["Error"] = "You are not registered as a house leader.";
                return RedirectToAction("Index", "Dashboard", new { area = "" });
            }

            var houseName = houseLeader.House?.Name;
            await _houseLeaderService.DeleteAsync(houseLeader.Id);

            TempData["Success"] = $"You have been unregistered as house leader for {houseName}.";
            _logger.LogInformation("User {UserId} unregistered as house leader", userId);

            return RedirectToAction("Index", "Dashboard", new { area = "" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unregistering user {UserId} as house leader", userId);
            TempData["Error"] = "An error occurred while unregistering. Please try again.";
            return RedirectToAction(nameof(Index));
        }
    }

    #endregion
}