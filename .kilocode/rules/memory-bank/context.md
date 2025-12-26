# Sports Day Manager - Current Context

## Current State

The Sports Day Manager project is a functional .NET 10.0 MVC application with core features implemented. The system is operational with basic tournament management, event tracking, and real-time updates via SignalR.

## Recent Work

- Initial project structure created with two projects: SportsDay.Lib and SportsDay.Web
- Database schema implemented with SQL Server support
- Core entities and models established
- Basic CRUD operations for tournaments, events, participants, and results
- SignalR integration for real-time updates
- Public dashboard with leaderboards and announcements
- Admin area with authentication (currently commented out for development)
- **Service layer implementation completed**:
  - Created HouseService with IHouseService interface
  - All services now registered via ServiceCollectionExtensions.AddSportsDayServices()
  - Both public and admin HouseControllers refactored to use service layer
  - Admin controllers now track CreatedBy/UpdatedBy with logged-in user's name
- **Event Templates feature implemented** (December 2025):
  - Created EventTemplate entity model with RecordSettingYear and RecordNote fields
  - EventTemplate stores reusable event configurations independent of tournaments
  - IEventTemplateService interface and EventTemplateService implementation
  - Admin EventTemplatesController with full CRUD operations
  - Import functionality to copy templates into active tournament
  - SQL scripts for table creation and population with Jamaican track and field events
  - Comprehensive UI with filtering, bulk selection, and import capabilities
- **Developer Module implemented** (December 2025):
  - IDeveloperService interface and DeveloperService implementation
  - Admin DeveloperController with data generation and cleanup actions
  - Developer dashboard view with statistics and action buttons
  - Only available in DEBUG/Development mode (compile-time and runtime protection)
  - Generate Events, Participants, Participation, and Results
  - Delete all Results, Participation, Participants, and Events
  - Documentation in docs/developer-module.md
- **EventService implemented** (December 2025):
  - IEventService interface and EventService implementation
  - Public EventController refactored to use service layer
  - Event Details view with full event information and results
- **House Details and Members views implemented** (December 2025):
  - Extended IHouseService with new methods for house details and rankings
  - HouseDetailsViewModel and HouseResultsViewModel created
  - Public HouseController updated with Details and Members actions
  - House Details view shows: house info, leaders, overall ranking, division rankings, leaderboard, event results
  - House Members view shows: all participants with expandable event participation details
  - House Index updated with links to Details and Members views

## Current Focus

The project is in a stable state with core functionality working. The system can:
- Manage tournaments (create, activate, view)
- Track events with different statuses
- Register participants to houses
- Enter results with automatic point calculation
- Display live leaderboards by division
- Show announcements and event updates
- Broadcast real-time updates via SignalR
- **Manage event templates**:
  - Create, edit, and delete reusable event templates
  - Import templates into tournaments (selective or bulk)
  - Pre-populated with 150+ Jamaican track and field events
  - Track historical records with year and notes
- **Developer tools** (DEBUG mode only):
  - Generate test data (events, participants, participation, results)
  - Clean up test data with bulk delete operations
  - View tournament statistics
- **View house details**:
  - House information with color and logo
  - House leaders list
  - Overall ranking and total points
  - Division-specific rankings (Boys, Girls, Open)
  - All house rankings leaderboard
  - Event results with placements and points
- **View house members**:
  - All participants registered to the house
  - Individual participant event participation
  - Points breakdown per participant
  - Filtering by division

## Known Issues

1. Authorization attributes are commented out in controllers (for development ease)
2. Azure SignalR integration is commented out in Program.cs (using self-hosted SignalR)
3. Some navigation properties in models may have incorrect relationship configurations
4. Database initialization uses hardcoded admin credentials

## Next Steps

Potential areas for enhancement:
- Enable and test authentication/authorization
- Implement comprehensive logging with ILogger
- Add error handling throughout the application
- Complete Azure deployment configuration
- Implement house leader self-registration feature
- Add participant self-registration functionality
- Create comprehensive unit and integration tests
- Enhance UI/UX with more interactive features
- Add data validation and business rule enforcement
- Implement TournamentHouseSummary calculation and updates
- Add records management page
- Complete all public-facing pages

## Technical Debt

- Missing anti-forgery token validation on some POST actions
- Consider creating additional services for Results and Announcements
- EventService created; consider similar pattern for Results, Announcements
- DeveloperService created for testing; consider expanding for more test scenarios
- HouseDetailsViewModel and HouseResultsViewModel created; consider similar ViewModels for other complex views