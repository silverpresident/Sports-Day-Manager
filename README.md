# Sports Day Manager

A C# .NET MVC web application for managing St Jago High School sports day events.

## Features

- User authentication with Google Single Sign-On
- Real-time updates using Azure SignalR
- Tournament management
- House (team) management
- **House Leader Self-Registration** - Users can register as house leaders
- **Participant Self-Registration** - House leaders can register and manage participants
- Event scheduling and registration
- Results tracking and leaderboards
- Real-time announcements
- Records management

## Prerequisites

- .NET 8.0 SDK
- Visual Studio Code or Visual Studio 2022
- Azure subscription
- SQL Server Management Studio (SSMS) or Azure Data Studio
- Terraform CLI

## Azure Resources Required

The following Azure resources are needed:

1. Azure SQL Database
2. Azure SignalR Service
3. Azure App Service
4. Azure Key Vault
5. Azure Application Insights

## Setup Instructions

### 1. Azure Setup

1. Install Azure CLI and log in:
   ```bash
   az login
   ```

2. Create Azure resources using Terraform:
   ```bash
   cd terraform
   cp terraform.tfvars.example terraform.tfvars
   # Edit terraform.tfvars with your values
   terraform init
   terraform plan
   terraform apply
   ```

### 2. Google OAuth Setup

1. Go to [Google Cloud Console](https://console.cloud.google.com)
2. Create a new project or select an existing one
3. Enable the Google+ API
4. Create OAuth 2.0 credentials
5. Add authorized redirect URIs:
   - For local development: `https://localhost:5001/signin-google`
   - For production: `https://your-azure-app-service-url/signin-google`

### 3. Database Setup

1. Run the database setup script:
   ```bash
   sqlcmd -S your-server.database.windows.net -U your-username -P your-password -d master -i SQL/setup.sql
   ```

2. To reset the database if needed:
   ```bash
   sqlcmd -S your-server.database.windows.net -U your-username -P your-password -d master -i SQL/cleanup.sql
   ```

### 4. Application Configuration

1. Create appsettings.json from the example:
   ```bash
   cp appsettings.Example.json appsettings.json
   ```

2. Update the following settings in appsettings.json:
   - SQL Server connection string
   - Azure SignalR connection string
   - Google OAuth credentials

### 5. Running the Application

1. Build and run the application:
   ```bash
   dotnet build
   dotnet run --project SportsDay.Web
   ```

2. Access the application at `https://localhost:5001`

3. Log in with the default admin credentials:
   - Email: admin@stjago.edu
   - Password: admin2025

## Project Structure

- `SportsDay.Lib/` - Core business logic and data models
  - `Models/` - Entity models
  - `Services/` - Business logic services
  - `Data/` - Database context and configurations

- `SportsDay.Web/` - Web application
  - `Areas/` - Admin and Identity areas
  - `Controllers/` - MVC controllers
  - `Views/` - Razor views
  - `wwwroot/` - Static files

## Default Data

### Houses
1. Beckford (Red)
2. Bell (Green)
3. Campbell (Orange)
4. Nutall (Purple)
5. Smith (Blue)
6. Wortley (Yellow)

### Divisions
1. BOYS
2. GIRLS

### Event Classes
0. Open
1. Class 1
2. Class 2
3. Class 3
4. Class 4
