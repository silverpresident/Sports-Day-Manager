using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Web.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ITournamentService _tournamentService;
        private readonly ILogger<EventController> _logger;

        public EventController(
            IEventService eventService,
            ITournamentService tournamentService,
            ILogger<EventController> logger)
        {
            _eventService = eventService;
            _tournamentService = tournamentService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var activeTournament = await _tournamentService.GetActiveTournamentAsync();
                if (activeTournament == null)
                {
                    _logger.LogWarning("No active tournament found when loading events");
                    return View(Enumerable.Empty<Event>());
                }

                var events = await _eventService.GetByActiveTournamentAsync();

                _logger.LogInformation("Loaded {EventCount} events for tournament {TournamentId}",
                    events.Count(), activeTournament.Id);

                ViewBag.ActiveTournament = activeTournament;
                return View(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading events");
                return View(Enumerable.Empty<Event>());
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var evt = await _eventService.GetByIdWithDetailsAsync(id);

                if (evt == null)
                {
                    _logger.LogWarning("Event not found with ID: {EventId}", id);
                    return NotFound();
                }

                _logger.LogInformation("Loaded event details for {EventName} (ID: {EventId})",
                    evt.Name, evt.Id);

                return View(evt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading event details for ID: {EventId}", id);
                return NotFound();
            }
        }
        public async Task<IActionResult> NextEvent(Guid id)
        {
            try
            {
                var nextEvent = await _eventService.GetNextEventAsync(id);

                if (nextEvent == null)
                {
                    _logger.LogInformation("No next event found after event ID: {EventId}", id);
                    // Stay on current event if no next event exists
                    return RedirectToAction(nameof(Details), new { id });
                }

                _logger.LogInformation("Navigating to next event {EventName} (ID: {EventId})",
                    nextEvent.Name, nextEvent.Id);

                return RedirectToAction(nameof(Details), new { id = nextEvent.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error navigating to next event from ID: {EventId}", id);
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        public async Task<IActionResult> PreviousEvent(Guid id)
        {
            try
            {
                var previousEvent = await _eventService.GetPreviousEventAsync(id);

                if (previousEvent == null)
                {
                    _logger.LogInformation("No previous event found before event ID: {EventId}", id);
                    // Stay on current event if no previous event exists
                    return RedirectToAction(nameof(Details), new { id });
                }

                _logger.LogInformation("Navigating to previous event {EventName} (ID: {EventId})",
                    previousEvent.Name, previousEvent.Id);

                return RedirectToAction(nameof(Details), new { id = previousEvent.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error navigating to previous event from ID: {EventId}", id);
                return RedirectToAction(nameof(Details), new { id });
            }
        }
    }
}
