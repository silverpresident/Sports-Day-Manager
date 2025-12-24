# Sports Day Manager - Project Brief

## Project Overview

Sports Day Manager is a comprehensive web application for managing St. Jago High School's annual sports day tournament. The system provides real-time event tracking, results management, and live leaderboards for a multi-house athletic competition.

## Core Purpose

Enable efficient management and real-time tracking of a school sports day tournament with:
- Multiple houses (teams) competing across various athletic events
- Real-time score updates and leaderboards
- Public viewing interface for spectators
- Secure administrative backend for event management
- Live announcements and event updates

## Key Stakeholders

- **Administrators**: Full system access for tournament setup and management
- **Judges/Officials**: Result entry and event status updates
- **House Leaders**: Limited access for participant registration
- **Spectators**: Public access to view live results and leaderboards
- **Participants**: Athletes competing in events

## Primary Features

### Public Interface
- Live leaderboards (overall and by division)
- Real-time event updates stream
- Announcements board
- Event schedules and details
- House information and participants
- Records tracking (current and historical)

### Administrative Interface
- Tournament management (create, activate, manage)
- House management (6 houses: Beckford, Bell, Campbell, Nutall, Smith, Wortley)
- Event management (scheduling, categories, point systems)
- Participant registration and management
- Results entry with automatic point calculation
- Announcement creation and management
- User and role management

## Technical Foundation

- **Framework**: .NET 10.0 with ASP.NET Core MVC
- **Database**: SQL Server (Azure SQL Database)
- **Authentication**: ASP.NET Core Identity with Google OAuth
- **Real-time**: SignalR for live updates
- **Deployment**: Microsoft Azure (App Services, SQL Database, SignalR Service)
- **Infrastructure**: Terraform for Azure resource provisioning

## Business Rules

1. Only one tournament can be active at a time
2. Houses are permanent entities (6 houses with specific colors)
3. Events have predefined point systems (9,7,6,5,4,3,2,1 or 12,10,9,8,7,6,1)
4. Participants compete in divisions (Boys, Girls, Open) and classes (Open, Class 1-4)
5. Records are tracked per event and automatically flagged when broken
6. Points accumulate at house level across all divisions
7. Real-time updates push to all connected clients via SignalR

## Success Criteria

- Seamless real-time updates during live events
- Accurate point calculation and leaderboard updates
- Intuitive interface for both administrators and spectators
- Reliable performance during high-traffic tournament days
- Secure authentication and authorization
- Mobile-responsive design for on-field access
