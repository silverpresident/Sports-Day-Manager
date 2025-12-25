#if DEBUG
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Web.Areas.Admin.Controllers;

/// <summary>
/// Developer controller for testing and data generation.
/// Only available in DEBUG mode.
/// </summary>
[Area("Admin")]
//[Authorize(Roles = "Administrator")]
public class DeveloperController : Controller
{
    private readonly ITournamentService _tournamentService;
    private readonly IDeveloperService _developerService;
    private readonly ILogger<DeveloperController> _logger;

    public DeveloperController(
        ITournamentService tournamentService,
        IDeveloperService developerService,
        ILogger<DeveloperController> logger)
    {
        _tournamentService = tournamentService;
        _developerService = developerService;
        _logger = logger;
    }

    /// <summary>
    /// Developer dashboard with data generation and cleanup actions.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            TempData["Error"] = "No active tournament found. Please activate a tournament first.";
            return RedirectToAction("Index", "Tournaments", new { area = "Admin" });
        }

        var stats = await _developerService.GetStatsAsync(activeTournament.Id);
        
        ViewBag.ActiveTournament = activeTournament;
        ViewBag.Stats = stats;
        
        return View();
    }

    /// <summary>
    /// Generate events from event templates for the active tournament.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateEvents()
    {
        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            TempData["Error"] = "No active tournament found.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var createdBy = User.Identity?.Name ?? "Developer";
            var count = await _developerService.GenerateEventsAsync(activeTournament.Id, createdBy);
            TempData["Success"] = $"Successfully generated {count} events from templates.";
            _logger.LogInformation("Generated {Count} events for tournament {TournamentId}", count, activeTournament.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating events for tournament {TournamentId}", activeTournament.Id);
            TempData["Error"] = $"Error generating events: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Generate random participants for each house.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateParticipants()
    {
        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            TempData["Error"] = "No active tournament found.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var createdBy = User.Identity?.Name ?? "Developer";
            var count = await _developerService.GenerateParticipantsAsync(activeTournament.Id, createdBy);
            TempData["Success"] = $"Successfully generated {count} participants (2 per house).";
            _logger.LogInformation("Generated {Count} participants for tournament {TournamentId}", count, activeTournament.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating participants for tournament {TournamentId}", activeTournament.Id);
            TempData["Error"] = $"Error generating participants: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Generate participation records (assign participants to events).
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateParticipation()
    {
        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            TempData["Error"] = "No active tournament found.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var createdBy = User.Identity?.Name ?? "Developer";
            var count = await _developerService.GenerateParticipationAsync(activeTournament.Id, createdBy);
            TempData["Success"] = $"Successfully generated {count} participation records.";
            _logger.LogInformation("Generated {Count} participations for tournament {TournamentId}", count, activeTournament.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating participation for tournament {TournamentId}", activeTournament.Id);
            TempData["Error"] = $"Error generating participation: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Generate results for events without results.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateResults()
    {
        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            TempData["Error"] = "No active tournament found.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var createdBy = User.Identity?.Name ?? "Developer";
            var count = await _developerService.GenerateResultsAsync(activeTournament.Id, createdBy);
            TempData["Success"] = $"Successfully generated {count} results.";
            _logger.LogInformation("Generated {Count} results for tournament {TournamentId}", count, activeTournament.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating results for tournament {TournamentId}", activeTournament.Id);
            TempData["Error"] = $"Error generating results: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Delete all results for the active tournament.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAllResults()
    {
        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            TempData["Error"] = "No active tournament found.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var count = await _developerService.DeleteAllResultsAsync(activeTournament.Id);
            TempData["Success"] = $"Successfully deleted {count} results.";
            _logger.LogInformation("Deleted {Count} results for tournament {TournamentId}", count, activeTournament.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting results for tournament {TournamentId}", activeTournament.Id);
            TempData["Error"] = $"Error deleting results: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Delete all participation records for the active tournament.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAllParticipation()
    {
        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            TempData["Error"] = "No active tournament found.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var count = await _developerService.DeleteAllParticipationAsync(activeTournament.Id);
            TempData["Success"] = $"Successfully deleted {count} participation records.";
            _logger.LogInformation("Deleted {Count} participations for tournament {TournamentId}", count, activeTournament.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting participation for tournament {TournamentId}", activeTournament.Id);
            TempData["Error"] = $"Error deleting participation: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Delete all participants for the active tournament.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAllParticipants()
    {
        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            TempData["Error"] = "No active tournament found.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var count = await _developerService.DeleteAllParticipantsAsync(activeTournament.Id);
            TempData["Success"] = $"Successfully deleted {count} participants.";
            _logger.LogInformation("Deleted {Count} participants for tournament {TournamentId}", count, activeTournament.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting participants for tournament {TournamentId}", activeTournament.Id);
            TempData["Error"] = $"Error deleting participants: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Delete all events for the active tournament.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAllEvents()
    {
        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            TempData["Error"] = "No active tournament found.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var count = await _developerService.DeleteAllEventsAsync(activeTournament.Id);
            TempData["Success"] = $"Successfully deleted {count} events.";
            _logger.LogInformation("Deleted {Count} events for tournament {TournamentId}", count, activeTournament.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting events for tournament {TournamentId}", activeTournament.Id);
            TempData["Error"] = $"Error deleting events: {ex.Message}";
        }

        return RedirectToAction(nameof(Index));
    }
}
#endif