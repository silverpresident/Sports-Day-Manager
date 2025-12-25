# Event Templates Feature

## Overview

Event Templates provide a way to create reusable event configurations that can be imported into tournaments. This feature allows administrators to maintain a library of standard track and field events that can be quickly added to any tournament, ensuring consistency and saving time during tournament setup.

## Key Features

- **Reusable Templates**: Create event templates once, use them across multiple tournaments
- **Record Tracking**: Store historical record information including record holder, year, and notes
- **Bulk Import**: Import multiple templates at once into a tournament
- **Flexible Selection**: Import by class, gender, or individual selection
- **Active/Inactive Status**: Control which templates are available for import

## Data Model

### EventTemplate Entity

The `EventTemplate` entity stores reusable event configurations independent of tournaments.

| Field | Type | Description |
|-------|------|-------------|
| Id | GUID | Primary key |
| Name | string (100) | Event name (e.g., "100m", "High Jump") |
| GenderGroup | DivisionType | Boys, Girls, or Open |
| ClassGroup | EventClass | Open, Class1, Class2, Class3, or Class4 |
| AgeGroup | string (10) | Age group identifier (e.g., "U14", "U16", "Open") |
| ParticipantMaxAge | int | Maximum age for participants (0 = no limit) |
| ParticipantLimit | int | Maximum participants per house (0 = no limit) |
| Category | string (50) | "Track" or "Field" |
| Type | EventType | Speed (lower is better) or Distance (higher is better) |
| Record | decimal? | Current record value |
| RecordHolder | string (100)? | Name of record holder |
| RecordSettingYear | int? | Year the record was set |
| RecordNote | string (500)? | Additional notes about the record |
| PointSystem | string (20) | Point allocation (e.g., "9,7,6,5,4,3,2,1") |
| IsActive | bool | Whether template is available for import |
| Description | string (500)? | Optional description |

### Relationship to Events

EventTemplates are **independent** of tournaments. When imported:
1. A new `Event` entity is created in the target tournament
2. Properties are copied from the template to the new event
3. The template remains unchanged for future use
4. Changes to templates do not affect previously created events

## User Interface

### Event Templates Index (`/Admin/EventTemplates`)

The main listing page displays all templates organized by class and gender:

- **Filtering**: Filter by class, gender, or category
- **Statistics**: Shows total, active, track, and field event counts
- **Actions**: View, edit, toggle active status, delete
- **Visual Indicators**: Active/inactive status, category badges

### Create Template (`/Admin/EventTemplates/Create`)

Form to create a new event template with sections for:
- Basic Information (name, status, description)
- Classification (gender, class, age group, category, type, points)
- Participant Settings (max age, participant limit)
- Record Information (record, holder, year, notes)

### Edit Template (`/Admin/EventTemplates/Edit/{id}`)

Similar to create form with pre-populated values and audit information.

### Template Details (`/Admin/EventTemplates/Details/{id}`)

Read-only view of all template properties with quick action buttons.

### Import Templates (`/Admin/EventTemplates/Import`)

Bulk import interface with:
- **Select All**: Select all templates at once
- **Select by Class**: Select all templates in a class
- **Select by Gender**: Select all templates for a gender within a class
- **Individual Selection**: Select specific templates
- **Import Selected**: Import only selected templates
- **Import All**: Import all active templates

## SQL Scripts

### Table Creation

File: `SQL/setup-event-templates.sql`

Creates the EventTemplates table with:
- All required columns
- Default values
- Indexes for common queries (ClassGroup, GenderGroup, Category, IsActive)

### Data Population

File: `SQL/populate-event-templates.sql`

Populates the table with 150+ standard Jamaican track and field events:

**Track Events:**
- Sprints: 100m, 200m, 400m
- Middle/Long Distance: 800m, 1500m, 3000m
- Hurdles: 70mH, 80mH, 100mH, 110mH, 400mH
- Relays: 4x100m, 4x400m, Medley

**Field Events:**
- Jumps: High Jump, Long Jump, Triple Jump
- Throws: Shot Put, Discus, Javelin
- Multi-events: Heptathlon, Decathlon

**Classes:**
- Class 4 (U14): Basic events
- Class 3 (U16): Expanded events including triple jump and javelin
- Class 2 (U18): Full event list with 3000m and 400m hurdles
- Class 1 (U20): Complete event list with multi-events
- Open: All ages eligible

## Service Layer

### IEventTemplateService Interface

```csharp
public interface IEventTemplateService
{
    Task<IEnumerable<EventTemplate>> GetAllAsync();
    Task<IEnumerable<EventTemplate>> GetActiveAsync();
    Task<EventTemplate?> GetByIdAsync(Guid id);
    Task<IEnumerable<EventTemplate>> GetByClassGroupAsync(EventClass classGroup);
    Task<IEnumerable<EventTemplate>> GetByGenderGroupAsync(DivisionType genderGroup);
    Task<IEnumerable<EventTemplate>> GetByCategoryAsync(string category);
    Task<EventTemplate> CreateAsync(EventTemplate template);
    Task<EventTemplate> UpdateAsync(EventTemplate template);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<Event>> ImportToTournamentAsync(IEnumerable<Guid> templateIds, Guid tournamentId, string createdBy);
    Task<IEnumerable<Event>> ImportAllToTournamentAsync(Guid tournamentId, string createdBy);
    Task<int> GetNextEventNumberAsync(Guid tournamentId);
}
```

### Key Methods

**ImportToTournamentAsync**: Imports selected templates into a tournament
- Retrieves templates by IDs
- Gets next available event number
- Creates Event entities from templates
- Saves to database
- Returns created events

**ImportAllToTournamentAsync**: Imports all active templates
- Retrieves all active templates
- Orders by class, gender, name
- Creates events for each template

## Usage Workflow

### Setting Up Templates

1. Navigate to Admin > Templates
2. Click "New Template" to create custom templates
3. Or run `SQL/populate-event-templates.sql` to load standard events
4. Review and adjust templates as needed
5. Toggle inactive any templates not needed

### Importing to Tournament

1. Ensure a tournament is active
2. Navigate to Admin > Templates > Import to Tournament
3. Select templates to import:
   - Use checkboxes for individual selection
   - Use class/gender headers for bulk selection
   - Use "Select All" for complete import
4. Click "Import Selected" or "Import All"
5. Events are created in the tournament
6. Navigate to Events to view imported events

### Managing Records

1. Edit a template to update record information
2. Fill in Record, Record Holder, Record Setting Year
3. Add notes in Record Note field
4. Save changes
5. Future imports will include updated record information

## Best Practices

1. **Keep Templates Updated**: Update record information when records are broken
2. **Use Descriptive Names**: Include distance/event type in name
3. **Set Appropriate Point Systems**: Use "9,7,6,5,4,3,2,1" for individual events, "12,10,9,8,7,6,1" for relays
4. **Deactivate Unused Templates**: Toggle inactive templates that aren't used
5. **Review Before Import**: Check selected templates before importing
6. **Document Records**: Use RecordNote for context about records

## Technical Notes

- Templates use GUID primary keys for consistency with other entities
- Enum values (GenderGroup, ClassGroup, Type) are stored as strings in the database
- The ToEvent() method on EventTemplate handles conversion to Event entity
- SignalR notifications are sent after successful imports
- All operations include logging via ILogger