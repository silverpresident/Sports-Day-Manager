using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportsDay.Lib.Data;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Lib.Services;

/// <summary>
/// Service for managing event class groups with caching support.
/// </summary>
public class EventClassGroupService : IEventClassGroupService
{
    private readonly SportsDayDbContext _context;
    private readonly ILogger<EventClassGroupService> _logger;

    // Static cache for class groups with expiration
    private static List<EventClassGroup>? _cachedClassGroups;
    private static DateTime _cacheExpiration = DateTime.MinValue;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(1);
    private static readonly object _cacheLock = new object();

    public EventClassGroupService(SportsDayDbContext context, ILogger<EventClassGroupService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Gets class groups from cache or database.
    /// </summary>
    private async Task<List<EventClassGroup>> GetCachedClassGroupsAsync()
    {
        lock (_cacheLock)
        {
            if (_cachedClassGroups != null && DateTime.Now < _cacheExpiration)
            {
                _logger.LogDebug("Returning cached class groups");
                return _cachedClassGroups;
            }
        }

        _logger.LogInformation("Loading class groups from database");
        var classGroups = await _context.EventClassGroups
            .OrderBy(cg => cg.ClassGroupNumber)
            .AsNoTracking()
            .ToListAsync();

        lock (_cacheLock)
        {
            _cachedClassGroups = classGroups;
            _cacheExpiration = DateTime.Now.Add(CacheDuration);
        }

        return classGroups;
    }

    /// <summary>
    /// Clears the class group cache. Call this when class groups are modified.
    /// </summary>
    public void ClearCache()
    {
        lock (_cacheLock)
        {
            _cachedClassGroups = null;
            _cacheExpiration = DateTime.MinValue;
            _logger.LogInformation("Class group cache cleared");
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<EventClassGroup>> GetAllClassGroupsAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all class groups");
            return await GetCachedClassGroupsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all class groups");
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<EventClassGroup?> GetClassGroupByNumberAsync(int classGroupNumber)
    {
        try
        {
            _logger.LogInformation("Retrieving class group with number {ClassGroupNumber}", classGroupNumber);
            var classGroups = await GetCachedClassGroupsAsync();
            return classGroups.FirstOrDefault(cg => cg.ClassGroupNumber == classGroupNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving class group with number {ClassGroupNumber}", classGroupNumber);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<EventClassGroup?> GetClassGroupByAgeAsync(int age)
    {
        try
        {
            _logger.LogInformation("Determining class group for age {Age}", age);
            
            var classGroups = await GetCachedClassGroupsAsync();
            
            if (classGroups.Any())
            {
                // Use database values to determine class group
                // Find the class group with the smallest MaxParticipantAge that is >= age
                // If no match, return Open (ClassGroupNumber = 0)
                var matchingGroup = classGroups
                    .Where(cg => cg.MaxParticipantAge > 0 && age <= cg.MaxParticipantAge)
                    .OrderBy(cg => cg.MaxParticipantAge)
                    .FirstOrDefault();
                
                if (matchingGroup != null)
                {
                    _logger.LogDebug("Age {Age} matched to class group {ClassGroupNumber} using database values", 
                        age, matchingGroup.ClassGroupNumber);
                    return matchingGroup;
                }
                
                // Return Open class (0) if no match found
                var openClass = classGroups.FirstOrDefault(cg => cg.ClassGroupNumber == 0);
                if (openClass != null)
                {
                    _logger.LogDebug("Age {Age} matched to Open class using database values", age);
                    return openClass;
                }
            }
            
            // Fallback to hardcoded logic if database values not found
            _logger.LogWarning("Using fallback logic for age classification as database values not found");
            int classGroupNumber = GetClassGroupNumberByAgeFallback(age);
            return await GetClassGroupByNumberAsync(classGroupNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error determining class group for age {Age}", age);
            throw;
        }
    }

    /// <summary>
    /// Fallback method to determine class group by age when database values are not available.
    /// </summary>
    private int GetClassGroupNumberByAgeFallback(int age)
    {
        if (age <= 12) return 4; // Class 4
        if (age <= 14) return 3; // Class 3
        if (age <= 16) return 2; // Class 2
        if (age <= 19) return 1; // Class 1
        return 0; // Open
    }

    /// <inheritdoc/>
    public async Task<EventClassGroup?> GetClassGroupByDateOfBirthAsync(DateTime dateOfBirth, DateTime tournamentDate)
    {
        try
        {
            _logger.LogInformation("Determining class group for date of birth {DateOfBirth} as of {TournamentDate}", 
                dateOfBirth, tournamentDate);
            int age = CalculateAge(dateOfBirth, tournamentDate);
            return await GetClassGroupByAgeAsync(age);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error determining class group for date of birth {DateOfBirth} as of {TournamentDate}", 
                dateOfBirth, tournamentDate);
            throw;
        }
    }

    /// <inheritdoc/>
    public int CalculateAge(DateTime dateOfBirth, DateTime asOfDate)
    {
        int age = asOfDate.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > asOfDate.AddYears(-age)) age--;
        return age;
    }
}