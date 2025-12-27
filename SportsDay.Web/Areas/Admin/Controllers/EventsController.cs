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
using Microsoft.Extensions.Logging;
using SportsDay.Web.Models;

namespace SportsDay.Web.Areas.Admin.Controllers;

[Area("Admin")]
//[Authorize(Roles = "Administrator")]
public class EventsController : AdminBaseController
{
    private readonly SportsDayDbContext _context;
    private readonly ITournamentService _tournamentService;
    private readonly IEventService _eventService;
    private readonly IParticipantService _participantService;
    private readonly IHubContext<SportsHub> _hubContext;
    private readonly ILogger<EventsController> _logger;

    public EventsController(
        SportsDayDbContext context,
        ITournamentService tournamentService,
        IEventService eventService,
        IParticipantService participantService,
        IHubContext<SportsHub> hubContext,
        ILogger<EventsController> logger)
    {
        _context = context;
        _tournamentService = tournamentService;
        _eventService = eventService;
        _participantService = participantService;
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string statusFilter)
    {
        try
        {
            var activeTournament = await _tournamentService.GetActiveTournamentAsync();
            if (activeTournament == null)
            {
                TempData["Error"] = "No active tournament found. Please activate a tournament first.";
                return RedirectToAction("Index", "Tournaments", new { area = "Admin" });
            }

            var events = await _eventService.GetByTournamentIdAsync(activeTournament.Id);

            if (!string.IsNullOrEmpty(statusFilter) && Enum.TryParse<EventStatus>(statusFilter, out var parsedStatus))
            {
                events = events.Where(e => e.Status == parsedStatus);
            }

            ViewBag.ActiveTournament = activeTournament;
            ViewBag.CurrentFilter = statusFilter;

            return View(events.OrderBy(e => e.EventNumber).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading events index");
            TempData["Error"] = "An error occurred while loading events.";
            return RedirectToAction("Index", "Dashboard");
        }
    }

    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var evt = await _eventService.GetByIdWithResultsAsync(id);
            if (evt == null)
            {
                return NotFound();
            }

            // Get eligible participants for adding to event
            var eligibleParticipants = await _eventService.GetEligibleParticipantsAsync(id);
            ViewBag.EligibleParticipants = eligibleParticipants;

            return View(evt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading event details for {EventId}", id);
            TempData["Error"] = "An error occurred while loading event details.";
            return RedirectToAction(nameof(Index));
        }
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
        // Remove AgeGroup from validation
        ModelState.Remove(nameof(evt.AgeGroup));
        
        if (ModelState.IsValid)
        {
            try
            {
                // Set AgeGroup based on ClassGroup
                evt.AgeGroup = evt.ClassGroup == EventClass.Open ? "0" : ((int)evt.ClassGroup).ToString();
                
                evt.CreatedBy = User.Identity?.Name ?? "system";
                var createdEvent = await _eventService.CreateAsync(evt);
                
                _logger.LogInformation("Event {EventName} created by {User}", evt.Name, evt.CreatedBy);
                TempData["Success"] = $"Event '{evt.Name}' created successfully.";
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event {EventName}", evt.Name);
                ModelState.AddModelError("", "An error occurred while creating the event.");
            }
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

        // Remove AgeGroup from validation
        ModelState.Remove(nameof(evt.AgeGroup));

        if (ModelState.IsValid)
        {
            try
            {
                // Set AgeGroup based on ClassGroup
                evt.AgeGroup = evt.ClassGroup == EventClass.Open ? "0" : ((int)evt.ClassGroup).ToString();
                
                evt.UpdatedBy = User.Identity?.Name ?? "system";
                await _eventService.UpdateAsync(evt);
                
                _logger.LogInformation("Event {EventId} updated by {User}", evt.Id, evt.UpdatedBy);
                TempData["Success"] = $"Event '{evt.Name}' updated successfully.";
                
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _eventService.ExistsAsync(id))
                {
                    return NotFound();
                }
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event {EventId}", id);
                ModelState.AddModelError("", "An error occurred while updating the event.");
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

        evt.Status = status;
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddParticipant(Guid eventId, Guid participantId)
    {
        try
        {
            var createdBy = User.Identity?.Name ?? "system";
            await _eventService.AddParticipantToEventAsync(eventId, participantId, createdBy);
            
            TempData["Success"] = "Participant added to event successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding participant {ParticipantId} to event {EventId}", participantId, eventId);
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id = eventId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveParticipant(Guid eventId, Guid participantId)
    {
        try
        {
            await _eventService.RemoveParticipantFromEventAsync(eventId, participantId);
            TempData["Success"] = "Participant removed from event successfully.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing participant {ParticipantId} from event {EventId}", participantId, eventId);
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Details), new { id = eventId });
    }

    public async Task<IActionResult> EnterResults(Guid id)
    {
        try
        {
            var evt = await _eventService.GetByIdWithResultsAsync(id);
            if (evt == null)
            {
                return NotFound();
            }

            return View(evt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading enter results for event {EventId}", id);
            TempData["Error"] = "An error occurred while loading the results entry form.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnterResults(Guid id, List<ResultEntryModel> results, string action)
    {
        try
        {
            var evt = await _eventService.GetByIdAsync(id);
            if (evt == null)
            {
                return NotFound();
            }

            var updatedBy = User.Identity?.Name ?? "system";
            var pointsArray = evt.PointSystem.Split(',').Select(int.Parse).ToArray();

            // Process each result
            foreach (var resultEntry in results.Where(r => r.ResultId != Guid.Empty))
            {
                var result = await _context.Results.FindAsync(resultEntry.ResultId);
                if (result == null) continue;

                // Handle DNS/DNF/DQ
                if (resultEntry.IsDNS)
                {
                    result.ResultLabel = "DNS";
                    result.Placement = 0;
                    result.SpeedOrDistance = null;
                    result.Points = 0;
                }
                else if (resultEntry.IsDNF)
                {
                    result.ResultLabel = "DNF";
                    result.Placement = 0;
                    result.SpeedOrDistance = null;
                    result.Points = 0;
                }
                else if (resultEntry.IsDisqualified)
                {
                    result.ResultLabel = "DQ";
                    result.IsDisqualified = true;
                    result.Placement = 0;
                    result.SpeedOrDistance = null;
                    result.Points = 0;
                }
                else
                {
                    result.ResultLabel = null;
                    result.IsDisqualified = false;
                    
                    // Set speed or distance
                    if (resultEntry.SpeedOrDistance.HasValue)
                    {
                        result.SpeedOrDistance = resultEntry.SpeedOrDistance;
                    }
                    else if (resultEntry.Placement.HasValue)
                    {
                        result.Placement = resultEntry.Placement;
                    }
                }

                result.UpdatedAt = DateTime.Now;
                result.UpdatedBy = updatedBy;
            }

            // Calculate placements from speed/distance if needed
            var resultsWithValues = await _context.Results
                .Where(r => r.EventId == id && r.SpeedOrDistance.HasValue && !r.IsDisqualified)
                .Include(r => r.Participant)
                .ToListAsync();

            if (resultsWithValues.Any())
            {
                // Sort based on event type
                var sortedResults = evt.Type == EventType.Speed
                    ? resultsWithValues.OrderBy(r => r.SpeedOrDistance).ToList()
                    : resultsWithValues.OrderByDescending(r => r.SpeedOrDistance).ToList();

                for (int i = 0; i < sortedResults.Count; i++)
                {
                    sortedResults[i].Placement = i + 1;
                    sortedResults[i].Points = i < pointsArray.Length ? pointsArray[i] : 0;

                    // Check for new record
                    if (evt.Record.HasValue)
                    {
                        if (evt.Type == EventType.Speed && sortedResults[i].SpeedOrDistance < evt.Record)
                        {
                            sortedResults[i].IsNewRecord = true;
                            evt.Record = sortedResults[i].SpeedOrDistance;
                            evt.RecordHolder = sortedResults[i].Participant?.FullName;
                        }
                        else if (evt.Type == EventType.Distance && sortedResults[i].SpeedOrDistance > evt.Record)
                        {
                            sortedResults[i].IsNewRecord = true;
                            evt.Record = sortedResults[i].SpeedOrDistance;
                            evt.RecordHolder = sortedResults[i].Participant?.FullName;
                        }
                    }
                    else if (i == 0 && sortedResults[i].SpeedOrDistance.HasValue)
                    {
                        sortedResults[i].IsNewRecord = true;
                        evt.Record = sortedResults[i].SpeedOrDistance;
                        evt.RecordHolder = sortedResults[i].Participant?.FullName;
                    }
                }
            }

            // Assign points based on placement for results without speed/distance
            var resultsWithPlacement = await _context.Results
                .Where(r => r.EventId == id && r.Placement.HasValue && r.Placement > 0 && !r.SpeedOrDistance.HasValue)
                .ToListAsync();

            foreach (var result in resultsWithPlacement)
            {
                var placementIndex = result.Placement.Value - 1;
                result.Points = placementIndex < pointsArray.Length ? pointsArray[placementIndex] : 0;
            }

            await _context.SaveChangesAsync();

            // If action is publish, mark event as published and create event update
            if (action == "publish")
            {
                evt.IsPublished = true;
                evt.Status = EventStatus.Completed;
                await _context.SaveChangesAsync();

                // Create event update
                var update = new EventUpdate
                {
                    Id = Guid.NewGuid(),
                    EventId = evt.Id,
                    TournamentId = evt.TournamentId,
                    Message = $"Results published for {evt.Name}",
                    CreatedAt = DateTime.Now
                };
                _context.EventUpdates.Add(update);
                await _context.SaveChangesAsync();

                // Send real-time update
                await _hubContext.Clients.All.SendAsync("ReceiveEventUpdate", evt.Name, "Results Published");

                TempData["Success"] = "Results saved and published successfully.";
            }
            else
            {
                TempData["Success"] = "Results saved successfully.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving results for event {EventId}", id);
            TempData["Error"] = "An error occurred while saving results.";
            return RedirectToAction(nameof(EnterResults), new { id });
        }
    }

    private Task PopulateViewBagAsync(Guid tournamentId)
    {
        ViewBag.Divisions = new SelectList(Enum.GetValues(typeof(DivisionType)).Cast<DivisionType>().Select(d => new { Id = (int)d, Name = d.ToString() }), "Id", "Name");

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

        // Categories
        ViewBag.Categories = new List<SelectListItem>
        {
            new SelectListItem { Value = "Track", Text = "Track" },
            new SelectListItem { Value = "Field", Text = "Field" },
            new SelectListItem { Value = "Other", Text = "Other" }
        };

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

        return Task.CompletedTask;
    }
}
