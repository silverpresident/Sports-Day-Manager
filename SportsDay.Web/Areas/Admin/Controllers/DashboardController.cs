using Microsoft.AspNetCore.Mvc;
using SportsDay.Lib.Data;

namespace SportsDay.Web.Areas.Admin.Controllers;

public class DashboardController : AdminBaseController
{
    private readonly SportsDayDbContext _context;

    public DashboardController(SportsDayDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var stats = new
        {
            TotalTournaments = _context.Tournaments.Count(),
            TotalParticipants = _context.Participants.Count(),
            TotalEvents = _context.Events.Count(),
            TotalResults = _context.Results.Count(),
            ActiveAnnouncements = _context.Announcements.Count(a => a.IsEnabled && (!a.ExpiresAt.HasValue || a.ExpiresAt > DateTime.UtcNow))
        };

        return View(stats);
    }
}
