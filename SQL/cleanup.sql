USE master;
GO

-- Drop database if it exists
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'SportsDay')
BEGIN
    ALTER DATABASE SportsDay SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE SportsDay;
END
GO
