# Sports Day Manager - Architecture

## System Architecture

Sports Day Manager follows a layered architecture pattern with clear separation between business logic and presentation layers.

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────┐
│                    Client Browser                        │
│  (Public Dashboard, Admin Interface, SignalR Client)    │
└─────────────────────────────────────────────────────────┘
                          ↕ HTTPS
┌─────────────────────────────────────────────────────────┐
│              SportsDay.Web (ASP.NET Core MVC)           │
│  ┌─────────────────────────────────────────────────┐   │
│  │  Controllers (Public & Admin Areas)              │   │
│  │  - DashboardController                           │   │
│  │  - EventController, HouseController              │   │
│  │  - Admin/EventsController, ResultsController     │   │
│  └─────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────┐   │
│  │  SignalR Hub (SportsHub)                         │   │
│  │  - Real-time event updates                       │   │
│  │  - Result broadcasts                             │   │
│  │  - Announcement notifications                    │   │
│  └─────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────┐   │
│  │  Views (Razor Pages)                             │   │
│  │  - Public views, Admin area views                │   │
│  │  - Partial views for components                  │   │
│  └─────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                          ↕
┌─────────────────────────────────────────────────────────┐
│           SportsDay.Lib (Business Logic Layer)          │
│  ┌─────────────────────────────────────────────────┐   │
│  │  Services                                        │   │
│  │  - ITournamentService / TournamentService        │   │
│  │  - IHouseService / HouseService                  │   │
│  │  - IHouseLeaderService / HouseLeaderService      │   │
│  │  - IParticipantService / ParticipantService      │   │
│  │  - IEventTemplateService / EventTemplateService  │   │
│  │  - IEventService / EventService                  │   │
│  │  - IDeveloperService / DeveloperService (DEBUG)  │   │
│  │  - IDashboardService / DashboardService          │   │
│  └─────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────┐   │
│  │  Extensions                                      │   │
│  │  - ServiceCollectionExtensions                   │   │
│  │  - IntegerExtensions                             │   │
│  └─────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────┐   │
│  │  Models (Domain Entities)                        │   │
│  │  - Tournament, Event, EventTemplate, Participant │   │
│  │  - Result, House, Announcement, EventUpdate      │   │
│  │  - TournamentHouseSummary, HouseLeader           │   │
│  └─────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────┐   │
│  │  Data Layer                                      │   │
│  │  - SportsDayDbContext (EF Core)                  │   │
│  │  - DbInitializer                                 │   │
│  └─────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────┐   │
│  │  Enums                                           │   │
│  │  - DivisionType, EventClass, EventStatus         │   │
│  │  - EventType, AnnouncementPriority               │   │
│  └─────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────┘
                          ↕
