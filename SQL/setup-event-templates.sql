-- =============================================
-- Sports Day Manager - Event Templates Table Setup
-- =============================================
-- This script creates the EventTemplates table for storing
-- event templates that can be copied to create events in tournaments.
-- =============================================

-- Create EventTemplates table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[EventTemplates]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[EventTemplates] (
        [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
        [Name] NVARCHAR(100) NOT NULL,
        [GenderGroup] NVARCHAR(50) NOT NULL,
        [ClassGroup] NVARCHAR(50) NOT NULL,
        [AgeGroup] NVARCHAR(10) NOT NULL,
        [ParticipantMaxAge] INT NOT NULL DEFAULT 0,
        [ParticipantLimit] INT NOT NULL DEFAULT 0,
        [MaxParticipantsPerHouse] INT NOT NULL DEFAULT 0,
        [Category] NVARCHAR(50) NOT NULL,
        [Type] NVARCHAR(50) NOT NULL,
        [Record] DECIMAL(18, 2) NULL,
        [RecordHolder] NVARCHAR(100) NULL,
        [RecordSettingYear] INT NULL,
        [RecordNote] NVARCHAR(500) NULL,
        [PointSystem] NVARCHAR(20) NOT NULL DEFAULT '9,7,6,5,4,3,2,1',
        [IsActive] BIT NOT NULL DEFAULT 1,
        [Description] NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [CreatedBy] NVARCHAR(256) NOT NULL DEFAULT 'system',
        [UpdatedAt] DATETIME2 NULL,
        [UpdatedBy] NVARCHAR(256) NULL
    );

    PRINT 'EventTemplates table created successfully.';
END
ELSE
BEGIN
    PRINT 'EventTemplates table already exists.';
END
GO

-- Create indexes for common queries
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EventTemplates_ClassGroup' AND object_id = OBJECT_ID('EventTemplates'))
BEGIN
    CREATE INDEX [IX_EventTemplates_ClassGroup] ON [dbo].[EventTemplates] ([ClassGroup]);
    PRINT 'Index IX_EventTemplates_ClassGroup created.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EventTemplates_GenderGroup' AND object_id = OBJECT_ID('EventTemplates'))
BEGIN
    CREATE INDEX [IX_EventTemplates_GenderGroup] ON [dbo].[EventTemplates] ([GenderGroup]);
    PRINT 'Index IX_EventTemplates_GenderGroup created.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EventTemplates_Category' AND object_id = OBJECT_ID('EventTemplates'))
BEGIN
    CREATE INDEX [IX_EventTemplates_Category] ON [dbo].[EventTemplates] ([Category]);
    PRINT 'Index IX_EventTemplates_Category created.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EventTemplates_IsActive' AND object_id = OBJECT_ID('EventTemplates'))
BEGIN
    CREATE INDEX [IX_EventTemplates_IsActive] ON [dbo].[EventTemplates] ([IsActive]);
    PRINT 'Index IX_EventTemplates_IsActive created.';
END
GO

PRINT 'Event Templates table setup completed.';
GO