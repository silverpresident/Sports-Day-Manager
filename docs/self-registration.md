# Self-Registration Features

## Overview

The Sports Day Manager now includes comprehensive self-registration features that allow users to register as house leaders and manage participants for their respective houses. This document describes the implementation and usage of these features.

## Features Implemented

### 1. House Leader Registration

**Purpose**: Allows authenticated users to register themselves as leaders for one of the six houses.

**Key Components**:
- **Model**: [`HouseLeader`](../SportsDay.Lib/Models/HouseLeader.cs) - Links a user to a house
- **Service**: [`IHouseLeaderService`](../SportsDay.Lib/Services/Interfaces/IHouseLeaderService.cs) and [`HouseLeaderService`](../SportsDay.Lib/Services/HouseLeaderService.cs)
- **Controller**: [`HouseLeaderController`](../SportsDay.Web/Controllers/HouseLeaderController.cs)
- **Views**:
  - [`Register.cshtml`](../SportsDay.Web/Views/HouseLeader/Register.cshtml) - House leader registration form
  - [`Dashboard.cshtml`](../SportsDay.Web/Views/HouseLeader/Dashboard.cshtml) - House leader management dashboard
  - [`Unregister.cshtml`](../SportsDay.Web/Views/HouseLeader/Unregister.cshtml) - Unregister confirmation

**Workflow**:
1. User logs in to the system
2. Navigates to House Leader Dashboard from user menu
3. If not registered, redirected to registration page
4. Selects their house from dropdown
5. Submits registration
6. Can now manage participants for their house

**Business Rules**:
- A user can only be a house leader for one house at a time
- House leaders can only manage participants for their assigned house
- House leaders can unregister at any time
- Existing participants remain in the system after unregistration

### 2. Participant Self-Registration

**Purpose**: Allows house leaders to register and manage participants for their house in the active tournament.

**Key Components**:
- **Model**: [`Participant`](../SportsDay.Lib/Models/Participant.cs) - Existing model, no changes needed
- **Service**: [`IParticipantService`](../SportsDay.Lib/Services/Interfaces/IParticipantService.cs) and [`ParticipantService`](../SportsDay.Lib/Services/ParticipantService.cs)
- **Controller**: [`ParticipantController`](../SportsDay.Web/Controllers/ParticipantController.cs)
- **Views**:
  - [`Register.cshtml`](../SportsDay.Web/Views/Participant/Register.cshtml) - Participant registration form
  - [`Edit.cshtml`](../SportsDay.Web/Views/Participant/Edit.cshtml) - Edit participant details
  - [`Delete.cshtml`](../SportsDay.Web/Views/Participant/Delete.cshtml) - Delete confirmation

**Workflow**:
1. House leader logs in and accesses their dashboard
2. Clicks "Register New Participant"
3. Fills in participant details:
   - First Name and Last Name
   - Date of Birth
   - Age Group
   - Gender Division (Boys, Girls, Open)
   - Event Class (Open, Class 1-4)
   - Optional notes
4. Submits registration
5. Participant appears in house leader's dashboard
6. Can edit or delete participants as needed

**Business Rules**:
- Only house leaders can register participants
- Participants can only be registered for the house leader's house
- Participants are registered for the active tournament only
- Age is automatically calculated from date of birth
- House leaders can only edit/delete participants from their own house

## Database Changes

### HouseLeader Table

```sql
CREATE TABLE HouseLeaders (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    HouseId INT NOT NULL,
    UserId NVARCHAR(450) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(450),
    UpdatedAt DATETIME2,
    UpdatedBy NVARCHAR(450),
    FOREIGN KEY (HouseId) REFERENCES Houses(Id) ON DELETE CASCADE,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);
```

### Relationship Updates

- **House** → **HouseLeaders**: One-to-Many relationship added
- **HouseLeader** → **House**: Many-to-One relationship
- **HouseLeader** → **User**: Many-to-One relationship (via UserId)

## Service Layer

### IHouseLeaderService

Methods:
- `GetByUserIdAsync(string userId)` - Get house leader by user ID
- `GetByIdAsync(Guid id)` - Get house leader by ID
- `GetAllAsync()` - Get all house leaders
- `GetByHouseIdAsync(int houseId)` - Get all leaders for a house
- `IsUserHouseLeaderAsync(string userId)` - Check if user is a house leader
- `IsUserHouseLeaderForHouseAsync(string userId, int houseId)` - Check if user leads specific house
- `CreateAsync(HouseLeader houseLeader)` - Register new house leader
- `UpdateAsync(HouseLeader houseLeader)` - Update house leader
- `DeleteAsync(Guid id)` - Remove house leader
- `ExistsAsync(Guid id)` - Check if house leader exists

### IParticipantService

