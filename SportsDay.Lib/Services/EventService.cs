using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportsDay.Lib.Data;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;
using SportsDay.Lib.Enums;

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

    public async Task<IEnumerable<Participant>> GetEligibleParticipantsAsync(Guid eventId)
    {
        try
        {
            var evt = await _context.Events
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (evt == null)
            {
                _logger.LogWarning("Event not found: {EventId}", eventId);
                return Enumerable.Empty<Participant>();
            }

            // Get participants already registered for this event
            var registeredParticipantIds = await _context.Results
                .Where(r => r.EventId == eventId)
                .Select(r => r.ParticipantId)
                .ToListAsync();

            // Build query for eligible participants
            var query = _context.Participants
                .Where(p => p.TournamentId == evt.TournamentId);

            // For Open class events, all participants are eligible
            if (evt.ClassGroup != EventClass.Open)
            {
                // For specific class events, match the class
                query = query.Where(p => p.EventClassGroup == evt.ClassGroup);
            }

            // Match division (gender) unless it's Open division
            if (evt.GenderGroup != DivisionType.Open)
            {
                query = query.Where(p => p.GenderGroup == evt.GenderGroup);
            }

            // Exclude already registered participants
            query = query.Where(p => !registeredParticipantIds.Contains(p.Id));

            var eligibleParticipants = await query
                .Include(p => p.House)
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .AsNoTracking()
                .ToListAsync();

            _logger.LogInformation("Found {Count} eligible participants for event {EventId}",
                eligibleParticipants.Count, eventId);

            return eligibleParticipants;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting eligible participants for event {EventId}", eventId);
            throw;
        }
    }

    public async Task<Result> AddParticipantToEventAsync(Guid eventId, Guid participantId, string createdBy)
    {
        try
        {
            var evt = await _context.Events.FindAsync(eventId);
            if (evt == null)
            {
                throw new InvalidOperationException($"Event {eventId} not found");
            }

            var participant = await _context.Participants
                .Include(p => p.House)
                .FirstOrDefaultAsync(p => p.Id == participantId);
            
            if (participant == null)
            {
                throw new InvalidOperationException($"Participant {participantId} not found");
            }

            // Check if participant is already registered
            var existingResult = await _context.Results
                .FirstOrDefaultAsync(r => r.EventId == eventId && r.ParticipantId == participantId);

            if (existingResult != null)
            {
                throw new InvalidOperationException("Participant is already registered for this event");
            }

            // Create result entry (registration without placement)
            var result = new Result
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                ParticipantId = participantId,
                HouseId = participant.HouseId,
                TournamentId = evt.TournamentId,
                Placement = null,
                SpeedOrDistance = null,
                Points = 0,
                IsNewRecord = false,
                IsDisqualified = false,
                CreatedAt = DateTime.Now,
                CreatedBy = createdBy
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Participant {ParticipantId} added to event {EventId}",
                participantId, eventId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding participant {ParticipantId} to event {EventId}",
                participantId, eventId);
            throw;
        }
    }

    public async Task RemoveParticipantFromEventAsync(Guid eventId, Guid participantId)
    {
        try
        {
            var result = await _context.Results
                .FirstOrDefaultAsync(r => r.EventId == eventId && r.ParticipantId == participantId);

            if (result == null)
            {
                throw new InvalidOperationException("Participant is not registered for this event");
            }

            // Only allow removal if no results have been recorded
            if (result.Placement.HasValue || result.SpeedOrDistance.HasValue)
            {
                throw new InvalidOperationException("Cannot remove participant with recorded results");
            }

            _context.Results.Remove(result);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Participant {ParticipantId} removed from event {EventId}",
                participantId, eventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing participant {ParticipantId} from event {EventId}",
                participantId, eventId);
            throw;
        }
    }
}