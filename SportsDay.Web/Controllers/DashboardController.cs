using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsDay.Lib.Data;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services;
using SportsDay.Web.Models;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Web.Controllers;

public class DashboardController : Controller
{
    private readonly ILogger<DashboardController> _logger;
    private readonly SportsDayDbContext _context;
    private readonly ITournamentService _tournamentService;

    public DashboardController(
        ILogger<DashboardController> logger,
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
            return View();
        }

        var announcements = await _context.Announcements
            .Where(a => a.TournamentId == activeTournament.Id && a.IsEnabled)
            .Where(a => !a.ExpiresAt.HasValue || a.ExpiresAt > DateTime.Now)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        var updates = await _context.EventUpdates
            .Where(u => u.TournamentId == activeTournament.Id)
            .OrderByDescending(u => u.CreatedAt)
            .Take(20)  // Show last 20 updates
            .ToListAsync();

        var summaries = await _context.TournamentHouseSummaries
            .Include(s => s.Tournament)
            .Include(s => s.House)
                .ThenInclude(h => h.Participants!)
            .Include(s => s.Division)
            .Where(s => s.TournamentId == activeTournament.Id)
            .ToListAsync();

        ViewBag.ActiveTournament = activeTournament;
        ViewBag.Announcements = announcements;
        ViewBag.Updates = updates;
        ViewBag.Summaries = summaries;
        ViewBag.DivisionCount = summaries.Select(s => s.Division).Distinct().Count();

        return View();
    }

    public async Task<IActionResult> GetAnnouncements()
    {
        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            return PartialView("_AnnouncementsPartial", new List<Announcement>());
        }

        var announcements = await _context.Announcements
            .Where(a => a.TournamentId == activeTournament.Id && a.IsEnabled)
            .Where(a => !a.ExpiresAt.HasValue || a.ExpiresAt > DateTime.Now)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        return PartialView("_AnnouncementsPartial", announcements);
    }

    public async Task<IActionResult> GetUpdates()
    {
        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            return PartialView("_UpdateStreamPartial", new List<EventUpdate>());
        }

        var updates = await _context.EventUpdates
            .Where(u => u.TournamentId == activeTournament.Id)
            .OrderByDescending(u => u.CreatedAt)
            .Take(20)  // Show last 20 updates
            .ToListAsync();

        return PartialView("_UpdateStreamPartial", updates);
    }

    public async Task<IActionResult> GetLeaderboard()
    {
        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            return PartialView("_LeaderboardPartial", new List<TournamentHouseSummary>());
        }

        var summaries = await _context.TournamentHouseSummaries
            .Include(s => s.Tournament)
            .Include(s => s.House)
                .ThenInclude(h => h.Participants!)
            .Include(s => s.Division)
            .Where(s => s.TournamentId == activeTournament.Id)
            .ToListAsync();

        return PartialView("_LeaderboardPartial", summaries);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