┌─────────────────────────────────────────────────────────┐
│              Azure SQL Database                          │
│  - Houses, Tournaments, Events, Participants             │
│  - Results, Announcements, EventUpdates                  │
│  - TournamentHouseSummaries, AspNetUsers/Roles           │
└─────────────────────────────────────────────────────────┘
```

## Project Structure

### SportsDay.Lib (Class Library)
**Purpose**: Core business logic, data models, and database access

**Key Directories**:
- `Models/` - Domain entities (Tournament, Event, Participant, Result, House, etc.)
- `Data/` - EF Core DbContext and database initialization
- `Services/` - Business logic services
- `Services/Interfaces/` - Service interfaces
- `Enums/` - Enumeration types
- `Extensions/` - Extension methods

**Key Files**:
- `Data/SportsDayDbContext.cs` - EF Core database context
- `Data/DbInitializer.cs` - Database seeding and initialization
- `Services/TournamentService.cs` - Tournament management logic
- `Services/HouseService.cs` - House management logic
- `Services/ParticipantService.cs` - Participant management logic
- `Services/HouseLeaderService.cs` - House leader management logic
- `Services/EventTemplateService.cs` - Event template management logic
- `Services/DeveloperService.cs` - Developer/testing data generation (DEBUG only)
- `Services/EventService.cs` - Event management logic
- `Extensions/ServiceCollectionExtensions.cs` - Service registration extension method
- `Models/BaseEntity.cs` - Base class for all entities with audit fields
- `ViewModels/HouseDetailsViewModel.cs` - House details with rankings and results
- `ViewModels/HouseResultsViewModel.cs` - House members with event participation

### SportsDay.Web (ASP.NET Core MVC)
**Purpose**: Web application with public and admin interfaces

**Key Directories**:
- `Areas/Admin/` - Administrative interface
  - `Controllers/` - Admin controllers
  - `Views/` - Admin views
- `Areas/Identity/` - Authentication pages
- `Controllers/` - Public-facing controllers
- `Views/` - Public views and shared components
- `Hubs/` - SignalR hubs
- `wwwroot/` - Static files (CSS, JS, images)

**Key Files**:
- `Program.cs` - Application startup and configuration
- `Hubs/SportsHub.cs` - SignalR hub for real-time updates
- `Controllers/DashboardController.cs` - Public dashboard
- `Areas/Admin/Controllers/EventsController.cs` - Event management
- `Areas/Admin/Controllers/EventTemplatesController.cs` - Event template management
- `Areas/Admin/Controllers/ResultsController.cs` - Result entry
- `Areas/Admin/Controllers/DeveloperController.cs` - Developer tools (DEBUG only)

## Data Model

### Core Entities

**Tournament** (GUID)
- Central entity for each sports day event
- Has one-to-many relationships with Events, Participants, Results, Announcements
- Only one tournament can be active at a time (enforced by unique index)

**House** (Integer ID)
- Permanent entities representing the six school houses
- Fixed set: Beckford, Bell, Campbell, Nutall, Smith, Wortley
- Each has a color and optional logo URL

**Event** (GUID)
- Belongs to a Tournament
- Has ClassGroup (Open, Class 1-4), GenderGroup (Boys, Girls, Open)
- Tracks record, record holder, point system
- Status: Scheduled, InProgress, Completed, Cancelled
- Type: Distance or Speed

**EventTemplate** (GUID)
- Independent of tournaments (reusable templates)
- Same classification as Event: ClassGroup, GenderGroup, Category, Type
- Stores record information: Record, RecordHolder, RecordSettingYear, RecordNote
- IsActive flag for filtering during import
- Can be imported into tournaments to create Event instances

**Participant** (GUID)
- Belongs to a Tournament and House
- Has GenderGroup, EventClassGroup, age information
- Tracks accumulated points

**Result** (GUID)
- Links Event, Participant, House, and Tournament
- Contains placement, speed/distance, calculated points
- Flags new records

**Announcement** (GUID)
- Belongs to Tournament
- Has priority level, expiration date, enable/disable flag
- Body supports markdown formatting

**EventUpdate** (GUID)
- Belongs to Event and Tournament
- Tracks event status changes and result entries
- Displayed in real-time update stream

**TournamentHouseSummary** (GUID)
- Aggregates points by Tournament, House, and Division
- Used for leaderboard calculations

### Entity Relationships

```
Tournament (1) ──→ (N) Event
Tournament (1) ──→ (N) Participant
Tournament (1) ──→ (N) Result
Tournament (1) ──→ (N) Announcement
Tournament (1) ──→ (N) EventUpdate
Tournament (1) ──→ (N) TournamentHouseSummary

House (1) ──→ (N) Participant
House (1) ──→ (N) Result
House (1) ──→ (N) TournamentHouseSummary

Event (1) ──→ (N) Result
Event (1) ──→ (N) EventUpdate

