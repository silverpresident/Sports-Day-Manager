-- Migration script to add MaxParticipantsPerHouse and IsPublished columns to Events table
-- Date: 2025-12-27
-- Description: Adds MaxParticipantsPerHouse (int, default 0) and IsPublished (bit, default 0) to Events table
--              Also adds MaxParticipantsPerHouse to EventTemplates table

USE [SportsDay];
GO

-- Check if MaxParticipantsPerHouse column exists in Events table
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[Events]') 
    AND name = 'MaxParticipantsPerHouse'
)
BEGIN
    PRINT 'Adding MaxParticipantsPerHouse column to Events table...';
    ALTER TABLE [dbo].[Events]
    ADD [MaxParticipantsPerHouse] INT NOT NULL DEFAULT 0;
    PRINT 'MaxParticipantsPerHouse column added successfully.';
END
ELSE
BEGIN
    PRINT 'MaxParticipantsPerHouse column already exists in Events table.';
END
GO

-- Check if IsPublished column exists in Events table
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[Events]') 
    AND name = 'IsPublished'
)
BEGIN
    PRINT 'Adding IsPublished column to Events table...';
    ALTER TABLE [dbo].[Events]
    ADD [IsPublished] BIT NOT NULL DEFAULT 0;
    PRINT 'IsPublished column added successfully.';
END
ELSE
BEGIN
    PRINT 'IsPublished column already exists in Events table.';
END
GO

-- Check if MaxParticipantsPerHouse column exists in EventTemplates table
IF NOT EXISTS (
    SELECT * FROM sys.columns 
    WHERE object_id = OBJECT_ID(N'[dbo].[EventTemplates]') 
    AND name = 'MaxParticipantsPerHouse'
)
BEGIN
    PRINT 'Adding MaxParticipantsPerHouse column to EventTemplates table...';
    ALTER TABLE [dbo].[EventTemplates]
    ADD [MaxParticipantsPerHouse] INT NOT NULL DEFAULT 0;
    PRINT 'MaxParticipantsPerHouse column added successfully to EventTemplates.';
END
ELSE
BEGIN
    PRINT 'MaxParticipantsPerHouse column already exists in EventTemplates table.';
END
GO

PRINT 'Migration completed successfully!';
GO