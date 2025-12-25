using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsDay.Lib.Data;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Web.Controllers;

public class UpdateController : Controller
{
    private readonly ILogger<UpdateController> _logger;
    private readonly SportsDayDbContext _context;
    private readonly ITournamentService _tournamentService;

    public UpdateController(
        ILogger<UpdateController> logger,
        SportsDayDbContext context,
        ITournamentService tournamentService)
    {
        _logger = logger;
        _context = context;
        _tournamentService = tournamentService;
    }

    public async Task<IActionResult> Index()
    {
        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            ViewBag.ActiveTournament = null;
            ViewBag.Updates = new List<EventUpdate>();
            return View();
        }

        var updates = await _context.EventUpdates
            .Include(u => u.Event)
            .Include(u => u.Tournament)
            .Where(u => u.TournamentId == activeTournament.Id)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        ViewBag.ActiveTournament = activeTournament;
        ViewBag.Updates = updates;

        return View();
    }

    public async Task<IActionResult> GetUpdates()
    {
        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            return PartialView("_UpdateStreamPartial", new List<EventUpdate>());
        }

        var updates = await _context.EventUpdates
            .Include(u => u.Event)
            .Include(u => u.Tournament)
            .Where(u => u.TournamentId == activeTournament.Id)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        return PartialView("_UpdateStreamPartial", updates);
    }
}