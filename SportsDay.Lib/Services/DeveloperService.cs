using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportsDay.Lib.Data;
using SportsDay.Lib.Enums;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Lib.Services;

/// <summary>
/// Service for developer/testing operations.
/// Only available in debug mode.
/// </summary>
public class DeveloperService : IDeveloperService
{
    private readonly SportsDayDbContext _context;
    private readonly IEventTemplateService _eventTemplateService;
    private readonly ILogger<DeveloperService> _logger;
    private readonly Random _random = new();

    // Sample first names for generating random participants
    private static readonly string[] FirstNames = new[]
    {
        "James", "John", "Michael", "David", "Robert", "William", "Richard", "Joseph", "Thomas", "Christopher",
        "Mary", "Patricia", "Jennifer", "Linda", "Elizabeth", "Barbara", "Susan", "Jessica", "Sarah", "Karen",
        "Andre", "Marcus", "Tyrone", "Dwayne", "Jamal", "Keisha", "Latoya", "Tamika", "Shaniqua", "Aaliyah",
        "Devon", "Malik", "Terrell", "Rashad", "Darnell", "Jasmine", "Destiny", "Imani", "Ebony", "Diamond"
    };

    // Sample last names for generating random participants
    private static readonly string[] LastNames = new[]
    {
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Miller", "Davis", "Garcia", "Rodriguez", "Wilson",
        "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin", "Lee", "Thompson", "White", "Harris",
        "Campbell", "Robinson", "Clarke", "Lewis", "Walker", "Hall", "Allen", "Young", "King", "Wright",
        "Scott", "Green", "Baker", "Adams", "Nelson", "Hill", "Mitchell", "Roberts", "Carter", "Phillips"
    };

