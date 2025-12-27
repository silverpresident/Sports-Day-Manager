# Sports Day Manager - Product Description

## What This Product Does

Sports Day Manager is a real-time tournament management system that transforms how St. Jago High School runs its annual sports day. It replaces manual scorekeeping and paper-based tracking with a live, digital platform that keeps everyone informed and engaged.

## Problems It Solves

### For Administrators
- **Manual Score Tracking**: Eliminates error-prone paper-based scoring
- **Delayed Updates**: Provides instant score and leaderboard updates
- **Complex Point Calculations**: Automates point allocation based on placement
- **Record Keeping**: Automatically tracks and flags new records
- **Communication**: Centralized announcement system for all attendees

### For Spectators
- **Limited Visibility**: Real-time access to all event results and standings
- **Information Gaps**: Live update stream keeps everyone informed
- **Engagement**: Interactive leaderboards show house competition in real-time

### For House Leaders
- **Participant Management**: Self-service registration for their house members
- **Performance Tracking**: View their house's performance across divisions

## How It Works

### Tournament Flow

1. **Setup Phase**
   - Administrator creates a new tournament
   - Sets up events with categories, divisions, and point systems
   - Registers participants to houses
   - Activates the tournament

2. **Competition Phase**
   - Events progress through statuses: Scheduled → InProgress → Completed
   - Officials enter results (placement, time/distance)
   - System automatically calculates points based on event's point system
   - Real-time updates broadcast to all connected clients
   - Leaderboards update instantly
   - New records are automatically detected and flagged

3. **Viewing Experience**
   - Public dashboard shows live leaderboards by division
   - Update stream displays recent results and announcements
   - Event details show participants and results
   - House pages display team members and accumulated points

### Key Workflows

**Result Entry**:
1. Official selects event and participant
2. Enters placement or time/distance
3. System calculates points from event's point system
4. Checks if result breaks existing record
5. Creates event update
6. Broadcasts via SignalR to all clients
7. Updates house summaries and leaderboards

**Real-time Updates**:
- SignalR hub maintains persistent connections
- Server pushes updates on: new results, announcements, event status changes
- Clients automatically refresh relevant sections
- No page reload required

## User Experience Goals

### For Spectators
- **Immediate**: See results as they happen
- **Clear**: Easy-to-read leaderboards with house colors
- **Engaging**: Live update stream creates excitement
- **Accessible**: Mobile-friendly for on-field viewing

### For Administrators
- **Efficient**: Quick result entry with minimal clicks
- **Reliable**: Automatic calculations reduce errors
- **Flexible**: Support for different event types and point systems
- **Comprehensive**: Full tournament management in one place

### For House Leaders
- **Empowered**: Register their own participants and manage event registrations
- **Informed**: Track their house's performance across all events
- **Simple**: Focused interface for their specific needs

### For All Users
- **Transparent**: View comprehensive records and results
- **Analytical**: Access statistics and performance metrics
- **Historical**: Track records over time with year-based filtering

## Core Features

### Public Dashboard
- Live leaderboards showing house standings by division
- Overall tournament leaderboard
- Real-time update stream with timestamps
- Announcements with priority levels (info, warning, danger)
- Auto-refresh every 2 minutes as fallback to SignalR

### Event Management
- Create events with divisions (Boys, Girls, Open)
- Assign to classes (Open, Class 1-4)
- Set event types (Distance or Speed)
- Configure point systems (9,7,6,5,4,3,2,1 or 12,10,9,8,7,6,1)
- Track records and record holders
- Manage event status lifecycle

### Results System
- Enter results by placement or time/distance
- Automatic point calculation
- Record detection and flagging
- Real-time broadcast to all clients
- Event update generation

### House System
- Six permanent houses with distinct colors
- Beckford (Red), Bell (Green), Campbell (Orange)
- Nutall (Purple), Smith (Blue), Wortley (Yellow)
- Point accumulation across divisions
- Participant roster management

### Participant Management
- Register participants with house assignment
- Track division (Boys, Girls, Open) and class
- Age group and date of birth tracking
- Notes field for additional information
- Result history per participant

### Announcements
- Create announcements with markdown formatting
- Set priority levels for visual distinction
- Optional expiration dates
- Enable/disable toggle
- Real-time broadcast to public dashboard

### Records Management
- View existing records from events (historical)
- View new records set during tournaments
- Filter by division, class, category, and year
- Toggle between new records only, existing only, or all
- Detailed record information with holder and year set

### Results Viewing
- Comprehensive result viewing with multiple filters
- Filter by division, class, category, house, or event
- View results by specific event or house
- Statistics dashboard showing:
  - Total results and events with results
  - Participants with results
  - New records set
  - Points by house and division
  - Top performers (podium finishers)

## Technical Approach

### Real-time Architecture
- SignalR hub (`SportsHub`) for bidirectional communication
- Server-to-client push for results, announcements, event updates
- Automatic reconnection on connection loss
- Fallback polling every 2 minutes

### Data Model
- Tournament-centric design (one active tournament at a time)
- Houses as permanent entities (integer IDs)
- All other entities use GUIDs
- BaseEntity pattern for audit fields (CreatedAt, CreatedBy, etc.)
- Enum-based divisions and event classes

### Point Calculation
- Configurable point systems per event
- Automatic calculation based on placement
- Support for 8-place (9,7,6,5,4,3,2,1) and 7-place (12,10,9,8,7,6,1) systems
- Points aggregate at house level by division

### Record Tracking
- Distance events: higher is better
- Speed events: lower (faster) is better
- Automatic comparison against existing records
- Flag new records and update record holder
- Historical record preservation