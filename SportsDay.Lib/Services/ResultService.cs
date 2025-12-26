using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportsDay.Lib.Data;
using SportsDay.Lib.Enums;
using SportsDay.Lib.Services.Interfaces;
using SportsDay.Lib.ViewModels;

namespace SportsDay.Lib.Services;

/// <summary>
/// Service for managing and retrieving event results.
/// Results represent participant placements and scores in events.
/// </summary>
public class ResultService : IResultService
{
    private readonly SportsDayDbContext _context;
    private readonly ITournamentService _tournamentService;
    private readonly ILogger<ResultService> _logger;

    public ResultService(
        SportsDayDbContext context,
        ITournamentService tournamentService,
        ILogger<ResultService> logger)
    {
        _context = context;
        _tournamentService = tournamentService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<ResultsIndexViewModel> GetResultsIndexAsync(
        DivisionType? division = null,
        EventClass? eventClass = null,
        string? category = null,
        int? houseId = null,
        Guid? eventId = null)
    {
        try
        {
            var activeTournament = await _tournamentService.GetActiveTournamentAsync();
            
            var results = await GetFilteredResultsAsync(division, eventClass, category, houseId, eventId);
            var categories = await GetCategoriesWithResultsAsync();
            var houses = await GetHousesWithResultsAsync();
            var events = await GetEventsWithResultsAsync();

            return new ResultsIndexViewModel
            {
                Results = results.ToList(),
                Categories = categories,
                Houses = houses,
                Events = events,
                SelectedDivision = division,
                SelectedClass = eventClass,
                SelectedCategory = category,
                SelectedHouseId = houseId,
                SelectedEventId = eventId,
                ActiveTournamentName = activeTournament?.Name
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting results index");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ResultViewModel>> GetAllResultsAsync()
    {
        try
        {
            var activeTournament = await _tournamentService.GetActiveTournamentAsync();
            if (activeTournament == null)
            {
                _logger.LogWarning("No active tournament found when loading results");
                return Enumerable.Empty<ResultViewModel>();
            }

            var results = await _context.Results
                .Include(r => r.Event)
                .Include(r => r.Participant)
                .Include(r => r.House)
                .Include(r => r.Tournament)
                .Where(r => r.TournamentId == activeTournament.Id && r.Placement.HasValue)
                .OrderByDescending(r => r.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

            return results.Select(MapToViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all results");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ResultViewModel>> GetResultsByDivisionAsync(DivisionType division)
    {
        try
        {
            var allResults = await GetAllResultsAsync();
            return allResults.Where(r => r.Division == division);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving results for division {Division}", division);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ResultViewModel>> GetResultsByHouseAsync(int houseId)
    {
        try
        {
            var allResults = await GetAllResultsAsync();
            return allResults.Where(r => r.HouseId == houseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving results for house {HouseId}", houseId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ResultViewModel>> GetResultsByEventAsync(Guid eventId)
    {
        try
        {
            var activeTournament = await _tournamentService.GetActiveTournamentAsync();
            if (activeTournament == null)
            {
                _logger.LogWarning("No active tournament found when loading results by event");
                return Enumerable.Empty<ResultViewModel>();
            }

            var results = await _context.Results
                .Include(r => r.Event)
                .Include(r => r.Participant)
                .Include(r => r.House)
                .Include(r => r.Tournament)
                .Where(r => r.TournamentId == activeTournament.Id && r.EventId == eventId && r.Placement.HasValue)
                .OrderBy(r => r.Placement)
                .AsNoTracking()
                .ToListAsync();

            return results.Select(MapToViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving results for event {EventId}", eventId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ResultViewModel?> GetResultByIdAsync(Guid resultId)
    {
        try
        {
            var result = await _context.Results
                .Include(r => r.Event)
                .Include(r => r.Participant)
                .Include(r => r.House)
                .Include(r => r.Tournament)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == resultId);

            if (result == null)
            {
                return null;
            }

            return MapToViewModel(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving result {ResultId}", resultId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetCategoriesWithResultsAsync()
    {
        try
        {
            var activeTournament = await _tournamentService.GetActiveTournamentAsync();
            if (activeTournament == null)
            {
                return Enumerable.Empty<string>();
            }

            var categories = await _context.Results
                .AsNoTracking()
                .Include(r => r.Event)
                .Where(r => r.TournamentId == activeTournament.Id && r.Placement.HasValue)
                .Select(r => r.Event.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return categories;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories with results");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<(int Id, string Name)>> GetHousesWithResultsAsync()
    {
        try
        {
            var activeTournament = await _tournamentService.GetActiveTournamentAsync();
            if (activeTournament == null)
            {
                return Enumerable.Empty<(int, string)>();
            }

            var houses = await _context.Results
                .AsNoTracking()
                .Include(r => r.House)
                .Where(r => r.TournamentId == activeTournament.Id && r.Placement.HasValue)
                .Select(r => new { r.House.Id, r.House.Name })
                .Distinct()
                .OrderBy(h => h.Name)
                .ToListAsync();

            return houses.Select(h => (h.Id, h.Name));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving houses with results");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<(Guid Id, string Name)>> GetEventsWithResultsAsync()
    {
        try
        {
            var activeTournament = await _tournamentService.GetActiveTournamentAsync();
            if (activeTournament == null)
            {
                return Enumerable.Empty<(Guid, string)>();
            }

            var events = await _context.Results
                .AsNoTracking()
                .Include(r => r.Event)
                .Where(r => r.TournamentId == activeTournament.Id && r.Placement.HasValue)
                .Select(r => new { r.Event.Id, r.Event.Name })
                .Distinct()
                .OrderBy(e => e.Name)
                .ToListAsync();

            return events.Select(e => (e.Id, e.Name));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving events with results");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<ResultViewModel>> GetTopResultsAsync(int count = 10)
    {
        try
        {
            var activeTournament = await _tournamentService.GetActiveTournamentAsync();
            if (activeTournament == null)
            {
                _logger.LogWarning("No active tournament found when loading top results");
                return Enumerable.Empty<ResultViewModel>();
            }

            var results = await _context.Results
                .Include(r => r.Event)
                .Include(r => r.Participant)
                .Include(r => r.House)
                .Include(r => r.Tournament)
                .Where(r => r.TournamentId == activeTournament.Id && r.Placement.HasValue && r.Placement <= 3)
                .OrderBy(r => r.Placement)
                .ThenByDescending(r => r.Points)
                .ThenByDescending(r => r.CreatedAt)
                .Take(count)
                .AsNoTracking()
                .ToListAsync();

            return results.Select(MapToViewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving top results");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<ResultStatisticsViewModel> GetResultStatisticsAsync()
    {
        try
        {
            var activeTournament = await _tournamentService.GetActiveTournamentAsync();
            if (activeTournament == null)
            {
                return new ResultStatisticsViewModel();
            }

            var results = await _context.Results
                .Include(r => r.Event)
                .Include(r => r.House)
                .Where(r => r.TournamentId == activeTournament.Id && r.Placement.HasValue)
                .AsNoTracking()
                .ToListAsync();

            var resultsByDivision = results
                .GroupBy(r => r.Event.GenderGroup)
                .ToDictionary(g => g.Key, g => g.Count());

            var pointsByHouse = results
                .GroupBy(r => r.House.Name)
                .ToDictionary(g => g.Key, g => g.Sum(r => r.Points));

            return new ResultStatisticsViewModel
            {
                TotalResults = results.Count,
                EventsWithResults = results.Select(r => r.EventId).Distinct().Count(),
                ParticipantsWithResults = results.Select(r => r.ParticipantId).Distinct().Count(),
                NewRecordsSet = results.Count(r => r.IsNewRecord),
                TotalPointsAwarded = results.Sum(r => r.Points),
                ResultsByDivision = resultsByDivision,
                PointsByHouse = pointsByHouse,
                ActiveTournamentName = activeTournament.Name
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving result statistics");
            throw;
        }
    }

    /// <summary>
    /// Gets filtered results based on the provided criteria.
    /// </summary>
    private async Task<IEnumerable<ResultViewModel>> GetFilteredResultsAsync(
        DivisionType? division,
        EventClass? eventClass,
        string? category,
        int? houseId,
        Guid? eventId)
    {
        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            return Enumerable.Empty<ResultViewModel>();
        }

        var query = _context.Results
            .Include(r => r.Event)
            .Include(r => r.Participant)
            .Include(r => r.House)
            .Include(r => r.Tournament)
            .Where(r => r.TournamentId == activeTournament.Id && r.Placement.HasValue)
            .AsNoTracking();

        if (division.HasValue)
        {
            query = query.Where(r => r.Event.GenderGroup == division.Value);
        }

        if (eventClass.HasValue)
        {
            query = query.Where(r => r.Event.ClassGroup == eventClass.Value);
        }

        if (!string.IsNullOrEmpty(category))
        {
            query = query.Where(r => r.Event.Category == category);
        }

        if (houseId.HasValue)
        {
            query = query.Where(r => r.HouseId == houseId.Value);
        }

        if (eventId.HasValue)
        {
            query = query.Where(r => r.EventId == eventId.Value);
        }

        var results = await query
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return results.Select(MapToViewModel);
    }

    /// <summary>
    /// Maps a Result entity to a ResultViewModel.
    /// </summary>
    private static ResultViewModel MapToViewModel(Models.Result result)
    {
        return new ResultViewModel
        {
            ResultId = result.Id,
            EventId = result.EventId,
            EventName = result.Event.Name,
            EventNumber = result.Event.EventNumber,
            ParticipantId = result.ParticipantId,
            ParticipantName = $"{result.Participant.FirstName} {result.Participant.LastName}",
            Division = result.Event.GenderGroup,
            ClassGroup = result.Event.ClassGroup,
            Category = result.Event.Category,
            EventType = result.Event.Type,
            Placement = result.Placement,
            SpeedOrDistance = result.SpeedOrDistance,
            Points = result.Points,
            IsNewRecord = result.IsNewRecord,
            HouseId = result.HouseId,
            HouseName = result.House.Name,
            HouseColor = result.House.Color,
            TournamentId = result.TournamentId,
            TournamentName = result.Tournament.Name,
            RecordedAt = result.CreatedAt
        };
    }
}