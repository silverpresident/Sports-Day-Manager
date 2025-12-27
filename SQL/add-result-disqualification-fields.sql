-- Migration script to add IsDisqualified and ResultLabel fields to Results table
-- Created: 2025-12-27
-- Description: Adds support for disqualification status and result labels (1st, 2nd, DQ, DNS, DNF)
 
-- Add IsDisqualified column (default to false for existing records)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Results]') AND name = 'IsDisqualified')
BEGIN
    ALTER TABLE [dbo].[Results]
    ADD [IsDisqualified] BIT NOT NULL DEFAULT 0;
    PRINT 'Added IsDisqualified column to Results table';
END
ELSE
BEGIN
    PRINT 'IsDisqualified column already exists in Results table';
END
GO
 
-- Add IsPublished column (default to false for existing records)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Results]') AND name = 'IsPublished')
BEGIN
    ALTER TABLE [dbo].[Results]
    ADD [IsPublished] BIT NOT NULL DEFAULT 0;
    PRINT 'Added IsPublished column to Results table';
END
ELSE
BEGIN
    PRINT 'IsPublished column already exists in Results table';
END
GO
-- Add ResultLabel column (nullable, max 10 characters)
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Results]') AND name = 'ResultLabel')
BEGIN
    ALTER TABLE [dbo].[Results]
    ADD [ResultLabel] NVARCHAR(10) NULL;
    PRINT 'Added ResultLabel column to Results table';
END
ELSE
BEGIN
    PRINT 'ResultLabel column already exists in Results table';
END
GO

-- Update existing records to populate ResultLabel based on Placement
-- This will set labels like "1st", "2nd", "3rd", etc. for existing results
UPDATE [dbo].[Results]
SET [ResultLabel] = 
    CASE 
        WHEN [Placement] = 1 THEN '1st'
        WHEN [Placement] = 2 THEN '2nd'
        WHEN [Placement] = 3 THEN '3rd'
        WHEN [Placement] = 4 THEN '4th'
        WHEN [Placement] = 5 THEN '5th'
        WHEN [Placement] = 6 THEN '6th'
        WHEN [Placement] = 7 THEN '7th'
        WHEN [Placement] = 8 THEN '8th'
        WHEN [Placement] IS NOT NULL THEN CAST([Placement] AS NVARCHAR(10)) + 'th'
        ELSE NULL
    END
WHERE [ResultLabel] IS NULL AND [Placement] IS NOT NULL;
GO

PRINT 'Migration completed successfully';
PRINT 'IsDisqualified and ResultLabel fields added to Results table';
GO