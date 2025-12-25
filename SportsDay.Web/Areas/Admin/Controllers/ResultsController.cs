using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SportsDay.Lib.Data;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services;
using SportsDay.Web.Hubs;
using SportsDay.Lib.Enums;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Web.Areas.Admin.Controllers;

[Area("Admin")]
//[Authorize(Roles = "Administrator")]
public class ResultsController : Controller
{
    private readonly SportsDayDbContext _context;
    private readonly ITournamentService _tournamentService;
    private readonly IHubContext<SportsHub> _hubContext;

    public ResultsController(
        SportsDayDbContext context, 
        ITournamentService tournamentService,
        IHubContext<SportsHub> hubContext)
    {
        _context = context;
        _tournamentService = tournamentService;
        _hubContext = hubContext;
    }

    public async Task<IActionResult> Index()
    {
        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            TempData["Error"] = "No active tournament found. Please activate a tournament first.";
            return RedirectToAction("Index", "Tournaments", new { area = "Admin" });
        }

        var results = await _context.Results
            .Include(r => r.Event)
            .Include(r => r.Participant)
            .Include(r => r.House)
            .Where(r => r.TournamentId == activeTournament.Id)
            .OrderByDescending(r => r.Event.ScheduledTime)
            .ThenBy(r => r.Event.Name)
            .ThenBy(r => r.Placement)
            .ToListAsync();

        ViewBag.ActiveTournament = activeTournament;
        return View(results);
    }

    public async Task<IActionResult> Create()
    {
        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            TempData["Error"] = "No active tournament found. Please activate a tournament first.";
            return RedirectToAction("Index", "Tournaments", new { area = "Admin" });
        }

        await PopulateViewBagAsync(activeTournament.Id);
        return View(new Result { TournamentId = activeTournament.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Result result)
    {
        if (ModelState.IsValid)
        {
            var evt = await _context.Events.FindAsync(result.EventId);
            if (evt == null)
            {
                return NotFound();
            }

            result.Id = Guid.NewGuid();
            result.CreatedBy = "system";
            result.CreatedAt = DateTime.UtcNow;

            // Check if this is a new record
            var recordHolder = await _context.Participants.FindAsync(result.ParticipantId);
            if (evt.Type == EventType.Distance && result.SpeedOrDistance > (evt.Record ?? 0m))
            {
                result.IsNewRecord = true;
                evt.Record = result.SpeedOrDistance;
                evt.RecordHolder = recordHolder?.FullName;
            }
            else if (evt.Type == EventType.Speed && result.SpeedOrDistance < (evt.Record ?? decimal.MaxValue))
            {
                result.IsNewRecord = true;
                evt.Record = result.SpeedOrDistance;
                evt.RecordHolder = recordHolder?.FullName;
            }

            // Calculate points based on placement
            if (result.Placement.HasValue)
            {
                var points = evt.PointSystem.Split(',')
                    .Select(int.Parse)
                    .ToList();
                
                result.Points = result.Placement.Value <= points.Count 
                    ? points[result.Placement.Value - 1] 
                    : 0;
            }

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            // Create event update
            var update = new EventUpdate
            {
                Id = Guid.NewGuid(),
                EventId = evt.Id,
                TournamentId = evt.TournamentId,
                Message = result.IsNewRecord
                    ? $"New record set in {evt.Name}!"
                    : $"New result recorded for {evt.Name}",
                CreatedAt = DateTime.Now
            };
            _context.EventUpdates.Add(update);
            await _context.SaveChangesAsync();

            // Send real-time updates
            await _hubContext.Clients.All.SendAsync("ReceiveResult", evt.Name, 
                recordHolder?.FullName, 
                result.Placement?.ToString() ?? result.SpeedOrDistance?.ToString());

            return RedirectToAction(nameof(Index));
        }

        await PopulateViewBagAsync(result.TournamentId);
        return View(result);
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _context.Results
            .Include(r => r.Event)
            .Include(r => r.Participant)
            .Include(r => r.House)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (result == null)
        {
            return NotFound();
        }

        await PopulateViewBagAsync(result.TournamentId);
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Result result)
    {
        if (id != result.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var evt = await _context.Events.FindAsync(result.EventId);
                if (evt == null)
                {
                    return NotFound();
                }

                // Recalculate points based on placement
                if (result.Placement.HasValue)
                {
                    var points = evt.PointSystem.Split(',')
                        .Select(int.Parse)
                        .ToList();
                    
                    result.Points = result.Placement.Value <= points.Count 
                        ? points[result.Placement.Value - 1] 
                        : 0;
                }

                result.UpdatedBy = "system";
                result.UpdatedAt = DateTime.UtcNow;
                _context.Update(result);
                await _context.SaveChangesAsync();

                // Send real-time update
                var resultParticipant = await _context.Participants.FindAsync(result.ParticipantId);
                await _hubContext.Clients.All.SendAsync("ReceiveResult", evt.Name, 
                    resultParticipant?.FullName, 
                    result.Placement?.ToString() ?? result.SpeedOrDistance?.ToString());

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Results.AnyAsync(r => r.Id == id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        await PopulateViewBagAsync(result.TournamentId);
        return View(result);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _context.Results.FindAsync(id);
        if (result == null)
        {
            return NotFound();
        }

        _context.Results.Remove(result);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateViewBagAsync(Guid tournamentId)
    {
        // Get events for active tournament
        var events = await _context.Events
            .Where(e => e.TournamentId == tournamentId && e.Status == EventStatus.InProgress)
            .OrderBy(e => e.GenderGroup)
            .ThenBy(e => e.Name)
            .ToListAsync();

        ViewBag.Events = new SelectList(events, "Id", "Name");

        // Get participants
        var participants = await _context.Participants
            .Include(p => p.House)
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();

        ViewBag.Participants = new SelectList(participants, "Id", "FullName", null, "House.Name");

        // Get houses
        var houses = await _context.Houses
            .OrderBy(h => h.Name)
            .ToListAsync();

        ViewBag.Houses = new SelectList(houses, "Id", "Name");
    }
}
