# Developer Module

The Developer Module provides data generation and cleanup tools for testing the Sports Day Manager application. This module is **only available in DEBUG/Development mode** and should never be accessible in production environments.

## Overview

The Developer Module allows administrators to quickly generate test data for the active tournament, including:
- Events (from event templates)
- Participants (random names and ages)
- Participation records (assigning participants to events)
- Results (random placements and scores)

It also provides cleanup actions to remove test data.

## Accessing the Developer Module

1. Ensure the application is running in Development mode
2. Navigate to the Admin area
3. Click on "Developer" in the navigation menu (highlighted in yellow/warning color)

The Developer link only appears when `Environment.IsDevelopment()` returns true.

## Features

### Data Generation

The generation actions should be run in order for best results:

#### 1. Generate Events
- Imports all active event templates into the current tournament
- Creates Event records with proper event numbers
- Uses the `IEventTemplateService.ImportAllToTournamentAsync()` method

#### 2. Generate Participants
- Creates 2 random participants per house (12 total for 6 houses)
- Assigns random first and last names from predefined lists
- Sets random ages between 11 and 20 years
- Automatically determines:
  - Gender group (Boys/Girls randomly assigned)
  - Event class based on age:
    - Age ≤ 12: Class 4
    - Age ≤ 14: Class 3
    - Age ≤ 16: Class 2
    - Age ≤ 18: Class 1
    - Age > 18: Open
  - Age group (U13, U15, U17, U19, Open)

#### 3. Generate Participation
- Assigns eligible participants to events
- Matches participants based on:
  - Gender group (or Open events accept all)
  - Event class (or Open class accepts all)
- Randomly selects up to 6 participants per event
- Creates Result records with null placement (representing registration)

#### 4. Generate Results
- Fills in placements for participants without results
- Calculates points based on the event's point system
- Generates random speed/distance values:
  - Speed events: 10-60 seconds
  - Distance events: 1-10 meters
- Marks events as Completed

### Data Cleanup

**Warning:** These actions cannot be undone!

#### Delete All Results
- Removes all Result records for the active tournament
- Resets event completion status

#### Delete All Participation
- Removes all participation records (same as Delete All Results)
- Participants remain but are no longer assigned to events

#### Delete All Participants
- Removes all Participant records for the active tournament
- Cascade deletes associated results

#### Delete All Events
- Removes all Event records for the active tournament
- Cascade deletes associated results and event updates

## Statistics Dashboard

The Developer page displays current statistics for the active tournament:
- **Events**: Total number of events
- **Participants**: Total number of participants
- **Results**: Number of completed results (with placement)
- **Events w/ Results**: Events that have at least one result
- **Templates**: Number of active event templates
- **Houses**: Total number of houses

## Security

The Developer Module is protected by multiple layers:

1. **Compile-time protection**: The `DeveloperController` and `DeveloperService` are wrapped in `#if DEBUG` preprocessor directives
2. **Runtime protection**: The navigation link only appears when `Environment.IsDevelopment()` is true
3. **Service registration**: The `IDeveloperService` is only registered in DEBUG builds

## Technical Implementation

### Files Created

- `SportsDay.Lib/Services/Interfaces/IDeveloperService.cs` - Service interface
- `SportsDay.Lib/Services/DeveloperService.cs` - Service implementation
- `SportsDay.Web/Areas/Admin/Controllers/DeveloperController.cs` - Controller
- `SportsDay.Web/Areas/Admin/Views/Developer/Index.cshtml` - View

### Service Registration

The service is registered in `ServiceCollectionExtensions.cs`:

```csharp
#if DEBUG
    services.AddScoped<IDeveloperService, DeveloperService>();
#endif
```

### Dependencies

The `DeveloperService` depends on:
- `SportsDayDbContext` - Database access
- `IEventTemplateService` - Event template import functionality
- `ILogger<DeveloperService>` - Logging

## Usage Example

1. Create a new tournament and activate it
2. Navigate to Developer Tools
3. Click "Generate Events" to import event templates
4. Click "Generate Participants" to create test participants
5. Click "Generate Participation" to assign participants to events
6. Click "Generate Results" to create random results
7. View the results in the Events, Results, and Dashboard pages

## Best Practices

- Always run generation actions in order (Events → Participants → Participation → Results)
- Use cleanup actions before regenerating data to avoid duplicates
- Check the statistics after each action to verify success
- Review generated data in the respective admin pages

## Troubleshooting

### "No active tournament found"
- Ensure a tournament is created and activated before using Developer Tools

### Events not generating
- Check that event templates exist and are marked as active
- Verify the EventTemplates table has data

### Participants not being assigned to events
- Ensure events exist before generating participation
- Check that participant gender/class matches event requirements

### Results not generating
- Ensure participation records exist (participants assigned to events)
- Check that events have participants without placements