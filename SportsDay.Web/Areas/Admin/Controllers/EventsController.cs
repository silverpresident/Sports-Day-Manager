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
public class EventsController : Controller
{
    private readonly SportsDayDbContext _context;
    private readonly ITournamentService _tournamentService;
    private readonly IHubContext<SportsHub> _hubContext;

    public EventsController(
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

        var events = await _context.Events
            .Include(e => e.Division)
            .Where(e => e.TournamentId == activeTournament.Id)
            .OrderBy(e => e.Division)
            .ThenBy(e => e.Name)
            .ToListAsync();

        ViewBag.ActiveTournament = activeTournament;
        return View(events);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var evt = await _context.Events
            .Include(e => e.Division)
            .Include(e => e.Results)
                .ThenInclude(r => r.Participant)
            .Include(e => e.Results)
                .ThenInclude(r => r.House)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (evt == null)
        {
            return NotFound();
        }

        return View(evt);
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
        return View(new Event { TournamentId = activeTournament.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Event evt)
    {
        if (ModelState.IsValid)
        {
            evt.Id = Guid.NewGuid();
            evt.CreatedBy = "system";
            evt.CreatedAt = DateTime.UtcNow;
            _context.Events.Add(evt);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        await PopulateViewBagAsync(evt.TournamentId);
        return View(evt);
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var evt = await _context.Events.FindAsync(id);
        if (evt == null)
        {
            return NotFound();
        }

        await PopulateViewBagAsync(evt.TournamentId);
        return View(evt);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, Event evt)
    {
        if (id != evt.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                evt.UpdatedBy = "system";
                evt.UpdatedAt = DateTime.UtcNow;
                _context.Update(evt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Events.AnyAsync(e => e.Id == id))
                {
                    return NotFound();
                }
                throw;
            }
        }

        await PopulateViewBagAsync(evt.TournamentId);
        return View(evt);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var evt = await _context.Events.FindAsync(id);
        if (evt == null)
        {
            return NotFound();
        }

        _context.Events.Remove(evt);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(Guid id, EventStatus status)
    {
        var evt = await _context.Events.FindAsync(id);
        if (evt == null)
        {
            return NotFound();
        }

        evt.Status = status.ToString();
        await _context.SaveChangesAsync();

        // Create event update
        var update = new EventUpdate
        {
            Id = Guid.NewGuid(),
            EventId = evt.Id,
            TournamentId = evt.TournamentId,
            Message = $"Event status changed to {status}",
            CreatedAt = DateTime.Now
        };
        _context.EventUpdates.Add(update);
        await _context.SaveChangesAsync();

        // Send real-time update
        await _hubContext.Clients.All.SendAsync("ReceiveEventUpdate", evt.Name, status.ToString());

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateViewBagAsync(Guid tournamentId)
    {
        // Get divisions for active tournament
        var divisions = await _context.Events.Select(e => e.Division).Distinct()
            .Where(d => d.TournamentId == tournamentId)
            .OrderBy(d => d.Name)
            .ToListAsync();

        ViewBag.Divisions = new SelectList(divisions, "Id", "Name");

        // Event classes
        ViewBag.Classes = new SelectList(Enum.GetValues(typeof(EventClass))
            .Cast<EventClass>()
            .Select(c => new
            {
                Id = (int)c,
                Name = c == EventClass.Open ? "Open" : $"Class {(int)c}"
            }), "Id", "Name");

        // Event types
        ViewBag.Types = new SelectList(Enum.GetValues(typeof(EventType))
            .Cast<EventType>()
            .Select(t => new
            {
                Id = t,
                Name = t.ToString()
            }), "Id", "Name");

        // Point systems
        ViewBag.PointSystems = new List<SelectListItem>
        {
            new SelectListItem { Value = "9,7,6,5,4,3,2,1", Text = "9,7,6,5,4,3,2,1" },
            new SelectListItem { Value = "12,10,9,8,7,6,1", Text = "12,10,9,8,7,6,1" }
        };

        // Event statuses
        ViewBag.Statuses = new SelectList(Enum.GetValues(typeof(EventStatus)) 
            .Cast<EventStatus>()
            .Select(s => new SelectListItem
            {
                Value = s.ToString(),
                Text = s.ToString()
            }), "Value", "Text");
    }
}
