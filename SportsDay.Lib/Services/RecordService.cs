using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportsDay.Lib.Data;
using SportsDay.Lib.Enums;
using SportsDay.Lib.Services.Interfaces;
using SportsDay.Lib.ViewModels;

namespace SportsDay.Lib.Services;

/// <summary>
/// Service for managing and retrieving event records.
/// Records come from two sources:
/// 1. Existing records stored in Events (historical records)
/// 2. New records from Results where IsNewRecord = true
/// </summary>
public class RecordService : IRecordService
{
    private readonly SportsDayDbContext _context;
    private readonly ITournamentService _tournamentService;
    private readonly ILogger<RecordService> _logger;

    public RecordService(
        SportsDayDbContext context,
        ITournamentService tournamentService,
        ILogger<RecordService> logger)
    {
        _context = context;
        _tournamentService = tournamentService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<RecordsIndexViewModel> GetRecordsIndexAsync(
        DivisionType? division = null,
        EventClass? eventClass = null,
        string? category = null,
        int? year = null,
        bool showNewRecordsOnly = false,
        bool showExistingRecordsOnly = false)
    {
        try
        {
            var activeTournament = await _tournamentService.GetActiveTournamentAsync();
            
            var existingRecords = showNewRecordsOnly 
                ? Enumerable.Empty<RecordViewModel>() 
                : await GetFilteredExistingRecordsAsync(division, eventClass, category);
            
            var newRecords = showExistingRecordsOnly 
                ? Enumerable.Empty<RecordViewModel>() 
                : await GetFilteredNewRecordsAsync(division, eventClass, category);

            // Apply year filter if specified
            if (year.HasValue)
            {
                existingRecords = existingRecords.Where(r => r.YearSet == year.Value);
                newRecords = newRecords.Where(r => r.YearSet == year.Value);
            }

            var categories = await GetCategoriesWithRecordsAsync();
            var years = await GetYearsWithRecordsAsync();

            return new RecordsIndexViewModel
            {
                ExistingRecords = existingRecords.ToList(),
                NewRecords = newRecords.ToList(),
                Categories = categories,
                Years = years,
                SelectedDivision = division,
                SelectedClass = eventClass,
                SelectedCategory = category,
                SelectedYear = year,
                ShowNewRecordsOnly = showNewRecordsOnly,
                ShowExistingRecordsOnly = showExistingRecordsOnly,
                ActiveTournamentName = activeTournament?.Name
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting records index");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RecordViewModel>> GetExistingRecordsAsync()
    {
        try
        {
            var activeTournament = await _tournamentService.GetActiveTournamentAsync();
            if (activeTournament == null)
            {
                _logger.LogWarning("No active tournament found when loading existing records");
                return Enumerable.Empty<RecordViewModel>();
            }

            var events = await _context.Events
                .Where(e => e.TournamentId == activeTournament.Id && e.Record != null)
                .OrderBy(e => e.Category)
                .ThenBy(e => e.GenderGroup)
                .ThenBy(e => e.ClassGroup)
                .ThenBy(e => e.Name)
                .AsNoTracking()
                .ToListAsync();

            return events.Select(e => new RecordViewModel
            {
                EventId = e.Id,
                ResultId = null,
                EventName = e.Name,
                Division = e.GenderGroup,
                ClassGroup = e.ClassGroup,
                Category = e.Category,
                EventType = e.Type,
                RecordValue = e.Record,
                RecordHolder = e.RecordHolder,
                HouseName = null,
                HouseColor = null,
                YearSet = null, // Events don't store year set
                TournamentName = activeTournament.Name,
                IsNewRecord = false,
                DateSet = null,
                Notes = null
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving existing records");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RecordViewModel>> GetNewRecordsAsync()
    {
        try
        {
            var activeTournament = await _tournamentService.GetActiveTournamentAsync();
            if (activeTournament == null)
            {
                _logger.LogWarning("No active tournament found when loading new records");
                return Enumerable.Empty<RecordViewModel>();
            }

            var results = await _context.Results
                .Include(r => r.Event)
                .Include(r => r.Participant)
                .Include(r => r.House)
                .Include(r => r.Tournament)
                .Where(r => r.TournamentId == activeTournament.Id && r.IsNewRecord)
                .OrderByDescending(r => r.CreatedAt)
                .AsNoTracking()
                .ToListAsync();

            return results.Select(r => new RecordViewModel
            {
                EventId = r.EventId,
                ResultId = r.Id,
                EventName = r.Event.Name,
                Division = r.Event.GenderGroup,
                ClassGroup = r.Event.ClassGroup,
                Category = r.Event.Category,
                EventType = r.Event.Type,
                RecordValue = r.SpeedOrDistance,
                RecordHolder = $"{r.Participant.FirstName} {r.Participant.LastName}",
                HouseName = r.House.Name,
                HouseColor = r.House.Color,
                YearSet = r.CreatedAt.Year,
                TournamentName = r.Tournament.Name,
                IsNewRecord = true,
                DateSet = r.CreatedAt,
                Notes = $"Set during {r.Tournament.Name}"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving new records");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RecordViewModel>> GetExistingRecordsByDivisionAsync(DivisionType division)
    {
        try
        {
            var allRecords = await GetExistingRecordsAsync();
            return allRecords.Where(r => r.Division == division);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving existing records for division {Division}", division);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RecordViewModel>> GetNewRecordsByDivisionAsync(DivisionType division)
    {
        try
        {
            var allRecords = await GetNewRecordsAsync();
            return allRecords.Where(r => r.Division == division);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving new records for division {Division}", division);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<RecordViewModel?> GetRecordByEventIdAsync(Guid eventId)
    {
        try
        {
            var evt = await _context.Events
                .Include(e => e.Tournament)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (evt == null || evt.Record == null)
            {
                return null;
            }

            // Check if there's a new record for this event
            var newRecord = await _context.Results
                .Include(r => r.Participant)
                .Include(r => r.House)
                .Include(r => r.Tournament)
                .Where(r => r.EventId == eventId && r.IsNewRecord)
                .OrderByDescending(r => r.CreatedAt)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (newRecord != null)
            {
                return new RecordViewModel
                {
                    EventId = evt.Id,
                    ResultId = newRecord.Id,
                    EventName = evt.Name,
                    Division = evt.GenderGroup,
                    ClassGroup = evt.ClassGroup,
                    Category = evt.Category,
                    EventType = evt.Type,
                    RecordValue = newRecord.SpeedOrDistance,
                    RecordHolder = $"{newRecord.Participant.FirstName} {newRecord.Participant.LastName}",
                    HouseName = newRecord.House.Name,
                    HouseColor = newRecord.House.Color,
                    YearSet = newRecord.CreatedAt.Year,
                    TournamentName = newRecord.Tournament.Name,
                    IsNewRecord = true,
                    DateSet = newRecord.CreatedAt,
                    Notes = $"Set during {newRecord.Tournament.Name}"
                };
            }

            return new RecordViewModel
            {
                EventId = evt.Id,
                ResultId = null,
                EventName = evt.Name,
                Division = evt.GenderGroup,
                ClassGroup = evt.ClassGroup,
                Category = evt.Category,
                EventType = evt.Type,
                RecordValue = evt.Record,
                RecordHolder = evt.RecordHolder,
                HouseName = null,
                HouseColor = null,
                YearSet = null,
                TournamentName = evt.Tournament?.Name,
                IsNewRecord = false,
                DateSet = null,
                Notes = null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving record for event {EventId}", eventId);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetCategoriesWithRecordsAsync()
    {
        try
        {
            var activeTournament = await _tournamentService.GetActiveTournamentAsync();
            if (activeTournament == null)
            {
                return Enumerable.Empty<string>();
            }

            var eventCategories = await _context.Events
                .AsNoTracking()
                .Where(e => e.TournamentId == activeTournament.Id && e.Record != null)
                .Select(e => e.Category)
                .Distinct()
                .ToListAsync();

            var resultCategories = await _context.Results
                .AsNoTracking()
                .Include(r => r.Event)
                .Where(r => r.TournamentId == activeTournament.Id && r.IsNewRecord)
                .Select(r => r.Event.Category)
                .Distinct()
                .ToListAsync();

            return eventCategories.Union(resultCategories).Distinct().OrderBy(c => c);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving categories with records");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<int>> GetYearsWithRecordsAsync()
    {
        try
        {
            var activeTournament = await _tournamentService.GetActiveTournamentAsync();
            if (activeTournament == null)
            {
                return Enumerable.Empty<int>();
            }

            // Get years from new records
            var years = await _context.Results
                .AsNoTracking()
                .Where(r => r.TournamentId == activeTournament.Id && r.IsNewRecord)
                .Select(r => r.CreatedAt.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            // Add current year if there are existing records
            var hasExistingRecords = await _context.Events
                .AnyAsync(e => e.TournamentId == activeTournament.Id && e.Record != null);

            if (hasExistingRecords && !years.Contains(DateTime.Now.Year))
            {
                years.Insert(0, DateTime.Now.Year);
            }

            return years;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving years with records");
            throw;
        }
    }

    /// <summary>
    /// Gets filtered existing records.
    /// </summary>
    private async Task<IEnumerable<RecordViewModel>> GetFilteredExistingRecordsAsync(
        DivisionType? division, EventClass? eventClass, string? category)
    {
        var records = await GetExistingRecordsAsync();

        if (division.HasValue)
        {
            records = records.Where(r => r.Division == division.Value);
        }

        if (eventClass.HasValue)
        {
            records = records.Where(r => r.ClassGroup == eventClass.Value);
        }

        if (!string.IsNullOrEmpty(category))
        {
            records = records.Where(r => r.Category == category);
        }

        return records;
    }

    /// <summary>
    /// Gets filtered new records.
    /// </summary>
    private async Task<IEnumerable<RecordViewModel>> GetFilteredNewRecordsAsync(
        DivisionType? division, EventClass? eventClass, string? category)
    {
        var records = await GetNewRecordsAsync();

        if (division.HasValue)
        {
            records = records.Where(r => r.Division == division.Value);
        }

        if (eventClass.HasValue)
        {
            records = records.Where(r => r.ClassGroup == eventClass.Value);
        }

        if (!string.IsNullOrEmpty(category))
        {
            records = records.Where(r => r.Category == category);
        }

        return records;
    }
}