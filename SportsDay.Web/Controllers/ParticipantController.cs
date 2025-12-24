using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportsDay.Lib.Data;
using SportsDay.Lib.Enums;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Web.Controllers;

[Authorize]
public class ParticipantController : Controller
{
    private readonly IParticipantService _participantService;
    private readonly IHouseLeaderService _houseLeaderService;
    private readonly SportsDayDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<ParticipantController> _logger;

    public ParticipantController(
        IParticipantService participantService,
        IHouseLeaderService houseLeaderService,
        SportsDayDbContext context,
        UserManager<IdentityUser> userManager,
        ILogger<ParticipantController> logger)
    {
        _participantService = participantService;
        _houseLeaderService = houseLeaderService;
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    // GET: Participant/Register
    public async Task<IActionResult> Register()
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        // Check if user is a house leader
        var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
        if (houseLeader == null)
        {
            TempData["Error"] = "You must be a house leader to register participants.";
            return RedirectToAction("Register", "HouseLeader");
        }

        // Get active tournament
        var activeTournament = await _context.Tournaments
            .FirstOrDefaultAsync(t => t.IsActive);

        if (activeTournament == null)
        {
            TempData["Error"] = "No active tournament at this time. Please wait for a tournament to be activated.";
            return RedirectToAction("Dashboard", "HouseLeader");
        }

        ViewBag.HouseLeader = houseLeader;
        ViewBag.ActiveTournament = activeTournament;
        ViewBag.GenderGroups = new SelectList(Enum.GetValues(typeof(DivisionType)));
        ViewBag.EventClasses = new SelectList(Enum.GetValues(typeof(EventClass)));

        return View(new Participant { HouseId = houseLeader.HouseId, TournamentId = activeTournament.Id });
    }

