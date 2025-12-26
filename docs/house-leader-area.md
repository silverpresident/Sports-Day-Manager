# House Leader Area

## Overview

The House Leader Area is a dedicated section of the Sports Day Manager application that allows house leaders to manage their house's participants and event registrations. This area provides a focused interface for house leaders to perform their duties without access to the full administrative functionality.

## Features

### 1. House Leader Registration
- Users can register as house leaders for a specific house
- Each user can only be a house leader for one house at a time
- House leaders can unregister themselves when needed

### 2. Dashboard
- Overview of house statistics (participants, points, events entered)
- Quick access to common actions
- Recent participants list
- Active tournament information

### 3. Participant Management
- **View Participants**: List all participants registered for the house in the active tournament
- **Add Participant**: Register new participants with details:
  - First and last name
  - Gender (Boys/Girls)
  - Class (Open, Class 1-4)
  - Date of birth and age group
  - Optional notes
- **Edit Participant**: Update participant information
- **Delete Participant**: Remove participants (with confirmation)

### 4. Event Management
- **View Events**: List all events in the active tournament with:
  - Event number and name
  - Division and class
  - Category and status
  - Number of house participants registered
  - Total participants across all houses
- **Event Details**: View detailed event information and registered participants
- **Register Participants**: Add house participants to events
  - Shows eligible participants based on gender and class matching
  - Respects participant limits per house
  - Allows unregistering participants (if no results recorded)

## URL Structure

All House Leader area URLs are prefixed with `/HouseLeader/`:

| URL | Description |
|-----|-------------|
| `/HouseLeader/Dashboard` | Main dashboard |
| `/HouseLeader/Dashboard/Register` | Register as house leader |
| `/HouseLeader/Dashboard/Unregister` | Unregister as house leader |
| `/HouseLeader/Participants` | List participants |
| `/HouseLeader/Participants/Add` | Add new participant |
| `/HouseLeader/Participants/Edit/{id}` | Edit participant |
| `/HouseLeader/Participants/Delete/{id}` | Delete participant |
| `/HouseLeader/Events` | List events |
| `/HouseLeader/Events/Details/{id}` | Event details |
| `/HouseLeader/Events/RegisterParticipant/{id}` | Register participants to event |

## Authorization

The House Leader area requires authentication and is restricted to users with the `HouseLeader` or `Administrator` role. The base controller applies the `[Authorize(Roles = "HouseLeader,Administrator")]` attribute.

## Controllers

### DashboardController
- `Index()` - Main dashboard with house overview
- `Register()` - GET/POST for house leader registration
- `Unregister()` - GET/POST for house leader unregistration

### ParticipantsController
- `Index()` - List all house participants
- `Add()` - GET/POST for adding new participants
- `Edit(Guid id)` - GET/POST for editing participants
- `Delete(Guid id)` - GET/POST for deleting participants

### EventsController
- `Index()` - List all events with registration counts
- `Details(Guid id)` - View event details
- `RegisterParticipant(Guid id)` - GET/POST for registering participants to events
- `UnregisterParticipant(Guid eventId, Guid participantId)` - POST for removing participants from events

## ViewModels

### HouseLeaderDashboardViewModel
Contains dashboard statistics and participant list for the house leader's house.

### HouseLeaderEventViewModel
Contains event information with participant counts for the house.

### HouseLeaderEventsViewModel
Contains list of events with house-specific registration information.

### RegisterParticipantToEventViewModel
Contains event details and lists of available/registered participants for event registration.

### AddParticipantViewModel
Contains form data for adding/editing participants.

## Layout

The House Leader area uses a dedicated layout (`_HouseLeaderLayout.cshtml`) with:
- Green navigation bar with house leader branding
- Navigation links to Dashboard, Participants, and Events
- Link back to main site
- Login partial for user authentication
- Alert message display (success, error, warning, info)
- SignalR connection for real-time updates

## Business Rules

1. **One House Leader per User**: A user can only be a house leader for one house at a time
2. **House Restriction**: House leaders can only manage participants from their own house
3. **Tournament Restriction**: Participants are registered for the active tournament only
4. **Event Eligibility**: Participants can only be registered for events matching their gender and class
5. **Participant Limits**: Events may have limits on participants per house
6. **Result Protection**: Participants with recorded results cannot be unregistered from events

## Security Considerations

- All POST actions are protected with `[ValidateAntiForgeryToken]`
- House ownership is verified before any participant operations
- User authentication is required for all actions
- Role-based authorization restricts access to house leaders and administrators

## Migration from Old Controller

The original `HouseLeaderController` in the `Controllers` folder has been superseded by the new area-based implementation. The old controller should be removed or deprecated after migration.

### Key Differences
1. **Area-based routing**: URLs now use `/HouseLeader/` prefix
2. **Separate controllers**: Functionality split into Dashboard, Participants, and Events controllers
3. **ViewModels**: Strongly-typed ViewModels instead of ViewBag
4. **Event registration**: New functionality for registering participants to events
5. **Dedicated layout**: Custom layout for the house leader portal

## Future Enhancements

1. **Bulk Registration**: Register multiple participants to an event at once
2. **Event Filtering**: Filter events by division, class, or status
3. **Participant Search**: Search participants by name
4. **Export**: Export participant lists to CSV/Excel
5. **Notifications**: Real-time notifications for event updates
6. **Mobile Optimization**: Enhanced mobile experience for on-field use