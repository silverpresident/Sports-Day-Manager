using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportsDay.Lib.Data;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Web.Controllers;

public class AnnouncementController : Controller
{
    private readonly SportsDayDbContext _context;
    private readonly ITournamentService _tournamentService;
    private readonly ILogger<AnnouncementController> _logger;

    public AnnouncementController(
        SportsDayDbContext context,
        ITournamentService tournamentService,
        ILogger<AnnouncementController> logger)
    {
        _context = context;
        _tournamentService = tournamentService;
        _logger = logger;
    }

    /// <summary>
    /// Display all active announcements for the active tournament
    /// </summary>
    public async Task<IActionResult> Index()
    {
        try
        {
            _logger.LogInformation("Loading announcements index");

            var activeTournament = await _tournamentService.GetActiveTournamentAsync();
            if (activeTournament == null)
            {
                _logger.LogWarning("No active tournament found");
                ViewBag.Message = "No active tournament found.";
                return View(new List<Announcement>());
            }

            var announcements = await _context.Announcements
                .Where(a => a.TournamentId == activeTournament.Id && a.IsEnabled)
                .Where(a => !a.ExpiresAt.HasValue || a.ExpiresAt > DateTime.Now)
                .OrderByDescending(a => a.Priority)
                .ThenByDescending(a => a.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

            ViewBag.ActiveTournament = activeTournament;
            _logger.LogInformation("Loaded {Count} announcements", announcements.Count);

            return View(announcements);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading announcements");
            ViewBag.Error = "An error occurred while loading announcements.";
            return View(new List<Announcement>());
        }
    }

    /// <summary>
    /// Display details of a specific announcement
    /// </summary>
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            _logger.LogInformation("Loading announcement details for {Id}", id);

            var announcement = await _context.Announcements
                .Include(a => a.Tournament)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);

            if (announcement == null)
            {
                _logger.LogWarning("Announcement {Id} not found", id);
                return NotFound();
            }

            // Check if announcement is enabled and not expired
            if (!announcement.IsEnabled || (announcement.ExpiresAt.HasValue && announcement.ExpiresAt <= DateTime.Now))
            {
                _logger.LogWarning("Announcement {Id} is disabled or expired", id);
                return NotFound();
            }

            return View(announcement);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading announcement details for {Id}", id);
            return NotFound();
        }
    }
}