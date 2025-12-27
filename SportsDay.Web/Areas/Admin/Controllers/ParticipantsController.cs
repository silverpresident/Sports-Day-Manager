using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportsDay.Lib.Enums;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Web.Areas.Admin.Controllers;

/// <summary>
/// Admin controller for managing participants across all houses and tournaments
/// </summary>
public class ParticipantsController : AdminBaseController
{
    private readonly IParticipantService _participantService;
    private readonly IHouseService _houseService;
    private readonly ITournamentService _tournamentService;
    private readonly ILogger<ParticipantsController> _logger;

    public ParticipantsController(
        IParticipantService participantService,
        IHouseService houseService,
        ITournamentService tournamentService,
        ILogger<ParticipantsController> logger)
    {
        _participantService = participantService;
        _houseService = houseService;
        _tournamentService = tournamentService;
        _logger = logger;
    }

    /// <summary>
    /// GET: List all participants
    /// </summary>
    public async Task<IActionResult> Index(Guid? tournamentId, int? houseId)
    {
        try
        {
            IEnumerable<Participant> participants;

            if (tournamentId.HasValue && houseId.HasValue)
            {
                participants = await _participantService.GetByTournamentAndHouseAsync(tournamentId.Value, houseId.Value);
            }
            else if (tournamentId.HasValue)
            {
                participants = await _participantService.GetByTournamentIdAsync(tournamentId.Value);
            }
            else if (houseId.HasValue)
            {
                participants = await _participantService.GetByHouseIdAsync(houseId.Value);
            }
            else
            {
                participants = await _participantService.GetAllAsync();
            }

            // Populate filter dropdowns
            var tournaments = await _tournamentService.GetTournamentsAsync();
            var houses = await _houseService.GetAllAsync();

            ViewBag.Tournaments = new SelectList(tournaments, "Id", "Name", tournamentId);
            ViewBag.Houses = new SelectList(houses, "Id", "Name", houseId);
            ViewBag.SelectedTournamentId = tournamentId;
            ViewBag.SelectedHouseId = houseId;

            _logger.LogInformation("Retrieved {Count} participants with filters: TournamentId={TournamentId}, HouseId={HouseId}",
                participants.Count(), tournamentId, houseId);

            return View(participants);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving participants");
            TempData["Error"] = "An error occurred while retrieving participants.";
            return View(new List<Participant>());
        }
    }