Methods:
- `GetByIdAsync(Guid id)` - Get participant by ID
- `GetAllAsync()` - Get all participants
- `GetByTournamentIdAsync(Guid tournamentId)` - Get participants for tournament
- `GetByHouseIdAsync(int houseId)` - Get participants for house
- `GetByTournamentAndHouseAsync(Guid tournamentId, int houseId)` - Get participants for specific tournament and house
- `CreateAsync(Participant participant)` - Register new participant
- `UpdateAsync(Participant participant)` - Update participant
- `DeleteAsync(Guid id)` - Delete participant
- `ExistsAsync(Guid id)` - Check if participant exists

## Security & Authorization

### Authentication
- All self-registration features require authentication
- Controllers use `[Authorize]` attribute
- Unauthenticated users are redirected to login

### Authorization
- House leaders can only manage their own house's participants
- Controllers verify house ownership before allowing operations
- Attempts to access other houses' data are blocked with error messages

### Audit Trail
- All operations record CreatedBy/UpdatedBy with user ID
- Timestamps (CreatedAt/UpdatedAt) automatically maintained
- Full audit history preserved in database

## User Interface

### Navigation
- **User Menu** (when logged in):
  - "House Leader Dashboard" link added to dropdown
  - Accessible from any page

### House Leader Dashboard
- Shows house name with house color
- Displays active tournament information
- Lists all registered participants with:
  - Name, Gender, Class, Age Group, Age, Points
  - Edit and Delete buttons for each participant
- "Register New Participant" button
- "Unregister as House Leader" button
- "Back to Dashboard" link

### Color Coding
- Dashboard header uses house color
- House badges display with appropriate colors
- Consistent with existing house color scheme

## Error Handling

### Common Scenarios
1. **User not logged in**: Redirect to login page
2. **User not a house leader**: Redirect to registration with info message
3. **No active tournament**: Display warning message
4. **Attempting to access other house's data**: Error message and redirect
5. **Duplicate house leader registration**: Error message with existing house name
6. **Database errors**: Logged and user-friendly error message displayed

### Logging
- All operations logged with ILogger
- Errors logged with full exception details
- Success operations logged with key identifiers

## Testing Checklist

### House Leader Registration
- [ ] User can register as house leader
- [ ] User cannot register for multiple houses
- [ ] Registration creates database record
- [ ] Dashboard displays after registration
- [ ] User can unregister
- [ ] Unregistration removes database record

### Participant Registration
- [ ] House leader can register participants
- [ ] All required fields validated
- [ ] Age calculated correctly from date of birth
- [ ] Participant appears in dashboard
- [ ] House leader can edit participants
- [ ] House leader can delete participants
- [ ] Cannot access other houses' participants

### Security
- [ ] Unauthenticated users redirected to login
- [ ] House leaders cannot access other houses' data
- [ ] All POST actions protected with anti-forgery tokens
- [ ] Audit fields populated correctly

## Future Enhancements

### Potential Improvements
1. **Email Notifications**: Notify house leaders of tournament activation
2. **Bulk Import**: Allow CSV upload for multiple participants
3. **Participant Photos**: Add photo upload capability
4. **House Leader Approval**: Admin approval workflow for house leader registration
5. **Participant Self-Registration**: Allow participants to register themselves with house leader approval
6. **Statistics Dashboard**: Show house performance metrics
7. **Export Functionality**: Export participant lists to Excel/PDF

## Migration Instructions

### For Developers

1. **Install EF Core Tools** (if not already installed):
   ```bash
   dotnet tool install --global dotnet-ef
   ```

2. **Create Migration**:
   ```bash
   cd SportsDay.Web
   dotnet ef migrations add AddHouseLeaderSupport --project ../SportsDay.Lib --startup-project .
   ```

3. **Apply Migration**:
   ```bash
   dotnet ef database update --project ../SportsDay.Lib --startup-project .
   ```

### For Production Deployment

1. Generate SQL script from migration:
   ```bash
   dotnet ef migrations script --project SportsDay.Lib --startup-project SportsDay.Web --output migration.sql
   ```

2. Review and apply SQL script to production database

3. Verify HouseLeaders table created successfully

## Support & Troubleshooting

### Common Issues

**Issue**: "You must be a house leader to register participants"
- **Solution**: Register as a house leader first from the House Leader Dashboard

**Issue**: "No active tournament at this time"
- **Solution**: Wait for administrator to activate a tournament

**Issue**: "You can only register participants for your own house"
- **Solution**: Ensure you're accessing the correct house's participants

**Issue**: Database errors on registration
- **Solution**: Check that migrations have been applied and database is accessible

## Related Documentation

- [Architecture](./architecture.md) - System architecture overview
- [Product Description](./product.md) - Product features and goals
- [Technical Stack](./tech.md) - Technologies used

## Change Log

### Version 1.0 (2024-12-24)
- Initial implementation of house leader self-registration
- Participant management by house leaders
- Service layer for house leaders and participants
- Complete UI for registration and management workflows
- Security and authorization implementation
- Comprehensive error handling and logging