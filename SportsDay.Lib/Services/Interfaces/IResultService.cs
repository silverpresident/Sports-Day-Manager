using SportsDay.Lib.Enums;
using SportsDay.Lib.ViewModels;

namespace SportsDay.Lib.Services.Interfaces;

/// <summary>
/// Service interface for managing and retrieving event results.
/// Results represent participant placements and scores in events.
/// </summary>
public interface IResultService
{
    /// <summary>
    /// Gets the complete results index view model with filtering options.
    /// </summary>
    /// <param name="division">Optional division filter.</param>
    /// <param name="eventClass">Optional event class filter.</param>
    /// <param name="category">Optional category filter.</param>
    /// <param name="houseId">Optional house filter.</param>
    /// <param name="eventId">Optional event filter.</param>
    /// <returns>The results index view model.</returns>
    Task<ResultsIndexViewModel> GetResultsIndexAsync(
        DivisionType? division = null,
        EventClass? eventClass = null,
        string? category = null,
        int? houseId = null,
        Guid? eventId = null);

    /// <summary>
    /// Gets all results for the active tournament.
    /// </summary>
    /// <returns>Collection of result view models.</returns>
    Task<IEnumerable<ResultViewModel>> GetAllResultsAsync();

    /// <summary>
    /// Gets results filtered by division.
    /// </summary>
    /// <param name="division">The division to filter by.</param>
    /// <returns>Collection of result view models.</returns>
    Task<IEnumerable<ResultViewModel>> GetResultsByDivisionAsync(DivisionType division);

    /// <summary>
    /// Gets results filtered by house.
    /// </summary>
    /// <param name="houseId">The house ID to filter by.</param>
    /// <returns>Collection of result view models.</returns>
    Task<IEnumerable<ResultViewModel>> GetResultsByHouseAsync(int houseId);

    /// <summary>
    /// Gets results filtered by event.
    /// </summary>
    /// <param name="eventId">The event ID to filter by.</param>
    /// <returns>Collection of result view models.</returns>
    Task<IEnumerable<ResultViewModel>> GetResultsByEventAsync(Guid eventId);

    /// <summary>
    /// Gets a specific result by ID.
    /// </summary>
    /// <param name="resultId">The result ID.</param>
    /// <returns>The result view model, or null if not found.</returns>
    Task<ResultViewModel?> GetResultByIdAsync(Guid resultId);

    /// <summary>
    /// Gets all unique categories that have results.
    /// </summary>
    /// <returns>Collection of category names.</returns>
    Task<IEnumerable<string>> GetCategoriesWithResultsAsync();

    /// <summary>
    /// Gets all houses that have results.
    /// </summary>
    /// <returns>Collection of house ID and name pairs.</returns>
    Task<IEnumerable<(int Id, string Name)>> GetHousesWithResultsAsync();

    /// <summary>
    /// Gets all events that have results.
    /// </summary>
    /// <returns>Collection of event ID and name pairs.</returns>
    Task<IEnumerable<(Guid Id, string Name)>> GetEventsWithResultsAsync();

    /// <summary>
    /// Gets the top results (podium finishers) for the active tournament.
    /// </summary>
    /// <param name="count">Number of top results to return.</param>
    /// <returns>Collection of result view models.</returns>
    Task<IEnumerable<ResultViewModel>> GetTopResultsAsync(int count = 10);

    /// <summary>
    /// Gets results summary statistics for the active tournament.
    /// </summary>
    /// <returns>Result statistics view model.</returns>
    Task<ResultStatisticsViewModel> GetResultStatisticsAsync();
}