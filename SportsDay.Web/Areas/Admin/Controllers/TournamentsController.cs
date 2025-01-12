using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services;

namespace SportsDay.Web.Areas.Admin.Controllers;

[Area("Admin")]
//[Authorize(Roles = "Administrator")]
public class TournamentsController : Controller
{
    private readonly ITournamentService _tournamentService;

    public TournamentsController(ITournamentService tournamentService)
    {
        _tournamentService = tournamentService;
    }

    public async Task<IActionResult> Index()
    {
        var tournaments = await _tournamentService.GetAllTournamentsAsync();
        return View(tournaments);
    }

    public IActionResult Create()
    {
        return View(new Tournament 
        { 
            TournamentDate = DateTime.Today,
            Name = $"Sports Day {DateTime.Today.Year}"
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Tournament tournament)
    {
        if (ModelState.IsValid)
        {
            // Set default values
            tournament.Id = Guid.NewGuid();
            tournament.IsActive = false;

            // TODO: Add tournament creation logic
            return RedirectToAction(nameof(Index));
        }
        return View(tournament);
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var tournament = await _tournamentService.GetTournamentByIdAsync(id);
        if (tournament == null)
        {
            return NotFound();
        }
        return View(tournament);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Tournament tournament)
    {
        if (id != tournament.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            // TODO: Add tournament update logic
            return RedirectToAction(nameof(Index));
        }
        return View(tournament);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetActive(Guid id)
    {
        var result = await _tournamentService.SetActiveTournamentAsync(id);
        if (!result)
        {
            TempData["Error"] = "Failed to set tournament as active.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var result = await _tournamentService.DeactivateTournamentAsync(id);
        if (!result)
        {
            TempData["Error"] = "Failed to deactivate tournament.";
        }
        return RedirectToAction(nameof(Index));
    }
}
