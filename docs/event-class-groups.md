# Event Class Groups

## Overview

The Event Class Groups feature provides a structured way to manage age-based classifications for events and participants in the Sports Day Manager system. This feature introduces a dedicated `EventClassGroup` entity and service to handle class group logic consistently across the application.

## Class Group Structure

The system supports 5 class groups numbered 0 to 4:

| Class Number | Name | Max Age | Description |
|--------------|------|---------|-------------|
| 0 | Open | No limit | Open class with no age restrictions |
| 1 | Class 1 | 19 | Participants aged 19 and under |
| 2 | Class 2 | 16 | Participants aged 16 and under |
| 3 | Class 3 | 14 | Participants aged 14 and under |
| 4 | Class 4 | 12 | Participants aged 12 and under |

## Age Classification Rules

Participants are automatically assigned to class groups based on their age:

- **Age ≤ 12**: Class 4
- **Age ≤ 14**: Class 3
- **Age ≤ 16**: Class 2
- **Age ≤ 19**: Class 1
- **Age > 19**: Open (Class 0)

## Database Schema

### EventClassGroups Table

```sql
CREATE TABLE EventClassGroups (
    ClassGroupNumber INT NOT NULL PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL,
    MaxParticipantAge INT NOT NULL,
    Description NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(256) NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(256) NULL
);
```

### ClassGroupNumber Field

The `ClassGroupNumber` field has been added to the following entities:

- **Event**: Links events to their class group
- **EventTemplate**: Links event templates to their class group
- **Participant**: Stores the participant's class group
- **TournamentHouseSummary**: Allows point aggregation by class group

## Service Layer

### IEventClassGroupService

The service interface provides the following methods:

```csharp
public interface IEventClassGroupService
{
    // Get all class groups
    Task<IEnumerable<EventClassGroup>> GetAllClassGroupsAsync();
    
    // Get class group by number (0-4)
    Task<EventClassGroup?> GetClassGroupByNumberAsync(int classGroupNumber);
    
    // Get class group for a participant based on age
    Task<EventClassGroup?> GetClassGroupByAgeAsync(int age);
    
    // Get class group based on date of birth and tournament date
    Task<EventClassGroup?> GetClassGroupByDateOfBirthAsync(DateTime dateOfBirth, DateTime tournamentDate);
    
    // Calculate age as of a specific date
    int CalculateAge(DateTime dateOfBirth, DateTime asOfDate);
}
```

### EventClassGroupService

The service implementation provides:

- **Database-driven classification**: Uses database values to determine class groups
- **Static caching**: Caches class groups for 1 hour to improve performance
- **Fallback logic**: Uses hardcoded logic if database values are not available
- Retrieval of all class groups
- Lookup by class group number
- Age-based class group determination
- Date of birth-based class group calculation (age reckoned as of tournament date)
- Age calculation utility method

#### Caching Strategy

The service implements a static cache with the following characteristics:

- **Cache Duration**: 1 hour
- **Thread-safe**: Uses lock mechanism for concurrent access
- **Automatic refresh**: Cache is automatically refreshed when expired
- **Manual clear**: `ClearCache()` method available for manual cache invalidation

#### Classification Logic

The service uses a two-tier approach for determining class groups:

1. **Primary (Database-driven)**: Queries the database for class groups and matches based on `MaxParticipantAge`
2. **Fallback (Hardcoded)**: If database values are not found, uses hardcoded age thresholds

This approach ensures the system works even if the database is not properly seeded, while allowing for flexible configuration through database values.

## Usage Examples

### Getting a Class Group by Age

```csharp
// Inject the service
private readonly IEventClassGroupService _classGroupService;

// Get class group for a 15-year-old participant
var classGroup = await _classGroupService.GetClassGroupByAgeAsync(15);
// Returns Class 2 (Max Age: 16)
```

### Getting a Class Group by Date of Birth

```csharp
// Calculate class group based on participant's DOB and tournament date
var dateOfBirth = new DateTime(2010, 5, 15);
var tournamentDate = new DateTime(2025, 3, 20);

var classGroup = await _classGroupService.GetClassGroupByDateOfBirthAsync(dateOfBirth, tournamentDate);
// Calculates age as of tournament date and returns appropriate class group
```

### Getting All Class Groups

```csharp
// Get all class groups for dropdown lists or selection
var allClassGroups = await _classGroupService.GetAllClassGroupsAsync();
```

