-- Create EventClassGroup table
-- This table stores the class group definitions for events and participants

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EventClassGroups')
BEGIN
    CREATE TABLE EventClassGroups (
        ClassGroupNumber INT NOT NULL PRIMARY KEY,
        Name NVARCHAR(50) NOT NULL,
        MaxParticipantAge INT NOT NULL,
        Description NVARCHAR(500) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        CreatedBy NVARCHAR(256) NULL,
        UpdatedAt DATETIME2 NULL,
        UpdatedBy NVARCHAR(256) NULL
    );

    PRINT 'EventClassGroups table created successfully';
END
ELSE
BEGIN
    PRINT 'EventClassGroups table already exists';
END
GO

-- Add ClassGroupNumber column to Events table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Events') AND name = 'ClassGroupNumber')
BEGIN
    ALTER TABLE Events ADD ClassGroupNumber INT NOT NULL DEFAULT 0;
    PRINT 'ClassGroupNumber column added to Events table';
END
ELSE
BEGIN
    PRINT 'ClassGroupNumber column already exists in Events table';
END
GO

-- Add ClassGroupNumber column to EventTemplates table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('EventTemplates') AND name = 'ClassGroupNumber')
BEGIN
    ALTER TABLE EventTemplates ADD ClassGroupNumber INT NOT NULL DEFAULT 0;
    PRINT 'ClassGroupNumber column added to EventTemplates table';
END
ELSE
BEGIN
    PRINT 'ClassGroupNumber column already exists in EventTemplates table';
END
GO

-- Add ClassGroupNumber column to Participants table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Participants') AND name = 'ClassGroupNumber')
BEGIN
    ALTER TABLE Participants ADD ClassGroupNumber INT NOT NULL DEFAULT 0;
    PRINT 'ClassGroupNumber column added to Participants table';
END
ELSE
BEGIN
    PRINT 'ClassGroupNumber column already exists in Participants table';
END
GO

-- Add ClassGroupNumber column to TournamentHouseSummaries table
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('TournamentHouseSummaries') AND name = 'ClassGroupNumber')
BEGIN
    ALTER TABLE TournamentHouseSummaries ADD ClassGroupNumber INT NOT NULL DEFAULT 0;
    PRINT 'ClassGroupNumber column added to TournamentHouseSummaries table';
END
ELSE
BEGIN
    PRINT 'ClassGroupNumber column already exists in TournamentHouseSummaries table';
END
GO

PRINT 'EventClassGroup migration completed successfully';