Participant (1) ──→ (N) Result
```

## Key Design Patterns

### Repository Pattern
- DbContext acts as repository
- Services provide abstraction over data access
- Example: `ITournamentService` abstracts tournament operations

### Service Layer Pattern
- Business logic encapsulated in services
- Controllers delegate to services
- All services registered via `ServiceCollectionExtensions.AddSportsDayServices()`
- Examples:
  - `TournamentService` handles tournament CRUD and activation
  - `HouseService` handles house CRUD operations
  - `ParticipantService` handles participant management
  - `HouseLeaderService` handles house leader operations
  - `EventTemplateService` handles event template CRUD and import operations
    - `EventService` handles event CRUD and retrieval for active tournament
    - `DeveloperService` handles test data generation and cleanup (DEBUG only)

### Hub Pattern (SignalR)
- `SportsHub` manages real-time connections
- Controllers inject `IHubContext<SportsHub>` to broadcast updates
- Clients subscribe to specific message types

### Base Entity Pattern
- All entities inherit from `BaseEntity`
- Provides audit fields: CreatedAt, CreatedBy, UpdatedAt, UpdatedBy
- Consistent tracking across all entities
- Controllers set CreatedBy/UpdatedBy to logged-in user's name via `User.Identity?.Name`

### Extension Method Pattern
- Service registration centralized in `ServiceCollectionExtensions`
- `AddSportsDayServices()` registers all application services
- Simplifies `Program.cs` configuration
- Makes service registration maintainable and testable

## Critical Implementation Paths

### Result Entry Flow
1. Admin enters result via `ResultsController.Create()`
2. System calculates points from event's point system
3. Checks if result breaks existing record
4. Saves result to database
5. Creates `EventUpdate` entry
6. Broadcasts via SignalR: `_hubContext.Clients.All.SendAsync("ReceiveResult", ...)`
7. Public dashboard receives update and refreshes

### Real-time Update Flow
1. Server-side event occurs (result entry, announcement, status change)
2. Controller calls `_hubContext.Clients.All.SendAsync(methodName, data)`
3. SignalR hub broadcasts to all connected clients
4. Client JavaScript receives message via connection.on(methodName)
5. Client updates DOM without page reload
6. Fallback: Page auto-refreshes every 2 minutes

### Tournament Activation Flow
1. Admin activates tournament via `TournamentsController.SetActiveTournamentAsync()`
2. Service deactivates currently active tournament (if any)
3. Activates selected tournament
4. Unique index ensures only one active tournament
5. Public dashboard shows newly activated tournament

### Point Calculation
1. Result placement determines points
2. Event's PointSystem string (e.g., "9,7,6,5,4,3,2,1") parsed
3. Points assigned based on placement index
4. Points stored in Result entity
5. TournamentHouseSummary aggregates points by division

### Event Template Import Flow
1. Admin navigates to Event Templates > Import
2. Selects templates to import (individual, by class, by gender, or all)
3. System retrieves next available event number for tournament
4. For each selected template, creates new Event with:
   - Copied properties from template (name, classification, record info)
   - Assigned tournament ID and event number
   - Status set to Scheduled
5. Events saved to database
6. SignalR notification sent to connected clients
7. Admin redirected to Events list

## Authentication & Authorization

### Identity Implementation
- ASP.NET Core Identity with IdentityUser
- Roles: Administrator, Judge, Announcer, HouseLeader, Viewer
- Google OAuth configured (currently commented out for development)
- Authorization attributes commented out in controllers for development

### Security Considerations
- Anti-forgery tokens on POST actions
- Role-based access control (when enabled)
- Azure Key Vault for production secrets
- HTTPS enforcement

## Database Schema

### Key Tables
- Houses (int PK)
- Tournaments (GUID PK, unique index on IsActive=1)
- Events (GUID PK, FK to Tournament)
- EventTemplates (GUID PK, no FK - independent of tournaments)
- Participants (GUID PK, FK to Tournament and House)
- Results (GUID PK, FK to Event, Participant, House, Tournament)
- Announcements (GUID PK, FK to Tournament)
- EventUpdates (GUID PK, FK to Event and Tournament)
- TournamentHouseSummaries (GUID PK, FK to Tournament and House)
- AspNetUsers, AspNetRoles (Identity tables)

### Indexes
- Unique filtered index on Tournaments.IsActive WHERE IsActive = 1
- Foreign key indexes on all relationships

## Technology Stack

### Backend
- .NET 10.0
- ASP.NET Core MVC
- Entity Framework Core 10.0.1
- ASP.NET Core Identity
- SignalR (self-hosted, Azure SignalR commented out)

### Frontend
- Bootstrap 5
- jQuery
- SignalR JavaScript client
- Bootstrap Icons
- Markdig (markdown parsing)
- Humanizer.Core (date/time formatting)

### Database
- SQL Server (Azure SQL Database for production)
- LocalDB for development

### Infrastructure
- Azure App Service
- Azure SQL Database
- Azure SignalR Service (configured but not active)
- Azure Key Vault
- Azure Application Insights
- Terraform for infrastructure as code

## Configuration Management

### appsettings.json Structure
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "SQL Server connection string"
  },
  "Azure": {
    "SignalR": {
      "ConnectionString": "Azure SignalR connection"
    }
  },
  "Authentication": {
    "Google": {
      "ClientId": "OAuth client ID",
      "ClientSecret": "OAuth secret"
    }
  }
}
```

### Environment-Specific Settings
- appsettings.json (base settings)
- appsettings.Development.json (dev overrides)
- appsettings.Production.json (production overrides)
- appsettings.Example.json (template with dummy values)

## Deployment Architecture

### Azure Resources
1. Resource Group
2. App Service Plan (B1 tier)
3. App Service (Windows, .NET 8.0 runtime)
4. SQL Server + SQL Database (Basic tier)
5. SignalR Service (Free tier)
6. Application Insights
7. Key Vault

### Terraform Configuration
- All resources defined in `terraform/main.tf`
- Variables in `terraform/variables.tf`
- Example values in `terraform/terraform.tfvars.example`