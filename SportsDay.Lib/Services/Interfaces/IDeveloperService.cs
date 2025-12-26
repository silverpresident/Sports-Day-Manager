using SportsDay.Lib.Models;

namespace SportsDay.Lib.Services.Interfaces;

/// <summary>
/// Service interface for developer/testing operations.
/// Only available in debug mode.
/// </summary>
public interface IDeveloperService
{
    /// <summary>
    /// Generates events from event templates for the active tournament.
    /// </summary>
    /// <param name="tournamentId">The tournament ID.</param>
    /// <param name="createdBy">The user performing the operation.</param>
    /// <returns>Number of events created.</returns>
    Task<int> GenerateEventsAsync(Guid tournamentId, string createdBy);

    /// <summary>
    /// Generates random participants for each house in the active tournament.
    /// Creates 2 participants per house with random names and ages.
    /// </summary>
    /// <param name="tournamentId">The tournament ID.</param>
    /// <param name="createdBy">The user performing the operation.</param>
    /// <returns>Number of participants created.</returns>
    Task<int> GenerateParticipantsAsync(Guid tournamentId, string createdBy);

    /// <summary>
    /// Generates 2 random participants per house.
    /// </summary>
    /// <param name="tournamentId">The tournament ID.</param>
    /// <param name="createdBy">The user performing the operation.</param>
    /// <returns>Number of participants created.</returns>
    Task<int> Generate2ParticipantsPerHouseAsync(Guid tournamentId, string createdBy);

    /// <summary>
    /// Generates 1 participant from each division (gender) from each class per house.
    /// This creates a comprehensive set covering all combinations.
    /// </summary>
    /// <param name="tournamentId">The tournament ID.</param>
    /// <param name="createdBy">The user performing the operation.</param>
    /// <returns>Number of participants created.</returns>
    Task<int> GenerateComprehensiveParticipantsAsync(Guid tournamentId, string createdBy);

    /// <summary>
    /// Assigns participants to events (creates participation records).
    /// </summary>
    /// <param name="tournamentId">The tournament ID.</param>
    /// <param name="createdBy">The user performing the operation.</param>
    /// <returns>Number of participations created.</returns>
    Task<int> GenerateParticipationAsync(Guid tournamentId, string createdBy);

    /// <summary>
    /// Generates random results for events that don't have results.
    /// </summary>
    /// <param name="tournamentId">The tournament ID.</param>
    /// <param name="createdBy">The user performing the operation.</param>
    /// <returns>Number of results created.</returns>
    Task<int> GenerateResultsAsync(Guid tournamentId, string createdBy);

    /// <summary>
    /// Deletes all results for the active tournament.
    /// </summary>
    /// <param name="tournamentId">The tournament ID.</param>
    /// <returns>Number of results deleted.</returns>
    Task<int> DeleteAllResultsAsync(Guid tournamentId);

    /// <summary>
    /// Deletes all participation records for the active tournament.
    /// This removes participants from events but keeps the participants.
    /// </summary>
    /// <param name="tournamentId">The tournament ID.</param>
    /// <returns>Number of participations deleted.</returns>
    Task<int> DeleteAllParticipationAsync(Guid tournamentId);

    /// <summary>
    /// Deletes all participants for the active tournament.
    /// </summary>
    /// <param name="tournamentId">The tournament ID.</param>
    /// <returns>Number of participants deleted.</returns>
    Task<int> DeleteAllParticipantsAsync(Guid tournamentId);

    /// <summary>
    /// Deletes all events for the active tournament.
    /// </summary>
    /// <param name="tournamentId">The tournament ID.</param>
    /// <returns>Number of events deleted.</returns>
    Task<int> DeleteAllEventsAsync(Guid tournamentId);

    /// <summary>
    /// Gets statistics about the current tournament data.
    /// </summary>
    /// <param name="tournamentId">The tournament ID.</param>
    /// <returns>Statistics object with counts.</returns>
    Task<DeveloperStats> GetStatsAsync(Guid tournamentId);
}

/// <summary>
/// Statistics about tournament data for the developer dashboard.
/// </summary>
public class DeveloperStats
{
    public int EventCount { get; set; }
    public int ParticipantCount { get; set; }
    public int ResultCount { get; set; }
    public int EventsWithResultsCount { get; set; }
    public int EventsWithoutResultsCount { get; set; }
    public int EventTemplateCount { get; set; }
    public int HouseCount { get; set; }
}