    /// <summary>
    /// GET: View participant details
    /// </summary>
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var participant = await _participantService.GetByIdAsync(id);
            if (participant == null)
            {
                TempData["Error"] = "Participant not found.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("Viewing details for participant {ParticipantId}", id);
            return View(participant);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving participant {ParticipantId}", id);
            TempData["Error"] = "An error occurred while retrieving the participant.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// GET: Create a new participant
    /// </summary>
    public async Task<IActionResult> Create()
    {
        try
        {
            await SetupViewBagAsync();
            return View(new Participant());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading create participant form");
            TempData["Error"] = "An error occurred while loading the form.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// POST: Create a new participant
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Participant participant)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                await SetupViewBagAsync(participant.TournamentId, participant.HouseId);
                return View(participant);
            }

            // Set audit fields
            participant.CreatedBy = User.Identity?.Name ?? "Admin";

            // Calculate age
            var today = DateTime.Today;
            var age = today.Year - participant.DateOfBirth.Year;
            if (participant.DateOfBirth.Date > today.AddYears(-age)) age--;
            participant.AgeInYears = age;

            // Set ClassGroupNumber based on EventClassGroup
            participant.ClassGroupNumber = (int)participant.EventClassGroup;

            await _participantService.CreateAsync(participant);

            TempData["Success"] = $"Participant {participant.FullName} has been successfully created!";
            _logger.LogInformation("Admin {User} created participant {ParticipantName} (ID: {ParticipantId})",
                User.Identity?.Name, participant.FullName, participant.Id);

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating participant");
            TempData["Error"] = "An error occurred while creating the participant. Please try again.";
            await SetupViewBagAsync(participant.TournamentId, participant.HouseId);
            return View(participant);
        }
    }

    /// <summary>
    /// GET: Edit a participant
    /// </summary>
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var participant = await _participantService.GetByIdAsync(id);
            if (participant == null)
            {
                TempData["Error"] = "Participant not found.";
                return RedirectToAction(nameof(Index));
            }

            await SetupViewBagAsync(participant.TournamentId, participant.HouseId);
            _logger.LogInformation("Editing participant {ParticipantId}", id);
            return View(participant);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading edit form for participant {ParticipantId}", id);
            TempData["Error"] = "An error occurred while loading the participant.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// POST: Edit a participant
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Participant participant)
    {
        if (id != participant.Id)
        {
            return NotFound();
        }

        try
        {
            if (!ModelState.IsValid)
            {
                await SetupViewBagAsync(participant.TournamentId, participant.HouseId);
                return View(participant);
            }

            // Set audit fields
            participant.UpdatedBy = User.Identity?.Name ?? "Admin";

            // Recalculate age
            var today = DateTime.Today;
            var age = today.Year - participant.DateOfBirth.Year;
            if (participant.DateOfBirth.Date > today.AddYears(-age)) age--;
            participant.AgeInYears = age;

            // Set ClassGroupNumber based on EventClassGroup
            participant.ClassGroupNumber = (int)participant.EventClassGroup;

            await _participantService.UpdateAsync(participant);

            TempData["Success"] = $"Participant {participant.FullName} has been successfully updated!";
            _logger.LogInformation("Admin {User} updated participant {ParticipantId}",
                User.Identity?.Name, id);

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating participant {ParticipantId}", id);
            TempData["Error"] = "An error occurred while updating the participant. Please try again.";
            await SetupViewBagAsync(participant.TournamentId, participant.HouseId);
            return View(participant);
        }
    }

    /// <summary>
    /// GET: Delete a participant
    /// </summary>
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var participant = await _participantService.GetByIdAsync(id);
            if (participant == null)
            {
                TempData["Error"] = "Participant not found.";
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("Viewing delete confirmation for participant {ParticipantId}", id);
            return View(participant);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading delete confirmation for participant {ParticipantId}", id);
            TempData["Error"] = "An error occurred while loading the participant.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// POST: Confirm delete a participant
    /// </summary>
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(Guid id)
    {
        try
        {
            var participant = await _participantService.GetByIdAsync(id);
            if (participant == null)
            {
                TempData["Error"] = "Participant not found.";
                return RedirectToAction(nameof(Index));
            }

            var participantName = participant.FullName;
            await _participantService.DeleteAsync(id);

            TempData["Success"] = $"Participant {participantName} has been successfully deleted.";
            _logger.LogInformation("Admin {User} deleted participant {ParticipantId} ({ParticipantName})",
                User.Identity?.Name, id, participantName);

            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting participant {ParticipantId}", id);
            TempData["Error"] = "An error occurred while deleting the participant. Please try again.";
            return RedirectToAction(nameof(Index));
        }
    }

    /// <summary>
    /// Setup ViewBag with dropdown data
    /// </summary>
    private async Task SetupViewBagAsync(Guid? selectedTournamentId = null, int? selectedHouseId = null)
    {
        var tournaments = await _tournamentService.GetTournamentsAsync();
        var houses = await _houseService.GetAllAsync();

        ViewBag.Tournaments = new SelectList(tournaments, "Id", "Name", selectedTournamentId);
        ViewBag.Houses = new SelectList(houses, "Id", "Name", selectedHouseId);
        ViewBag.GenderGroups = new SelectList(Enum.GetValues(typeof(DivisionType)));
        ViewBag.EventClasses = new SelectList(Enum.GetValues(typeof(EventClass)));
    }
}