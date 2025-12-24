# Sports Day Manager

## High-Level Overview

The Sports Day Manager is a comprehensive web application designed to manage St. Jago High School's annual sports day tournament. It provides a public-facing interface for spectators and participants to view real-time information, and a secure administrative backend for staff to manage all aspects of the event.

### Key Features:

*   **Public Portal:**
    *   Real-time leaderboards for houses and individual participants.
    *   Live stream of event updates and announcements.
    *   Detailed schedules, event information, and results.
    *   View lists of participating houses and their members.

*   **Admin Area:**
    *   Secure login for authorized administrators.
    *   Full CRUD (Create, Read, Update, Delete) functionality for:
        *   Tournaments
        *   Houses
        *   Events
        *   Participants
        *   Results
        *   Announcements
    *   Management of user roles and permissions.

*   **Real-Time Functionality:**
    *   Uses SignalR to instantly push updates (e.g., new results, announcements) to all connected clients without requiring a page refresh.

### Technical Stack:

*   **Backend:** C# .NET, ASP.NET Core MVC
*   **Data Access:** Entity Framework Core with a SQL Server database.
*   **Frontend:** Bootstrap, jQuery, and Bootstrap Icons.
*   **Authentication:** ASP.NET Core Identity, with support for Google OAuth.
*   **Deployment:** Designed for hosting on Microsoft Azure, with infrastructure defined using Terraform.

### Project Structure:

The solution is organized into two main projects:

1.  **`SportsDay.Lib`**: A class library containing the core business logic, including models (entities), services, the database context, and SignalR hubs.
2.  **`SportsDay.Web`**: The ASP.NET Core MVC application that provides both the public UI and the protected admin area.
