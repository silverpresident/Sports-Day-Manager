using SportsDay.Lib.Enums;
using SportsDay.Lib.Models;

namespace SportsDay.Lib.Services.Interfaces;

/// <summary>
/// Service interface for managing event templates.
/// </summary>
public interface IEventTemplateService
{
    /// <summary>
    /// Gets all event templates.
    /// </summary>
    /// <returns>A collection of all event templates.</returns>
    Task<IEnumerable<EventTemplate>> GetAllAsync();

    /// <summary>
    /// Gets all active event templates.
    /// </summary>
    /// <returns>A collection of active event templates.</returns>
    Task<IEnumerable<EventTemplate>> GetActiveAsync();

    /// <summary>
    /// Gets an event template by its ID.
    /// </summary>
    /// <param name="id">The template ID.</param>
    /// <returns>The event template, or null if not found.</returns>
    Task<EventTemplate?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets event templates filtered by class group.
    /// </summary>
    /// <param name="classGroup">The class group to filter by.</param>
    /// <returns>A collection of matching event templates.</returns>
    Task<IEnumerable<EventTemplate>> GetByClassGroupAsync(EventClass classGroup);

    /// <summary>
    /// Gets event templates filtered by gender group.
    /// </summary>
    /// <param name="genderGroup">The gender group to filter by.</param>
    /// <returns>A collection of matching event templates.</returns>
    Task<IEnumerable<EventTemplate>> GetByGenderGroupAsync(DivisionType genderGroup);

    /// <summary>
    /// Gets event templates filtered by category (Track or Field).
    /// </summary>
    /// <param name="category">The category to filter by.</param>
    /// <returns>A collection of matching event templates.</returns>
    Task<IEnumerable<EventTemplate>> GetByCategoryAsync(string category);

    /// <summary>
    /// Creates a new event template.
    /// </summary>
    /// <param name="template">The template to create.</param>
    /// <returns>The created template.</returns>
    Task<EventTemplate> CreateAsync(EventTemplate template);

    /// <summary>
    /// Updates an existing event template.
    /// </summary>
    /// <param name="template">The template to update.</param>
    /// <returns>The updated template.</returns>
    Task<EventTemplate> UpdateAsync(EventTemplate template);

    /// <summary>
    /// Deletes an event template.
    /// </summary>
    /// <param name="id">The ID of the template to delete.</param>
    /// <returns>True if deleted, false if not found.</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Imports selected event templates into a tournament as events.
    /// </summary>
    /// <param name="templateIds">The IDs of templates to import.</param>
    /// <param name="tournamentId">The tournament to import into.</param>
    /// <param name="createdBy">The user performing the import.</param>
    /// <returns>A collection of created events.</returns>
    Task<IEnumerable<Event>> ImportToTournamentAsync(IEnumerable<Guid> templateIds, Guid tournamentId, string createdBy);

    /// <summary>
    /// Imports all active event templates into a tournament as events.
    /// </summary>
    /// <param name="tournamentId">The tournament to import into.</param>
    /// <param name="createdBy">The user performing the import.</param>
    /// <returns>A collection of created events.</returns>
    Task<IEnumerable<Event>> ImportAllToTournamentAsync(Guid tournamentId, string createdBy);

    /// <summary>
    /// Gets the next available event number for a tournament.
    /// </summary>
    /// <param name="tournamentId">The tournament ID.</param>
    /// <returns>The next available event number.</returns>
    Task<int> GetNextEventNumberAsync(Guid tournamentId);
}