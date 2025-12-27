using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using SportsDay.Lib.Data;
using SportsDay.Lib.Enums;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;
using SportsDay.Lib.ViewModels;
using SportsDay.Web.Filters;
using System.Security.Claims;

namespace SportsDay.Web.Areas.HouseLeader.Controllers;

/// <summary>
/// Main dashboard controller for House Leaders
/// Provides functionality for managing house participants and event registrations
/// </summary>
[ServiceFilter(typeof(HouseLeaderHouseFilter))]
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

    #region House Selection

    /// <summary>
    /// GET: Select a house to manage (for users who are leaders of multiple houses or administrators)
    /// </summary>
    public async Task<IActionResult> Select()
    {
        var userId = _userManager.GetUserId(User);
        /* if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        } */
        var isAdmin = User.IsInRole("Administrator");
        var isHouseLeader = User.IsInRole("HouseLeader");

        // Get the house leader record for this user
        var houseIds = await _houseLeaderService.GetHousesByUserIdAsync(userId);

        if (houseIds.Count() == 0 && !isAdmin && !isHouseLeader)
        {
            TempData["Error"] = "You are not registered as a house leader.";
            return RedirectToAction(nameof(NotAllowed));
        }
        if (houseIds.Count() == 1 && !isAdmin)
        {
            SelectSingleHouseClaim(houseIds.First());
            return RedirectToAction(nameof(Index));
        }
        IEnumerable<House> availableHouses;
        if (isAdmin)
        {
            //all houses
            availableHouses = await _houseService.GetAllAsync();
        }
        else if (houseIds.Any())
        {
            availableHouses = await _houseService.GetAllAsync();
            availableHouses = availableHouses.Where(h => houseIds.Contains(h.Id)).ToList();
        }
        else
        {
            TempData["Error"] = "You are not registered as a house leader.";
            return RedirectToAction(nameof(Register));
        }

        // If user is an administrator, they can view all houses

        ViewBag.Houses = new SelectList(availableHouses, "Id", "Name");
        ViewBag.IsAdmin = isAdmin;

        return View();
    }

    private async void SelectSingleHouseClaim(int houseId)
    {
        // Add HouseLeaderHouseId claim
        var user = await _userManager.GetUserAsync(User);
        if (user != null)
        {
            var existingClaim = (await _userManager.GetClaimsAsync(user))
                .FirstOrDefault(c => c.Type == "HouseLeaderHouseId");

            if (existingClaim != null)
            {
                await _userManager.RemoveClaimAsync(user, existingClaim);
            }

            await _userManager.AddClaimAsync(user, new Claim("HouseLeaderHouseId", houseId.ToString()));
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "HouseLeader"));

            // Refresh the sign-in to update claims
            await _userManager.UpdateSecurityStampAsync(user);
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

            // Create new claims principal with updated claims
            var claims = await _userManager.GetClaimsAsync(user);
            var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

            _logger.LogInformation("selected house {HouseId}", houseId);
        }
    }

    /// <summary>
    /// POST: Select a house and write HouseLeaderHouseId claim
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Select(int houseId)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        try
        {
            // Verify house exists
            var house = await _houseService.GetByIdAsync(houseId);
            if (house == null)
            {
                TempData["Error"] = "Selected house not found.";
                return RedirectToAction(nameof(Select));
            }

            // Verify user is a house leader for this house
            var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
            if (houseLeader == null || houseLeader.HouseId != houseId)
            {
                TempData["Error"] = "You are not authorized to manage this house.";
                return RedirectToAction(nameof(Select));
            }

            // Add HouseLeaderHouseId claim
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var existingClaim = (await _userManager.GetClaimsAsync(user))
                    .FirstOrDefault(c => c.Type == "HouseLeaderHouseId");

                if (existingClaim != null)
                {
                    await _userManager.RemoveClaimAsync(user, existingClaim);
                }

                await _userManager.AddClaimAsync(user, new Claim("HouseLeaderHouseId", houseId.ToString()));
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "HouseLeader"));
                // Refresh the sign-in to update claims
                await _userManager.UpdateSecurityStampAsync(user);
                await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

                // Create new claims principal with updated claims
                var claims = await _userManager.GetClaimsAsync(user);
                var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

                TempData["Success"] = $"You are now managing {house.Name}!";
                _logger.LogInformation("User {UserId} selected house {HouseName} (ID: {HouseId})", userId, house.Name, houseId);
            }

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting house for user {UserId}", userId);
            TempData["Error"] = "An error occurred while selecting the house. Please try again.";

            ViewBag.Houses = new SelectList(
                await _houseService.GetAllAsync(),
                "Id",
                "Name"
            );

            return View();
        }
    }

    /// <summary>
    /// GET: Not allowed page for users who are not house leaders
    /// </summary>
    public IActionResult NotAllowed()
    {
        return View();
    }

    #endregion

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

            return RedirectToAction(nameof(Select));
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