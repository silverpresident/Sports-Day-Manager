using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SportsDay.Lib.Data;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services;
using SportsDay.Web.Hubs;
using SportsDay.Lib.Enums;
using Markdig;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Web.Areas.Admin.Controllers;

[Area("Admin")]
//[Authorize(Roles = "Administrator")]
public class AnnouncementsController : Controller
{
    private readonly SportsDayDbContext _context;
    private readonly ITournamentService _tournamentService;
    private readonly IHubContext<SportsHub> _hubContext;
    private readonly MarkdownPipeline _markdownPipeline;

    public AnnouncementsController(
        SportsDayDbContext context, 
        ITournamentService tournamentService,
        IHubContext<SportsHub> hubContext)
    {
        _context = context;
        _tournamentService = tournamentService;
        _hubContext = hubContext;
        _markdownPipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseBootstrap()
            .UseEmojiAndSmiley()
            .Build();
    }

    public async Task<IActionResult> Index()
    {
        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            TempData["Error"] = "No active tournament found. Please activate a tournament first.";
            return RedirectToAction("Index", "Tournaments", new { area = "Admin" });
        }

        var announcements = await _context.Announcements
            .Where(a => a.TournamentId == activeTournament.Id)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();

        ViewBag.ActiveTournament = activeTournament;
        return View(announcements);
    }

    public async Task<IActionResult> Create()
    {
        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            TempData["Error"] = "No active tournament found. Please activate a tournament first.";
            return RedirectToAction("Index", "Tournaments", new { area = "Admin" });
        }

        return View(new Announcement 
        { 
            TournamentId = activeTournament.Id,
            Priority = AnnouncementPriority.Info,
            IsEnabled = true,
            CreatedAt = DateTime.Now
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Announcement announcement)
    {
        if (ModelState.IsValid)
        {
            announcement.Id = Guid.NewGuid();
            announcement.CreatedBy = "system";
            announcement.CreatedAt = DateTime.UtcNow;
            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            // Send real-time update
            await _hubContext.Clients.All.SendAsync("ReceiveAnnouncement");

            return RedirectToAction(nameof(Index));
        }
        return View(announcement);
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var announcement = await _context.Announcements.FindAsync(id);
        if (announcement == null)
        {
            return NotFound();
        }

        return View(announcement);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Announcement announcement)
    {
        if (id != announcement.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                announcement.UpdatedBy = "system";
                announcement.UpdatedAt = DateTime.UtcNow;
                _context.Update(announcement);
                await _context.SaveChangesAsync();

                // Send real-time update
                await _hubContext.Clients.All.SendAsync("ReceiveAnnouncement");

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Announcements.AnyAsync(a => a.Id == id))
                {
                    return NotFound();
                }
                throw;
            }
        }
        return View(announcement);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var announcement = await _context.Announcements.FindAsync(id);
        if (announcement == null)
        {
            return NotFound();
        }

        _context.Announcements.Remove(announcement);
        await _context.SaveChangesAsync();

        // Send real-time update
        await _hubContext.Clients.All.SendAsync("ReceiveAnnouncement");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(Guid id)
    {
        var announcement = await _context.Announcements.FindAsync(id);
        if (announcement == null)
        {
            return NotFound();
        }

        announcement.IsEnabled = !announcement.IsEnabled;
        await _context.SaveChangesAsync();

        // Send real-time update
        await _hubContext.Clients.All.SendAsync("ReceiveAnnouncement");

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Preview([FromBody] string markdown)
    {
        if (string.IsNullOrEmpty(markdown))
        {
            return Json(new { html = "" });
        }

        var html = Markdown.ToHtml(markdown, _markdownPipeline);
        return Json(new { html });
    }
}
