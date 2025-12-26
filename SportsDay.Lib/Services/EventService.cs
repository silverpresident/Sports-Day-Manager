using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportsDay.Lib.Data;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Lib.Services;

public class EventService : IEventService
{
    private readonly SportsDayDbContext _context;
    private readonly ITournamentService _tournamentService;
    private readonly ILogger<EventService> _logger;

    public EventService(
        SportsDayDbContext context,
        ITournamentService tournamentService,
        ILogger<EventService> logger)
    {
        _context = context;
        _tournamentService = tournamentService;
        _logger = logger;
    }

    public async Task<Event?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving event {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Event>> GetAllAsync()
    {
        try
        {
            return await _context.Events
                .AsNoTracking()
                .OrderBy(e => e.ScheduledTime)
                .ThenBy(e => e.EventNumber)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all events");
            throw;
        }
    }

    public async Task<IEnumerable<Event>> GetByTournamentIdAsync(Guid tournamentId)
    {
        try
        {
            return await _context.Events
                .Where(e => e.TournamentId == tournamentId)
                .OrderBy(e => e.ScheduledTime)
                .ThenBy(e => e.EventNumber)
                .AsNoTracking()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving events for tournament {TournamentId}", tournamentId);
            throw;
        }
    }

    public async Task<IEnumerable<Event>> GetByActiveTournamentAsync()
    {
        try
        {
            var activeTournament = await _tournamentService.GetActiveTournamentAsync();
            if (activeTournament == null)
            {
                _logger.LogWarning("No active tournament found when loading events");
                return Enumerable.Empty<Event>();
            }

            return await GetByTournamentIdAsync(activeTournament.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving events for active tournament");
            throw;
        }
    }

    public async Task<Event?> GetByIdWithResultsAsync(Guid id)
    {
        try
        {
            return await _context.Events
                .Include(e => e.Results)
                    .ThenInclude(r => r.Participant)
                .Include(e => e.Results)
                    .ThenInclude(r => r.House)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving event {Id} with results", id);
            throw;
        }
    }

    public async Task<Event?> GetByIdWithDetailsAsync(Guid id)
    {
        try
        {
            return await _context.Events
                .Include(e => e.Tournament)
                .Include(e => e.Results)
                    .ThenInclude(r => r.Participant)
                .Include(e => e.Results)
                    .ThenInclude(r => r.House)
                .Include(e => e.Updates)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving event {Id} with details", id);
            throw;
        }
    }

    public async Task<Event> CreateAsync(Event evt)
    {
        try
        {
            evt.Id = Guid.NewGuid();
            evt.CreatedAt = DateTime.Now;
            evt.CreatedBy = evt.CreatedBy ?? "system";

            if (evt.EventNumber == 0)
            {
                evt.EventNumber = await GetNextEventNumberAsync(evt.TournamentId);
            }

            _context.Events.Add(evt);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Event {Name} created with ID {Id}", evt.Name, evt.Id);

            return evt;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating event {Name}", evt.Name);
            throw;
        }
    }

    public async Task UpdateAsync(Event evt)
    {
        try
        {
            evt.UpdatedAt = DateTime.Now;
            evt.UpdatedBy = evt.UpdatedBy ?? "system";

            _context.Events.Update(evt);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Event {Id} updated", evt.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating event {Id}", evt.Id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        try
        {
            var evt = await GetByIdAsync(id);
            if (evt == null)
            {
                throw new InvalidOperationException($"Event {id} not found");
            }

            _context.Events.Remove(evt);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Event {Id} deleted", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting event {Id}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        try
        {
            return await _context.Events.AnyAsync(e => e.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if event {Id} exists", id);
            throw;
        }
    }

    public async Task<int> GetNextEventNumberAsync(Guid tournamentId)
    {
        try
        {
            var lastEvent = await _context.Events
                .Where(e => e.TournamentId == tournamentId)
                .OrderByDescending(e => e.EventNumber)
                .FirstOrDefaultAsync();

            return (lastEvent?.EventNumber ?? 0) + 1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting next event number for tournament {TournamentId}", tournamentId);
            throw;
        }
    }

    public async Task<Event?> GetNextEventAsync(Guid currentEventId)
    {
        try
        {
            var currentEvent = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == currentEventId);

            if (currentEvent == null)
            {
                _logger.LogWarning("Current event not found: {EventId}", currentEventId);
                return null;
            }

            // Get the next event by event number within the same tournament
            var nextEvent = await _context.Events
                .Where(e => e.TournamentId == currentEvent.TournamentId
                         && e.EventNumber > currentEvent.EventNumber)
                .OrderBy(e => e.EventNumber)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return nextEvent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting next event for {EventId}", currentEventId);
            throw;
        }
    }

    public async Task<Event?> GetPreviousEventAsync(Guid currentEventId)
    {
        try
        {
            var currentEvent = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == currentEventId);

            if (currentEvent == null)
            {
                _logger.LogWarning("Current event not found: {EventId}", currentEventId);
                return null;
            }

            // Get the previous event by event number within the same tournament
            var previousEvent = await _context.Events
                .Where(e => e.TournamentId == currentEvent.TournamentId
                         && e.EventNumber < currentEvent.EventNumber)
                .OrderByDescending(e => e.EventNumber)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return previousEvent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting previous event for {EventId}", currentEventId);
            throw;
        }
    }
}