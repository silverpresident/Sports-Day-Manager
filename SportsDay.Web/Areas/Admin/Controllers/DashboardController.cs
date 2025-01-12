using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsDay.Lib.Data;
using SportsDay.Lib.Services;

namespace SportsDay.Web.Areas.Admin.Controllers;

[Area("Admin")]
//[Authorize(Roles = "Administrator")]
public class DashboardController : Controller
{
    private readonly SportsDayDbContext _context;
    private readonly ITournamentService _tournamentService;

    public DashboardController(SportsDayDbContext context, ITournamentService tournamentService)
    {
        _context = context;
        _tournamentService = tournamentService;
    }

    public async Task<IActionResult> Index()
    {
        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        
        var viewModel = new
        {
            ActiveTournament = activeTournament,
            TotalTournaments = await _context.Tournaments.CountAsync(),
            TotalEvents = await _context.Events.CountAsync(),
            TotalParticipants = await _context.Participants.CountAsync(),
            TotalResults = await _context.Results.CountAsync(),
            ActiveAnnouncements = await _context.Announcements.CountAsync(a => a.IsEnabled && (!a.ExpiresAt.HasValue || a.ExpiresAt > DateTime.Now)),
            RecentUpdates = await _context.EventUpdates
                .Include(u => u.Event)
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .ToListAsync(),
            RecentResults = await _context.Results
                .Include(r => r.Event)
                .Include(r => r.Participant)
                .Include(r => r.House)
                .OrderByDescending(r => r.Id)
                .Take(5)
                .ToListAsync()
        };

        return View(viewModel);
    }
}