## Database Setup

### Automatic Initialization (Recommended)

The EventClassGroups are automatically seeded when the application starts via the [`DbInitializer`](../SportsDay.Lib/Data/DbInitializer.cs). The initializer:

1. Checks if EventClassGroups already exist
2. If not, creates all 5 class groups with proper configuration
3. Logs the seeding process

This happens automatically during application startup, so no manual intervention is required.

### Manual Setup (Alternative)

If you need to manually set up the database or reset the class groups:

#### Step 1: Create Table and Add Columns

Run the migration script to create the `EventClassGroups` table and add `ClassGroupNumber` columns to existing tables:

```bash
# Execute the SQL script
sqlcmd -S (localdb)\mssqllocaldb -d SportsDay -i SQL/create-eventclassgroup-table.sql
```

Or use SQL Server Management Studio to execute [`SQL/create-eventclassgroup-table.sql`](../SQL/create-eventclassgroup-table.sql).

#### Step 2: Populate Class Group Data

Run the data population script to insert the 5 class groups:

```bash
# Execute the SQL script
sqlcmd -S (localdb)\mssqllocaldb -d SportsDay -i SQL/populate-eventclassgroup-data.sql
```

Or use SQL Server Management Studio to execute [`SQL/populate-eventclassgroup-data.sql`](../SQL/populate-eventclassgroup-data.sql).

## Integration Points

### Event Creation/Editing

When creating or editing events, set the `ClassGroupNumber` based on the selected `ClassGroup` enum:

```csharp
event.ClassGroupNumber = (int)event.ClassGroup;
```

### Participant Registration

When registering participants, calculate their class group based on age:

```csharp
var age = _classGroupService.CalculateAge(participant.DateOfBirth, tournament.Date);
var classGroup = await _classGroupService.GetClassGroupByAgeAsync(age);
participant.ClassGroupNumber = classGroup.ClassGroupNumber;
participant.EventClassGroup = (EventClass)classGroup.ClassGroupNumber;
```

### Tournament House Summaries

When aggregating points, group by `ClassGroupNumber` to get class-specific summaries:

```csharp
var summaries = await _context.TournamentHouseSummaries
    .Where(s => s.TournamentId == tournamentId)
    .GroupBy(s => new { s.HouseId, s.ClassGroupNumber })
    .Select(g => new {
        HouseId = g.Key.HouseId,
        ClassGroupNumber = g.Key.ClassGroupNumber,
        TotalPoints = g.Sum(s => s.Points)
    })
    .ToListAsync();
```

## Cache Management

The service uses a static cache to improve performance. The cache:

- Automatically expires after 1 hour
- Is thread-safe for concurrent access
- Can be manually cleared when class groups are modified

### Clearing the Cache

If you modify class groups in the database, clear the cache to ensure the service uses the updated values:

```csharp
// Inject the service
private readonly IEventClassGroupService _classGroupService;

// Clear the cache after modifying class groups
_classGroupService.ClearCache();
```

**Note**: The cache is static, so clearing it affects all instances of the service across the application.

## Service Registration

The `EventClassGroupService` is automatically registered in the dependency injection container via [`ServiceCollectionExtensions.AddSportsDayServices()`](../SportsDay.Lib/Extensions/ServiceCollectionExtensions.cs):

```csharp
services.AddScoped<IEventClassGroupService, EventClassGroupService>();
```

## Benefits

1. **Consistency**: Centralized logic for class group determination
2. **Flexibility**: Database-driven configuration allows easy modification of age thresholds
3. **Performance**: Static caching reduces database queries
4. **Reliability**: Fallback logic ensures system works even without database seeding
5. **Maintainability**: Single source of truth for class group data
6. **Type Safety**: Strongly-typed service methods with proper error handling
7. **Testability**: Service layer can be easily mocked for unit testing
8. **Logging**: Comprehensive logging for debugging and monitoring
9. **Automatic Initialization**: DbInitializer seeds class groups on application startup

## Future Enhancements

Potential improvements to consider:

- Admin UI for managing class groups (add, edit, delete) with automatic cache clearing
- Configurable age thresholds per tournament
- Historical tracking of class group changes
- Validation rules to ensure participants are in appropriate class groups
- Automatic class group updates when participant ages change
- Configurable cache duration via application settings
- Cache statistics and monitoring