    // POST: Participant/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(Participant participant)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        try
        {
            // Verify user is a house leader
            var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
            if (houseLeader == null)
            {
                TempData["Error"] = "You must be a house leader to register participants.";
                return RedirectToAction("Register", "HouseLeader");
            }

            // Verify participant is for the house leader's house
            if (participant.HouseId != houseLeader.HouseId)
            {
                TempData["Error"] = "You can only register participants for your own house.";
                return RedirectToAction(nameof(Register));
            }

            // Verify tournament is active
            var tournament = await _context.Tournaments.FindAsync(participant.TournamentId);
            if (tournament == null || !tournament.IsActive)
            {
                TempData["Error"] = "The tournament is not active.";
                return RedirectToAction(nameof(Register));
            }

            participant.CreatedBy = userId;
            await _participantService.CreateAsync(participant);

            TempData["Success"] = $"Participant {participant.FullName} has been successfully registered!";
            _logger.LogInformation("House leader {UserId} registered participant {ParticipantName} for house {HouseId}",
                userId, participant.FullName, participant.HouseId);

            return RedirectToAction("Dashboard", "HouseLeader");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering participant by user {UserId}", userId);
            TempData["Error"] = "An error occurred while registering the participant. Please try again.";

            var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
            var activeTournament = await _context.Tournaments.FirstOrDefaultAsync(t => t.IsActive);

            ViewBag.HouseLeader = houseLeader;
            ViewBag.ActiveTournament = activeTournament;
            ViewBag.GenderGroups = new SelectList(Enum.GetValues(typeof(DivisionType)));
            ViewBag.EventClasses = new SelectList(Enum.GetValues(typeof(EventClass)));

            return View(participant);
        }
    }

    // GET: Participant/Edit/5
    public async Task<IActionResult> Edit(Guid id)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
        if (houseLeader == null)
        {
            TempData["Error"] = "You must be a house leader to edit participants.";
            return RedirectToAction("Register", "HouseLeader");
        }

        var participant = await _participantService.GetByIdAsync(id);
        if (participant == null)
        {
            TempData["Error"] = "Participant not found.";
            return RedirectToAction("Dashboard", "HouseLeader");
        }

        // Verify participant belongs to house leader's house
        if (participant.HouseId != houseLeader.HouseId)
        {
            TempData["Error"] = "You can only edit participants from your own house.";
            return RedirectToAction("Dashboard", "HouseLeader");
        }

        ViewBag.HouseLeader = houseLeader;
        ViewBag.GenderGroups = new SelectList(Enum.GetValues(typeof(DivisionType)), participant.GenderGroup);
        ViewBag.EventClasses = new SelectList(Enum.GetValues(typeof(EventClass)), participant.EventClassGroup);

        return View(participant);
    }

    // POST: Participant/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Participant participant)
    {
        if (id != participant.Id)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        try
        {
            var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
            if (houseLeader == null)
            {
                TempData["Error"] = "You must be a house leader to edit participants.";
                return RedirectToAction("Register", "HouseLeader");
            }

            // Verify participant belongs to house leader's house
            if (participant.HouseId != houseLeader.HouseId)
            {
                TempData["Error"] = "You can only edit participants from your own house.";
                return RedirectToAction("Dashboard", "HouseLeader");
            }

            participant.UpdatedBy = userId;
            await _participantService.UpdateAsync(participant);

            TempData["Success"] = $"Participant {participant.FullName} has been successfully updated!";
            _logger.LogInformation("House leader {UserId} updated participant {ParticipantId}", userId, id);

            return RedirectToAction("Dashboard", "HouseLeader");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating participant {ParticipantId} by user {UserId}", id, userId);
            TempData["Error"] = "An error occurred while updating the participant. Please try again.";

            var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
            ViewBag.HouseLeader = houseLeader;
            ViewBag.GenderGroups = new SelectList(Enum.GetValues(typeof(DivisionType)), participant.GenderGroup);
            ViewBag.EventClasses = new SelectList(Enum.GetValues(typeof(EventClass)), participant.EventClassGroup);

            return View(participant);
        }
    }

    // GET: Participant/Delete/5
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
        if (houseLeader == null)
        {
            TempData["Error"] = "You must be a house leader to delete participants.";
            return RedirectToAction("Register", "HouseLeader");
        }

        var participant = await _participantService.GetByIdAsync(id);
        if (participant == null)
        {
            TempData["Error"] = "Participant not found.";
            return RedirectToAction("Dashboard", "HouseLeader");
        }

        // Verify participant belongs to house leader's house
        if (participant.HouseId != houseLeader.HouseId)
        {
            TempData["Error"] = "You can only delete participants from your own house.";
            return RedirectToAction("Dashboard", "HouseLeader");
        }

        ViewBag.HouseLeader = houseLeader;
        return View(participant);
    }

    // POST: Participant/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var userId = _userManager.GetUserId(User);
        if (userId == null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        try
        {
            var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
            if (houseLeader == null)
            {
                TempData["Error"] = "You must be a house leader to delete participants.";
                return RedirectToAction("Register", "HouseLeader");
            }

            var participant = await _participantService.GetByIdAsync(id);
            if (participant == null)
            {
                TempData["Error"] = "Participant not found.";
                return RedirectToAction("Dashboard", "HouseLeader");
            }

            // Verify participant belongs to house leader's house
            if (participant.HouseId != houseLeader.HouseId)
            {
                TempData["Error"] = "You can only delete participants from your own house.";
                return RedirectToAction("Dashboard", "HouseLeader");
            }

            var participantName = participant.FullName;
            await _participantService.DeleteAsync(id);

            TempData["Success"] = $"Participant {participantName} has been successfully deleted.";
            _logger.LogInformation("House leader {UserId} deleted participant {ParticipantId}", userId, id);

            return RedirectToAction("Dashboard", "HouseLeader");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting participant {ParticipantId} by user {UserId}", id, userId);
            TempData["Error"] = "An error occurred while deleting the participant. Please try again.";
            return RedirectToAction("Dashboard", "HouseLeader");
        }
    }
}