    public DeveloperService(
        SportsDayDbContext context,
        IEventTemplateService eventTemplateService,
        ILogger<DeveloperService> logger)
    {
        _context = context;
        _eventTemplateService = eventTemplateService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<int> GenerateEventsAsync(Guid tournamentId, string createdBy)
    {
        _logger.LogInformation("Generating events for tournament {TournamentId}", tournamentId);

        var events = await _eventTemplateService.ImportAllToTournamentAsync(tournamentId, createdBy);
        var count = events.Count();

        _logger.LogInformation("Generated {Count} events for tournament {TournamentId}", count, tournamentId);
        return count;
    }

    /// <inheritdoc />
    public async Task<int> GenerateParticipantsAsync(Guid tournamentId, string createdBy)
    {
        _logger.LogInformation("Generating participants for tournament {TournamentId}", tournamentId);

        var houses = await _context.Houses.ToListAsync();
        var participantsCreated = 0;

        foreach (var house in houses)
        {
            // Create 2 participants per house
            for (int i = 0; i < 2; i++)
            {
                var isMale = _random.Next(2) == 0;
                var firstName = FirstNames[_random.Next(FirstNames.Length)];
                var lastName = LastNames[_random.Next(LastNames.Length)];
                var age = _random.Next(11, 21); // Ages 11-20
                var dateOfBirth = DateTime.Now.AddYears(-age).AddDays(_random.Next(-180, 180));

                // Determine class based on age
                var eventClass = age switch
                {
                    <= 12 => EventClass.Class4,
                    <= 14 => EventClass.Class3,
                    <= 16 => EventClass.Class2,
                    <= 18 => EventClass.Class1,
                    _ => EventClass.Open
                };

                var participant = new Participant
                {
                    Id = Guid.NewGuid(),
                    FirstName = firstName,
                    LastName = lastName,
                    HouseId = house.Id,
                    TournamentId = tournamentId,
                    GenderGroup = isMale ? DivisionType.Boys : DivisionType.Girls,
                    DateOfBirth = dateOfBirth,
                    AgeInYears = age,
                    AgeGroup = GetAgeGroup(age),
                    EventClassGroup = eventClass,
                    Points = 0,
                    CreatedAt = DateTime.Now,
                    CreatedBy = createdBy
                };

                _context.Participants.Add(participant);
                participantsCreated++;
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Generated {Count} participants for tournament {TournamentId}", participantsCreated, tournamentId);
        return participantsCreated;
    }

    /// <inheritdoc />
    public async Task<int> GenerateParticipationAsync(Guid tournamentId, string createdBy)
    {
        _logger.LogInformation("Generating participation for tournament {TournamentId}", tournamentId);

        // Get all events for the tournament
        var events = await _context.Events
            .Where(e => e.TournamentId == tournamentId)
            .ToListAsync();

        // Get all participants for the tournament
        var participants = await _context.Participants
            .Where(p => p.TournamentId == tournamentId)
            .ToListAsync();

        if (!events.Any() || !participants.Any())
        {
            _logger.LogWarning("No events or participants found for tournament {TournamentId}", tournamentId);
            return 0;
        }

        var participationCount = 0;

        foreach (var evt in events)
        {
            // Find eligible participants based on gender and class
            var eligibleParticipants = participants
                .Where(p => (evt.GenderGroup == DivisionType.Open || p.GenderGroup == evt.GenderGroup) &&
                           (evt.ClassGroup == EventClass.Open || p.EventClassGroup == evt.ClassGroup))
                .ToList();

            // Randomly select up to 6 participants per event (or all if fewer)
            var selectedParticipants = eligibleParticipants
                .OrderBy(_ => _random.Next())
                .Take(Math.Min(6, eligibleParticipants.Count))
                .ToList();

            foreach (var participant in selectedParticipants)
            {
                // Check if result already exists for this participant in this event
                var existingResult = await _context.Results
                    .AnyAsync(r => r.EventId == evt.Id && r.ParticipantId == participant.Id);

                if (!existingResult)
                {
                    // Create a placeholder result to represent participation
                    var result = new Result
                    {
                        Id = Guid.NewGuid(),
                        EventId = evt.Id,
                        ParticipantId = participant.Id,
                        HouseId = participant.HouseId,
                        TournamentId = tournamentId,
                        Placement = null, // No placement yet
                        SpeedOrDistance = null,
                        Points = 0,
                        IsNewRecord = false,
                        CreatedAt = DateTime.Now,
                        CreatedBy = createdBy
                    };

                    _context.Results.Add(result);
                    participationCount++;
                }
            }
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Generated {Count} participations for tournament {TournamentId}", participationCount, tournamentId);
        return participationCount;
    }

    /// <inheritdoc />
    public async Task<int> GenerateResultsAsync(Guid tournamentId, string createdBy)
    {
        _logger.LogInformation("Generating results for tournament {TournamentId}", tournamentId);

        // Get all events that have participants but no completed results
        var events = await _context.Events
            .Include(e => e.Results)
            .Where(e => e.TournamentId == tournamentId)
            .ToListAsync();

        var resultsCreated = 0;

        foreach (var evt in events)
        {
            // Get participants in this event without placement
            var participantsWithoutResults = evt.Results
                .Where(r => r.Placement == null)
                .OrderBy(_ => _random.Next())
                .ToList();

            if (!participantsWithoutResults.Any())
            {
                continue;
            }

            // Parse point system
            var points = evt.PointSystem.Split(',')
                .Select(int.Parse)
                .ToList();

            var placement = 1;
            foreach (var result in participantsWithoutResults)
            {
                result.Placement = placement;
                result.Points = placement <= points.Count ? points[placement - 1] : 0;

                // Generate random speed/distance based on event type
                if (evt.Type == EventType.Speed)
                {
                    // Generate time in seconds (e.g., 10-60 seconds for track events)
                    result.SpeedOrDistance = Math.Round((decimal)(_random.NextDouble() * 50 + 10), 2);
                }
                else
                {
                    // Generate distance in meters (e.g., 1-10 meters for field events)
                    result.SpeedOrDistance = Math.Round((decimal)(_random.NextDouble() * 9 + 1), 2);
                }

                result.UpdatedAt = DateTime.Now;
                result.UpdatedBy = createdBy;

                placement++;
                resultsCreated++;
            }

            // Update event status to completed
            evt.Status = EventStatus.Completed;
            evt.UpdatedAt = DateTime.Now;
            evt.UpdatedBy = createdBy;
        }

        await _context.SaveChangesAsync();
        _logger.LogInformation("Generated {Count} results for tournament {TournamentId}", resultsCreated, tournamentId);
        return resultsCreated;
    }

    /// <inheritdoc />
    public async Task<int> DeleteAllResultsAsync(Guid tournamentId)
    {
        _logger.LogInformation("Deleting all results for tournament {TournamentId}", tournamentId);

        var results = await _context.Results
            .Where(r => r.TournamentId == tournamentId)
            .ToListAsync();

        var count = results.Count;
        _context.Results.RemoveRange(results);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted {Count} results for tournament {TournamentId}", count, tournamentId);
        return count;
    }

    /// <inheritdoc />
    public async Task<int> DeleteAllParticipationAsync(Guid tournamentId)
    {
        _logger.LogInformation("Deleting all participation for tournament {TournamentId}", tournamentId);

        // Participation is represented by Results with no placement
        // For this implementation, we'll delete all results (which includes participation)
        return await DeleteAllResultsAsync(tournamentId);
    }

    /// <inheritdoc />
    public async Task<int> DeleteAllParticipantsAsync(Guid tournamentId)
    {
        _logger.LogInformation("Deleting all participants for tournament {TournamentId}", tournamentId);

        var participants = await _context.Participants
            .Where(p => p.TournamentId == tournamentId)
            .ToListAsync();

        var count = participants.Count;
        _context.Participants.RemoveRange(participants);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted {Count} participants for tournament {TournamentId}", count, tournamentId);
        return count;
    }

    /// <inheritdoc />
    public async Task<int> DeleteAllEventsAsync(Guid tournamentId)
    {
        _logger.LogInformation("Deleting all events for tournament {TournamentId}", tournamentId);

        var events = await _context.Events
            .Where(e => e.TournamentId == tournamentId)
            .ToListAsync();

        var count = events.Count;
        _context.Events.RemoveRange(events);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted {Count} events for tournament {TournamentId}", count, tournamentId);
        return count;
    }

    /// <inheritdoc />
    public async Task<DeveloperStats> GetStatsAsync(Guid tournamentId)
    {
        var eventCount = await _context.Events
            .CountAsync(e => e.TournamentId == tournamentId);

        var participantCount = await _context.Participants
            .CountAsync(p => p.TournamentId == tournamentId);

        var resultCount = await _context.Results
            .CountAsync(r => r.TournamentId == tournamentId && r.Placement != null);

        var eventsWithResults = await _context.Events
            .CountAsync(e => e.TournamentId == tournamentId && 
                           e.Results.Any(r => r.Placement != null));

        var eventTemplateCount = await _context.EventTemplates
            .CountAsync(et => et.IsActive);

        var houseCount = await _context.Houses.CountAsync();

        return new DeveloperStats
        {
            EventCount = eventCount,
            ParticipantCount = participantCount,
            ResultCount = resultCount,
            EventsWithResultsCount = eventsWithResults,
            EventsWithoutResultsCount = eventCount - eventsWithResults,
            EventTemplateCount = eventTemplateCount,
            HouseCount = houseCount
        };
    }

    private static string GetAgeGroup(int age)
    {
        return age switch
        {
            <= 12 => "U13",
            <= 14 => "U15",
            <= 16 => "U17",
            <= 18 => "U19",
            _ => "Open"
        };
    }
}