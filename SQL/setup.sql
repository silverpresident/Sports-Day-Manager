-- Create database if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'SportsDay')
BEGIN
    CREATE DATABASE SportsDay;
END
GO

USE SportsDay;
GO

-- Create Houses table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Houses')
BEGIN
    CREATE TABLE Houses (
        Id INT NOT NULL,
        Name NVARCHAR(50) NOT NULL,
        Color NVARCHAR(20) NOT NULL,
        LogoUrl NVARCHAR(255) NULL,
        CONSTRAINT PK_Houses PRIMARY KEY (Id)
    );
END
GO

-- Create Divisions table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Divisions')
BEGIN
    CREATE TABLE Divisions (
        Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        Name NVARCHAR(50) NOT NULL,
        CONSTRAINT PK_Divisions PRIMARY KEY (Id)
    );
END
GO

-- Create Tournaments table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tournaments')
BEGIN
    CREATE TABLE Tournaments (
        Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        Name NVARCHAR(100) NOT NULL,
        [Date] DATETIME2 NOT NULL,
        [Year] INT NOT NULL,
        IsActive BIT NOT NULL DEFAULT 0,
        CONSTRAINT PK_Tournaments PRIMARY KEY (Id)
    );
END
GO

-- Add unique filtered index to ensure only one active tournament
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'UX_Tournaments_IsActive' AND object_id = OBJECT_ID('Tournaments'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_Tournaments_IsActive
    ON Tournaments(IsActive)
    WHERE IsActive = 1;
END
GO

-- Create Events table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Events')
BEGIN
    CREATE TABLE Events (
        Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        Name NVARCHAR(100) NOT NULL,
        Class INT NOT NULL,
        Category NVARCHAR(50) NOT NULL,
        Type INT NOT NULL,
        Record DECIMAL(18,2) NULL,
        RecordHolderName NVARCHAR(100) NULL,
        PointSystem NVARCHAR(20) NOT NULL,
        DivisionId UNIQUEIDENTIFIER NOT NULL,
        TournamentId UNIQUEIDENTIFIER NOT NULL,
        ScheduledTime DATETIME2 NULL,
        Status NVARCHAR(20) NOT NULL,
        CONSTRAINT PK_Events PRIMARY KEY (Id),
        CONSTRAINT FK_Events_Divisions FOREIGN KEY (DivisionId) REFERENCES Divisions(Id),
        CONSTRAINT FK_Events_Tournaments FOREIGN KEY (TournamentId) REFERENCES Tournaments(Id)
    );
END
GO

-- Create Participants table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Participants')
BEGIN
    CREATE TABLE Participants (
        Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        Name NVARCHAR(100) NOT NULL,
        HouseId INT NOT NULL,
        DivisionId UNIQUEIDENTIFIER NOT NULL,
        Gender NVARCHAR(10) NOT NULL,
        Class INT NOT NULL,
        Notes NVARCHAR(500) NULL,
        CONSTRAINT PK_Participants PRIMARY KEY (Id),
        CONSTRAINT FK_Participants_Houses FOREIGN KEY (HouseId) REFERENCES Houses(Id),
        CONSTRAINT FK_Participants_Divisions FOREIGN KEY (DivisionId) REFERENCES Divisions(Id)
    );
END
GO

-- Create Results table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Results')
BEGIN
    CREATE TABLE Results (
        Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        EventId UNIQUEIDENTIFIER NOT NULL,
        ParticipantId UNIQUEIDENTIFIER NOT NULL,
        HouseId INT NOT NULL,
        Placement INT NULL,
        SpeedOrDistance DECIMAL(18,2) NULL,
        Points INT NOT NULL,
        IsNewRecord BIT NOT NULL,
        TournamentId UNIQUEIDENTIFIER NOT NULL,
        CONSTRAINT PK_Results PRIMARY KEY (Id),
        CONSTRAINT FK_Results_Events FOREIGN KEY (EventId) REFERENCES Events(Id),
        CONSTRAINT FK_Results_Participants FOREIGN KEY (ParticipantId) REFERENCES Participants(Id),
        CONSTRAINT FK_Results_Houses FOREIGN KEY (HouseId) REFERENCES Houses(Id),
        CONSTRAINT FK_Results_Tournaments FOREIGN KEY (TournamentId) REFERENCES Tournaments(Id)
    );
END
GO

-- Create Announcements table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Announcements')
BEGIN
    CREATE TABLE Announcements (
        Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        Body NVARCHAR(500) NOT NULL,
        Priority INT NOT NULL,
        CreatedAt DATETIME2 NOT NULL,
        ExpiresAt DATETIME2 NULL,
        IsEnabled BIT NOT NULL,
        TournamentId UNIQUEIDENTIFIER NOT NULL,
        CONSTRAINT PK_Announcements PRIMARY KEY (Id),
        CONSTRAINT FK_Announcements_Tournaments FOREIGN KEY (TournamentId) REFERENCES Tournaments(Id)
    );
END
GO

-- Create EventUpdates table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EventUpdates')
BEGIN
    CREATE TABLE EventUpdates (
        Id UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        Message NVARCHAR(500) NOT NULL,
        CreatedAt DATETIME2 NOT NULL,
        EventId UNIQUEIDENTIFIER NOT NULL,
        TournamentId UNIQUEIDENTIFIER NOT NULL,
        CONSTRAINT PK_EventUpdates PRIMARY KEY (Id),
        CONSTRAINT FK_EventUpdates_Events FOREIGN KEY (EventId) REFERENCES Events(Id),
        CONSTRAINT FK_EventUpdates_Tournaments FOREIGN KEY (TournamentId) REFERENCES Tournaments(Id)
    );
END
GO

-- Insert default houses if they don't exist
IF NOT EXISTS (SELECT * FROM Houses)
BEGIN
    INSERT INTO Houses (Id, Name, Color) VALUES
    (1, 'Beckford', 'Red'),
    (2, 'Bell', 'Green'),
    (3, 'Campbell', 'Orange'),
    (4, 'Nutall', 'Purple'),
    (5, 'Smith', 'Blue'),
    (6, 'Wortley', 'Yellow');
END
GO

-- Insert default divisions if they don't exist
IF NOT EXISTS (SELECT * FROM Divisions)
BEGIN
    INSERT INTO Divisions (Name) VALUES
    ('BOYS'),
    ('GIRLS');
END
GO
