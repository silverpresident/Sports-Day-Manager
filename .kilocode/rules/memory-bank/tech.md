# Sports Day Manager - Technology Stack

## Core Technologies

### Backend Framework
- **.NET 10.0**: Latest LTS version of .NET
- **ASP.NET Core MVC**: Web application framework
- **C# 10**: Programming language with nullable reference types enabled

### Database
- **SQL Server**: Primary database engine
- **Azure SQL Database**: Cloud-hosted production database
- **LocalDB**: Development database (mssqllocaldb)
- **Entity Framework Core 10.0.1**: ORM for database access

### Authentication & Authorization
- **ASP.NET Core Identity**: User management and authentication
- **IdentityUser**: Base user class
- **IdentityRole**: Role-based authorization
- **Google OAuth**: External authentication provider (configured but commented out)

### Real-time Communication
- **SignalR**: Real-time bidirectional communication
- **Self-hosted SignalR**: Currently active implementation
- **Azure SignalR Service**: Configured but commented out for development

## Frontend Technologies

### UI Frameworks
- **Bootstrap 5**: CSS framework for responsive design
- **Bootstrap Icons**: Icon library (preferred)
- **Font Awesome**: Secondary icon library (when Bootstrap Icons unavailable)
- **jQuery**: JavaScript library for DOM manipulation

### JavaScript Libraries
- **SignalR JavaScript Client**: Client-side SignalR connection
- **@microsoft/signalr**: NPM package for SignalR

### Content Processing
- **Markdig 0.40.0**: Markdown parsing for announcements
- **Humanizer.Core 2.14.1**: Human-readable date/time formatting

## Development Tools

### IDE & Editors
- **Visual Studio Code**: Primary development environment
- **Visual Studio 2022**: Alternative IDE option

### Version Control
- **Git**: Source control
- **.gitignore**: Configured to exclude appsettings.json (except .Example.json)

### Project Files
- **SportsDay.sln**: Visual Studio solution file
- **.csproj**: Project files for SportsDay.Lib and SportsDay.Web

## Package Dependencies

### SportsDay.Lib Dependencies
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.1" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.1" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="10.0.1" />
<PackageReference Include="Microsoft.Extensions.Hosting.Abstractions" Version="10.0.1" />
```

### SportsDay.Web Dependencies
```xml
<PackageReference Include="Humanizer.Core" Version="2.14.1" />
<PackageReference Include="Markdig" Version="0.40.0" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.Google" Version="10.0.1" />
<PackageReference Include="Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore" Version="10.0.1" />
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="10.0.1" />
<PackageReference Include="Microsoft.AspNetCore.Identity.UI" Version="10.0.1" />
<PackageReference Include="Microsoft.Azure.SignalR" Version="1.29.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="10.0.1" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.1" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.1" />
```

## Infrastructure & Deployment

### Cloud Platform
- **Microsoft Azure**: Cloud hosting platform
- **Azure App Service**: Web application hosting
- **Azure SQL Database**: Managed database service
- **Azure SignalR Service**: Managed SignalR service
- **Azure Key Vault**: Secrets management
- **Azure Application Insights**: Application monitoring and logging

### Infrastructure as Code
- **Terraform**: Infrastructure provisioning
- **terraform/main.tf**: Main Terraform configuration
- **terraform/variables.tf**: Variable definitions
- **terraform/terraform.tfvars.example**: Example variable values

### Azure Resources Configuration
```hcl
- Resource Group
- App Service Plan (B1 tier, Windows)
- Windows Web App (.NET 8.0 runtime)
- SQL Server (version 12.0)
- SQL Database (Basic tier, 2GB)
- SignalR Service (Free tier)
- Application Insights
- Key Vault (Standard SKU)
```

## Database Management

### Entity Framework Core
- **Code-First Approach**: Models define database schema
- **Migrations**: Database schema versioning
- **DbContext**: SportsDayDbContext for data access
- **LINQ**: Query language for data operations

### Database Scripts
- **SQL/setup.sql**: Database creation and initialization
- **SQL/cleanup.sql**: Database reset script
- **SQL/setup-identity.sql**: Identity tables setup
- **SQL/cleanup-identity.sql**: Identity cleanup

### Database Features
- **GUID Primary Keys**: For most entities (except House which uses int)
- **Unique Filtered Index**: Ensures only one active tournament
- **Foreign Key Constraints**: Maintain referential integrity
- **Cascade Delete**: Automatic cleanup of related records
- **Enum Storage**: Stored as strings in database

## Development Setup

### Prerequisites
- .NET 10.0 SDK
- SQL Server or LocalDB
- Azure CLI (for deployment)
- Terraform CLI (for infrastructure)
- Visual Studio Code or Visual Studio 2022

### Configuration Files
- **appsettings.json**: Application configuration (gitignored)
- **appsettings.Example.json**: Template with dummy values
- **appsettings.Development.json**: Development overrides
- **appsettings.Production.json**: Production overrides

### Connection Strings
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SportsDay;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

## Build & Run Commands

### Build
```bash
dotnet build
```

### Run Application
```bash
dotnet run --project SportsDay.Web
```

### Database Migrations
```bash
dotnet ef database update --project SportsDay.Lib --startup-project SportsDay.Web
```

### Run Tests
```bash
dotnet test
```

## Security Considerations

### Authentication
- ASP.NET Core Identity with password requirements
- Google OAuth for external authentication
- Email confirmation required (configurable)
- Account lockout after failed attempts

### Password Policy
```csharp
RequireDigit = true
RequireLowercase = true
RequireNonAlphanumeric = true
RequireUppercase = true
RequiredLength = 8
RequiredUniqueChars = 1
```

### Secrets Management
- **Development**: User Secrets (UserSecretsId in .csproj)
- **Production**: Azure Key Vault
- **Never commit**: appsettings.json with real credentials

## Performance Optimizations

### Entity Framework
- `.AsNoTracking()` for read-only queries
- Eager loading with `.Include()` for related data
- Indexed foreign keys for faster joins

### SignalR
- Automatic reconnection on connection loss
- Fallback polling every 2 minutes
- Efficient message broadcasting

### Caching Strategy
- No explicit caching implemented yet
- Potential for output caching on public pages
- SignalR handles real-time updates efficiently

## Monitoring & Logging

### Application Insights
- Configured for production deployment
- Tracks application performance
- Logs errors and exceptions
- Monitors user activity

### Logging Framework
- ASP.NET Core built-in logging
- ILogger<T> dependency injection
- Log levels: Information, Warning, Error
- Console logging in development

## Theme & Styling

### Color Scheme
- **Primary**: #006400 (Dark Green)
- **Secondary**: #FFD700 (Gold)
- **House Colors**:
  - Beckford: #FF0000 (Red)
  - Bell: #006400 (Green)
  - Campbell: #FFA500 (Orange)
  - Nutall: #800080 (Purple)
  - Smith: #0000FF (Blue)
  - Wortley: #FFFF00 (Yellow)

### Custom CSS
- `wwwroot/css/site.css`: Custom styles
- Bootstrap 5 customization
- Responsive design
- Markdown content styling
- Custom scrollbars for update stream

## Browser Compatibility

### Supported Browsers
- Modern browsers with ES6 support
- Chrome, Firefox, Safari, Edge
- Mobile browsers (iOS Safari, Chrome Mobile)

### Required Features
- WebSocket support for SignalR
- JavaScript enabled
- CSS Grid and Flexbox support