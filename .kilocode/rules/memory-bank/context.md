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

## Current Focus

The project is in a stable state with core functionality working. The system can:
- Manage tournaments (create, activate, view)
- Track events with different statuses
- Register participants to houses
- Enter results with automatic point calculation
- Display live leaderboards by division
- Show announcements and event updates
- Broadcast real-time updates via SignalR

## Known Issues

1. Authorization attributes are commented out in controllers (for development ease)
2. Azure SignalR integration is commented out in Program.cs (using self-hosted SignalR)
3. Some navigation properties in models may have incorrect relationship configurations
4. Database initialization uses hardcoded admin credentials
5. No comprehensive error handling or logging implemented yet

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

- Need to implement proper service layer for all business logic
- Missing interfaces for several services
- Some controllers directly access DbContext instead of using services
- Incomplete separation of concerns in some areas
- Need to add proper ViewModel classes for complex views
- Missing anti-forgery token validation on some POST actions