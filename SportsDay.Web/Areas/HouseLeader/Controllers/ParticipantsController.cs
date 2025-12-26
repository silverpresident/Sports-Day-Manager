using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportsDay.Lib.Data;
using SportsDay.Lib.Enums;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;
using SportsDay.Lib.ViewModels;

namespace SportsDay.Web.Areas.HouseLeader.Controllers;

/// <summary>
/// Controller for managing house participants
/// </summary>
public class ParticipantsController : HouseLeaderBaseController
{
    private readonly IHouseLeaderService _houseLeaderService;
    private readonly IParticipantService _participantService;
    private readonly ITournamentService _tournamentService;
    private readonly SportsDayDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<ParticipantsController> _logger;

    public ParticipantsController(
        IHouseLeaderService houseLeaderService,
        IParticipantService participantService,
        ITournamentService tournamentService,
        SportsDayDbContext context,
        UserManager<IdentityUser> userManager,
        ILogger<ParticipantsController> logger)
    {
        _houseLeaderService = houseLeaderService;
        _participantService = participantService;
        _tournamentService = tournamentService;
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// List all participants for the house leader's house
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
        if (houseLeader == null)
        {
            TempData["Error"] = "You must be a house leader to view participants.";
            return RedirectToAction("Register", "Dashboard");
        }

        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            TempData["Warning"] = "No active tournament at this time.";
            return RedirectToAction("Index", "Dashboard");
        }

        var participants = await _participantService.GetByTournamentAndHouseAsync(
            activeTournament.Id, houseLeader.HouseId);

        var viewModel = new HouseLeaderDashboardViewModel
        {
            HouseLeader = houseLeader,
            House = houseLeader.House!,
            ActiveTournament = activeTournament,
            Participants = participants.OrderBy(p => p.LastName).ThenBy(p => p.FirstName).ToList()
        };

