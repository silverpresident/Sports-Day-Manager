using SportsDay.Lib.Models;

namespace SportsDay.Lib.Services.Interfaces;

/// <summary>
/// Service for managing event class groups.
/// </summary>
public interface IEventClassGroupService
{
    /// <summary>
    /// Gets all class groups.
    /// </summary>
    /// <returns>A collection of all class groups.</returns>
    Task<IEnumerable<EventClassGroup>> GetAllClassGroupsAsync();

    /// <summary>
    /// Gets a class group by its number.
    /// </summary>
    /// <param name="classGroupNumber">The class group number (0-4).</param>
    /// <returns>The class group, or null if not found.</returns>
    Task<EventClassGroup?> GetClassGroupByNumberAsync(int classGroupNumber);

    /// <summary>
    /// Gets the appropriate class group for a participant based on their age.
    /// </summary>
    /// <param name="age">The participant's age in years.</param>
    /// <returns>The appropriate class group.</returns>
    Task<EventClassGroup?> GetClassGroupByAgeAsync(int age);

    /// <summary>
    /// Gets the appropriate class group for a participant based on their date of birth and tournament date.
    /// </summary>
    /// <param name="dateOfBirth">The participant's date of birth.</param>
    /// <param name="tournamentDate">The tournament date.</param>
    /// <returns>The appropriate class group.</returns>
    Task<EventClassGroup?> GetClassGroupByDateOfBirthAsync(DateTime dateOfBirth, DateTime tournamentDate);

    /// <summary>
    /// Calculates a participant's age as of a specific date.
    /// </summary>
    /// <param name="dateOfBirth">The participant's date of birth.</param>
    /// <param name="asOfDate">The date to calculate age as of.</param>
    /// <returns>Age in years.</returns>
    int CalculateAge(DateTime dateOfBirth, DateTime asOfDate);
}