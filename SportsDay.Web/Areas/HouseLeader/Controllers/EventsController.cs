using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsDay.Lib.Data;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;
using SportsDay.Lib.ViewModels;

namespace SportsDay.Web.Areas.HouseLeader.Controllers;

/// <summary>
/// Controller for managing event registrations for house participants
/// </summary>
public class EventsController : HouseLeaderBaseController
{
    private readonly IHouseLeaderService _houseLeaderService;
    private readonly IParticipantService _participantService;
    private readonly IEventService _eventService;
    private readonly ITournamentService _tournamentService;
    private readonly SportsDayDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<EventsController> _logger;

    public EventsController(
        IHouseLeaderService houseLeaderService,
        IParticipantService participantService,
        IEventService eventService,
        ITournamentService tournamentService,
        SportsDayDbContext context,
        UserManager<IdentityUser> userManager,
        ILogger<EventsController> logger)
    {
        _houseLeaderService = houseLeaderService;
        _participantService = participantService;
        _eventService = eventService;
        _tournamentService = tournamentService;
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// List all events with participant registration counts
    /// </summary>
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        /* if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        } */

        var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
        if (houseLeader == null)
        {
            TempData["Error"] = "You must be a house leader to view events.";
            return RedirectToAction("Register", "Dashboard");
        }

        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            TempData["Warning"] = "No active tournament at this time.";
            return RedirectToAction("Index", "Dashboard");
        }

        var events = await _eventService.GetByTournamentIdAsync(activeTournament.Id);
        var eventViewModels = new List<HouseLeaderEventViewModel>();

        foreach (var evt in events.OrderBy(e => e.EventNumber))
        {
            // Get participants registered for this event from this house
            var houseParticipants = await _context.Results
                .Include(r => r.Participant)
                .Where(r => r.EventId == evt.Id && r.HouseId == houseLeader.HouseId)
                .Select(r => r.Participant)
                .ToListAsync();

            // Get total participants for this event
            var totalParticipants = await _context.Results
                .Where(r => r.EventId == evt.Id)
                .CountAsync();

            eventViewModels.Add(new HouseLeaderEventViewModel
            {
                Event = evt,
                HouseParticipantCount = houseParticipants.Count,
                TotalParticipantCount = totalParticipants,
                HouseParticipants = houseParticipants
            });
        }

        var viewModel = new HouseLeaderEventsViewModel
        {
            HouseLeader = houseLeader,
            House = houseLeader.House!,
            ActiveTournament = activeTournament,
            Events = eventViewModels
        };