        return View(viewModel);
    }

    /// <summary>
    /// GET: Add a new participant
    /// </summary>
    public async Task<IActionResult> Add()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
        if (houseLeader == null)
        {
            TempData["Error"] = "You must be a house leader to add participants.";
            return RedirectToAction("Register", "Dashboard");
        }

        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            TempData["Error"] = "No active tournament at this time. Please wait for a tournament to be activated.";
            return RedirectToAction("Index", "Dashboard");
        }

        var viewModel = new AddParticipantViewModel
        {
            HouseLeader = houseLeader,
            House = houseLeader.House!,
            ActiveTournament = activeTournament,
            Participant = new Participant 
            { 
                HouseId = houseLeader.HouseId, 
                TournamentId = activeTournament.Id 
            }
        };

        SetupParticipantViewBag();
        return View(viewModel);
    }

    /// <summary>
    /// POST: Add a new participant
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(AddParticipantViewModel viewModel)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        try
        {
            var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
            if (houseLeader == null)
            {
                TempData["Error"] = "You must be a house leader to add participants.";
                return RedirectToAction("Register", "Dashboard");
            }

            var activeTournament = await _tournamentService.GetActiveTournamentAsync();
            if (activeTournament == null)
            {
                TempData["Error"] = "No active tournament at this time.";
                return RedirectToAction("Index", "Dashboard");
            }

            // Ensure participant is for the house leader's house
            viewModel.Participant.HouseId = houseLeader.HouseId;
            viewModel.Participant.TournamentId = activeTournament.Id;
            viewModel.Participant.CreatedBy = userId;

            // Calculate age
            var today = DateTime.Today;
            var age = today.Year - viewModel.Participant.DateOfBirth.Year;
            if (viewModel.Participant.DateOfBirth.Date > today.AddYears(-age)) age--;
            viewModel.Participant.AgeInYears = age;

            await _participantService.CreateAsync(viewModel.Participant);

            TempData["Success"] = $"Participant {viewModel.Participant.FullName} has been successfully added!";
            _logger.LogInformation("House leader {UserId} added participant {ParticipantName} for house {HouseId}",
                userId, viewModel.Participant.FullName, houseLeader.HouseId);

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding participant by user {UserId}", userId);
            TempData["Error"] = "An error occurred while adding the participant. Please try again.";

            var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
            var activeTournament = await _tournamentService.GetActiveTournamentAsync();

            viewModel.HouseLeader = houseLeader!;
            viewModel.House = houseLeader!.House!;
            viewModel.ActiveTournament = activeTournament!;

            SetupParticipantViewBag();
            return View(viewModel);
        }
    }

    /// <summary>
    /// GET: Edit a participant
    /// </summary>
    public async Task<IActionResult> Edit(Guid id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
        if (houseLeader == null)
        {
            TempData["Error"] = "You must be a house leader to edit participants.";
            return RedirectToAction("Register", "Dashboard");
        }

        var participant = await _participantService.GetByIdAsync(id);
        if (participant == null)
        {
            TempData["Error"] = "Participant not found.";
            return RedirectToAction(nameof(Index));
        }

        // Verify participant belongs to house leader's house
        if (participant.HouseId != houseLeader.HouseId)
        {
            TempData["Error"] = "You can only edit participants from your own house.";
            return RedirectToAction(nameof(Index));
        }

        var activeTournament = await _tournamentService.GetActiveTournamentAsync();

        var viewModel = new AddParticipantViewModel
        {
            HouseLeader = houseLeader,
            House = houseLeader.House!,
            ActiveTournament = activeTournament!,
            Participant = participant
        };

        SetupParticipantViewBag();
        return View(viewModel);
    }

    /// <summary>
    /// POST: Edit a participant
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, AddParticipantViewModel viewModel)
    {
        if (id != viewModel.Participant.Id)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        try
        {
            var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
            if (houseLeader == null)
            {
                TempData["Error"] = "You must be a house leader to edit participants.";
                return RedirectToAction("Register", "Dashboard");
            }

            // Verify participant belongs to house leader's house
            if (viewModel.Participant.HouseId != houseLeader.HouseId)
            {
                TempData["Error"] = "You can only edit participants from your own house.";
                return RedirectToAction(nameof(Index));
            }

            // Calculate age
            var today = DateTime.Today;
            var age = today.Year - viewModel.Participant.DateOfBirth.Year;
            if (viewModel.Participant.DateOfBirth.Date > today.AddYears(-age)) age--;
            viewModel.Participant.AgeInYears = age;

            viewModel.Participant.UpdatedBy = userId;
            await _participantService.UpdateAsync(viewModel.Participant);

            TempData["Success"] = $"Participant {viewModel.Participant.FullName} has been successfully updated!";
            _logger.LogInformation("House leader {UserId} updated participant {ParticipantId}", userId, id);

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating participant {ParticipantId} by user {UserId}", id, userId);
            TempData["Error"] = "An error occurred while updating the participant. Please try again.";

            var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
            var activeTournament = await _tournamentService.GetActiveTournamentAsync();

            viewModel.HouseLeader = houseLeader!;
            viewModel.House = houseLeader!.House!;
            viewModel.ActiveTournament = activeTournament!;

            SetupParticipantViewBag();
            return View(viewModel);
        }
    }

    /// <summary>
    /// GET: Delete a participant
    /// </summary>
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
        if (houseLeader == null)
        {
            TempData["Error"] = "You must be a house leader to delete participants.";
            return RedirectToAction("Register", "Dashboard");
        }

        var participant = await _participantService.GetByIdAsync(id);
        if (participant == null)
        {
            TempData["Error"] = "Participant not found.";
            return RedirectToAction(nameof(Index));
        }

        // Verify participant belongs to house leader's house
        if (participant.HouseId != houseLeader.HouseId)
        {
            TempData["Error"] = "You can only delete participants from your own house.";
            return RedirectToAction(nameof(Index));
        }

        ViewBag.HouseLeader = houseLeader;
        return View(participant);
    }

    /// <summary>
    /// POST: Confirm delete a participant
    /// </summary>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }

        try
        {
            var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
            if (houseLeader == null)
            {
                TempData["Error"] = "You must be a house leader to delete participants.";
                return RedirectToAction("Register", "Dashboard");
            }

            var participant = await _participantService.GetByIdAsync(id);
            if (participant == null)
            {
                TempData["Error"] = "Participant not found.";
                return RedirectToAction(nameof(Index));
            }

            // Verify participant belongs to house leader's house
            if (participant.HouseId != houseLeader.HouseId)
            {
                TempData["Error"] = "You can only delete participants from your own house.";
                return RedirectToAction(nameof(Index));
            }

            var participantName = participant.FullName;
            await _participantService.DeleteAsync(id);

            TempData["Success"] = $"Participant {participantName} has been successfully deleted.";
            _logger.LogInformation("House leader {UserId} deleted participant {ParticipantId}", userId, id);

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting participant {ParticipantId} by user {UserId}", id, userId);
            TempData["Error"] = "An error occurred while deleting the participant. Please try again.";
            return RedirectToAction(nameof(Index));
        }
    }

    private void SetupParticipantViewBag()
    {
        ViewBag.GenderGroups = new SelectList(
            new[] { DivisionType.Boys, DivisionType.Girls },
            DivisionType.Boys);
        ViewBag.EventClasses = new SelectList(Enum.GetValues(typeof(EventClass)));
    }
}