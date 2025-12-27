-- Populate EventClassGroup table with the 5 class groups
-- Class 0 (Open) - No age limit
-- Class 1 - Age ≤ 19
-- Class 2 - Age ≤ 16
-- Class 3 - Age ≤ 14
-- Class 4 - Age ≤ 12

-- Clear existing data (optional - comment out if you want to preserve existing data)
-- DELETE FROM EventClassGroups;

-- Insert Class 0 (Open)
IF NOT EXISTS (SELECT 1 FROM EventClassGroups WHERE ClassGroupNumber = 0)
BEGIN
    INSERT INTO EventClassGroups (ClassGroupNumber, Name, MaxParticipantAge, Description, CreatedAt, CreatedBy)
    VALUES (0, 'Open', 0, 'Open class with no age limit', GETDATE(), 'System');
    PRINT 'Class 0 (Open) inserted';
END
ELSE
BEGIN
    PRINT 'Class 0 (Open) already exists';
END
GO

-- Insert Class 1
IF NOT EXISTS (SELECT 1 FROM EventClassGroups WHERE ClassGroupNumber = 1)
BEGIN
    INSERT INTO EventClassGroups (ClassGroupNumber, Name, MaxParticipantAge, Description, CreatedAt, CreatedBy)
    VALUES (1, 'Class 1', 19, 'Class 1 - Participants aged 19 and under', GETDATE(), 'System');
    PRINT 'Class 1 inserted';
END
ELSE
BEGIN
    PRINT 'Class 1 already exists';
END
GO

-- Insert Class 2
IF NOT EXISTS (SELECT 1 FROM EventClassGroups WHERE ClassGroupNumber = 2)
BEGIN
    INSERT INTO EventClassGroups (ClassGroupNumber, Name, MaxParticipantAge, Description, CreatedAt, CreatedBy)
    VALUES (2, 'Class 2', 16, 'Class 2 - Participants aged 16 and under', GETDATE(), 'System');
    PRINT 'Class 2 inserted';
END
ELSE
BEGIN
    PRINT 'Class 2 already exists';
END
GO

-- Insert Class 3
IF NOT EXISTS (SELECT 1 FROM EventClassGroups WHERE ClassGroupNumber = 3)
BEGIN
    INSERT INTO EventClassGroups (ClassGroupNumber, Name, MaxParticipantAge, Description, CreatedAt, CreatedBy)
    VALUES (3, 'Class 3', 14, 'Class 3 - Participants aged 14 and under', GETDATE(), 'System');
    PRINT 'Class 3 inserted';
END
ELSE
BEGIN
    PRINT 'Class 3 already exists';
END
GO

-- Insert Class 4
IF NOT EXISTS (SELECT 1 FROM EventClassGroups WHERE ClassGroupNumber = 4)
BEGIN
    INSERT INTO EventClassGroups (ClassGroupNumber, Name, MaxParticipantAge, Description, CreatedAt, CreatedBy)
    VALUES (4, 'Class 4', 12, 'Class 4 - Participants aged 12 and under', GETDATE(), 'System');
    PRINT 'Class 4 inserted';
END
ELSE
BEGIN
    PRINT 'Class 4 already exists';
END
GO

-- Verify the data
SELECT ClassGroupNumber, Name, MaxParticipantAge, Description
FROM EventClassGroups
ORDER BY ClassGroupNumber;

PRINT 'EventClassGroup data population completed successfully';