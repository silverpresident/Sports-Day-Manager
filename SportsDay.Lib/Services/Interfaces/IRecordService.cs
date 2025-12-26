using SportsDay.Lib.Enums;
using SportsDay.Lib.ViewModels;

namespace SportsDay.Lib.Services.Interfaces;

/// <summary>
/// Service interface for managing and retrieving event records.
/// Records come from two sources:
/// 1. Existing records stored in Events (historical records)
/// 2. New records from Results where IsNewRecord = true
/// </summary>
public interface IRecordService
{
    /// <summary>
    /// Gets the complete records index view model with both existing and new records.
    /// </summary>
    /// <param name="division">Optional division filter.</param>
    /// <param name="eventClass">Optional event class filter.</param>
    /// <param name="category">Optional category filter.</param>
    /// <param name="year">Optional year filter.</param>
    /// <param name="showNewRecordsOnly">Show only new records from results.</param>
    /// <param name="showExistingRecordsOnly">Show only existing records from events.</param>
    /// <returns>The records index view model.</returns>
    Task<RecordsIndexViewModel> GetRecordsIndexAsync(
        DivisionType? division = null,
        EventClass? eventClass = null,
        string? category = null,
        int? year = null,
        bool showNewRecordsOnly = false,
        bool showExistingRecordsOnly = false);

    /// <summary>
    /// Gets existing records from events in the active tournament.
    /// </summary>
    /// <returns>Collection of record view models from events.</returns>
    Task<IEnumerable<RecordViewModel>> GetExistingRecordsAsync();

    /// <summary>
    /// Gets new records from results in the active tournament.
    /// </summary>
    /// <returns>Collection of record view models from results with IsNewRecord = true.</returns>
    Task<IEnumerable<RecordViewModel>> GetNewRecordsAsync();

    /// <summary>
    /// Gets existing records filtered by division.
    /// </summary>
    /// <param name="division">The division to filter by.</param>
    /// <returns>Collection of record view models.</returns>
    Task<IEnumerable<RecordViewModel>> GetExistingRecordsByDivisionAsync(DivisionType division);

    /// <summary>
    /// Gets new records filtered by division.
    /// </summary>
    /// <param name="division">The division to filter by.</param>
    /// <returns>Collection of record view models.</returns>
    Task<IEnumerable<RecordViewModel>> GetNewRecordsByDivisionAsync(DivisionType division);

    /// <summary>
    /// Gets a specific record by event ID.
    /// </summary>
    /// <param name="eventId">The event ID.</param>
    /// <returns>The record view model, or null if not found.</returns>
    Task<RecordViewModel?> GetRecordByEventIdAsync(Guid eventId);

    /// <summary>
    /// Gets all unique categories that have records.
    /// </summary>
    /// <returns>Collection of category names.</returns>
    Task<IEnumerable<string>> GetCategoriesWithRecordsAsync();

    /// <summary>
    /// Gets all unique years that have records set.
    /// </summary>
    /// <returns>Collection of years.</returns>
    Task<IEnumerable<int>> GetYearsWithRecordsAsync();
}