        return View(viewModel);
    }

    /// <summary>
    /// View event details with registered participants
    /// </summary>
    public async Task<IActionResult> Details(Guid id)
    {
        var userId = _userManager.GetUserId(User);
        /* if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        } */

        var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
        if (houseLeader == null)
        {
            TempData["Error"] = "You must be a house leader to view event details.";
            return RedirectToAction("Register", "Dashboard");
        }

        var evt = await _eventService.GetByIdWithResultsAsync(id);
        if (evt == null)
        {
            TempData["Error"] = "Event not found.";
            return RedirectToAction(nameof(Index));
        }

        // Get participants registered for this event from this house
        var houseParticipants = await _context.Results
            .Include(r => r.Participant)
            .Where(r => r.EventId == evt.Id && r.HouseId == houseLeader.HouseId)
            .Select(r => r.Participant)
            .ToListAsync();

        // Get total participants for this event
        var totalParticipants = await _context.Results
            .Where(r => r.EventId == evt.Id)
            .CountAsync();

        var eventViewModel = new HouseLeaderEventViewModel
        {
            Event = evt,
            HouseParticipantCount = houseParticipants.Count,
            TotalParticipantCount = totalParticipants,
            HouseParticipants = houseParticipants
        };

        ViewBag.HouseLeader = houseLeader;
        return View(eventViewModel);
    }

    /// <summary>
    /// GET: Register a participant to an event
    /// </summary>
    public async Task<IActionResult> RegisterParticipant(Guid id)
    {
        var userId = _userManager.GetUserId(User);
        /* if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        } */

        var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
        if (houseLeader == null)
        {
            TempData["Error"] = "You must be a house leader to register participants.";
            return RedirectToAction("Register", "Dashboard");
        }

        var evt = await _eventService.GetByIdAsync(id);
        if (evt == null)
        {
            TempData["Error"] = "Event not found.";
            return RedirectToAction(nameof(Index));
        }

        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            TempData["Error"] = "No active tournament.";
            return RedirectToAction("Index", "Dashboard");
        }

        // Get all participants from this house for the active tournament
        var allHouseParticipants = await _participantService.GetByTournamentAndHouseAsync(
            activeTournament.Id, houseLeader.HouseId);

        // Get participants already registered for this event
        var registeredParticipantIds = await _context.Results
            .Where(r => r.EventId == evt.Id && r.HouseId == houseLeader.HouseId)
            .Select(r => r.ParticipantId)
            .ToListAsync();

        // Filter to eligible participants (matching gender and class)
        var eligibleParticipants = allHouseParticipants
            .Where(p => p.GenderGroup == evt.GenderGroup || evt.GenderGroup == Lib.Enums.DivisionType.Open)
            .Where(p => p.EventClassGroup == evt.ClassGroup || evt.ClassGroup == Lib.Enums.EventClass.Open)
            .ToList();

        // Get available participants (not already registered)
        var availableParticipants = eligibleParticipants
            .Where(p => !registeredParticipantIds.Contains(p.Id))
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToList();

        // Get registered participants
        var registeredParticipants = eligibleParticipants
            .Where(p => registeredParticipantIds.Contains(p.Id))
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToList();

        var viewModel = new RegisterParticipantToEventViewModel
        {
            Event = evt,
            House = houseLeader.House!,
            AvailableParticipants = availableParticipants,
            RegisteredParticipants = registeredParticipants
        };

        return View(viewModel);
    }

    /// <summary>
    /// POST: Register a participant to an event
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterParticipant(Guid id, Guid participantId)
    {
        var userId = _userManager.GetUserId(User);
        /* if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        } */

        try
        {
            var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
            if (houseLeader == null)
            {
                TempData["Error"] = "You must be a house leader to register participants.";
                return RedirectToAction("Register", "Dashboard");
            }

            var evt = await _eventService.GetByIdAsync(id);
            if (evt == null)
            {
                TempData["Error"] = "Event not found.";
                return RedirectToAction(nameof(Index));
            }

            var participant = await _participantService.GetByIdAsync(participantId);
            if (participant == null)
            {
                TempData["Error"] = "Participant not found.";
                return RedirectToAction(nameof(RegisterParticipant), new { id });
            }

            // Verify participant belongs to house leader's house
            if (participant.HouseId != houseLeader.HouseId)
            {
                TempData["Error"] = "You can only register participants from your own house.";
                return RedirectToAction(nameof(RegisterParticipant), new { id });
            }

            // Check if participant is already registered
            var existingResult = await _context.Results
                .FirstOrDefaultAsync(r => r.EventId == id && r.ParticipantId == participantId);

            if (existingResult != null)
            {
                TempData["Warning"] = $"{participant.FullName} is already registered for this event.";
                return RedirectToAction(nameof(RegisterParticipant), new { id });
            }

            // Check participant limit
            if (evt.ParticipantLimit > 0)
            {
                var currentCount = await _context.Results
                    .Where(r => r.EventId == id && r.HouseId == houseLeader.HouseId)
                    .CountAsync();

                if (currentCount >= evt.ParticipantLimit)
                {
                    TempData["Error"] = $"This event has reached the maximum number of participants per house ({evt.ParticipantLimit}).";
                    return RedirectToAction(nameof(RegisterParticipant), new { id });
                }
            }

            // Create a result entry (registration)
            var result = new Result
            {
                Id = Guid.NewGuid(),
                EventId = id,
                ParticipantId = participantId,
                HouseId = houseLeader.HouseId,
                TournamentId = evt.TournamentId,
                CreatedBy = userId,
                CreatedAt = DateTime.Now
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{participant.FullName} has been registered for {evt.Name}.";
            _logger.LogInformation("House leader {UserId} registered participant {ParticipantId} for event {EventId}",
                userId, participantId, id);

            return RedirectToAction(nameof(RegisterParticipant), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering participant {ParticipantId} for event {EventId}", participantId, id);
            TempData["Error"] = "An error occurred while registering the participant. Please try again.";
            return RedirectToAction(nameof(RegisterParticipant), new { id });
        }
    }

    /// <summary>
    /// POST: Unregister a participant from an event
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnregisterParticipant(Guid eventId, Guid participantId)
    {
        var userId = _userManager.GetUserId(User);
       /*  if (string.IsNullOrEmpty(userId))
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        } */

        try
        {
            var houseLeader = await _houseLeaderService.GetByUserIdAsync(userId);
            if (houseLeader == null)
            {
                TempData["Error"] = "You must be a house leader to unregister participants.";
                return RedirectToAction("Register", "Dashboard");
            }

            var result = await _context.Results
                .Include(r => r.Participant)
                .Include(r => r.Event)
                .FirstOrDefaultAsync(r => r.EventId == eventId && r.ParticipantId == participantId);

            if (result == null)
            {
                TempData["Error"] = "Registration not found.";
                return RedirectToAction(nameof(RegisterParticipant), new { id = eventId });
            }

            // Verify participant belongs to house leader's house
            if (result.HouseId != houseLeader.HouseId)
            {
                TempData["Error"] = "You can only unregister participants from your own house.";
                return RedirectToAction(nameof(RegisterParticipant), new { id = eventId });
            }

            // Check if result has been entered (has placement or points)
            if (result.Placement.HasValue || result.Points > 0)
            {
                TempData["Error"] = "Cannot unregister a participant who has already received a result.";
                return RedirectToAction(nameof(RegisterParticipant), new { id = eventId });
            }

            var participantName = result.Participant?.FullName;
            var eventName = result.Event?.Name;

            _context.Results.Remove(result);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"{participantName} has been unregistered from {eventName}.";
            _logger.LogInformation("House leader {UserId} unregistered participant {ParticipantId} from event {EventId}",
                userId, participantId, eventId);

            return RedirectToAction(nameof(RegisterParticipant), new { id = eventId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unregistering participant {ParticipantId} from event {EventId}", participantId, eventId);
            TempData["Error"] = "An error occurred while unregistering the participant. Please try again.";
            return RedirectToAction(nameof(RegisterParticipant), new { id = eventId });
        }
    }
}