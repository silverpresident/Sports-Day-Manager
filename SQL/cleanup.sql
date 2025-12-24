--This script removes all non-identity tables from the database

-- Drop dependent tables first to avoid foreign key constraint errors
DROP TABLE IF EXISTS [EventUpdates];
DROP TABLE IF EXISTS [Results];
DROP TABLE IF EXISTS [Participants];
DROP TABLE IF EXISTS [HouseLeaders];
DROP TABLE IF EXISTS [Events];
DROP TABLE IF EXISTS [Announcements];

-- Drop parent tables last
DROP TABLE IF EXISTS [Tournaments];
DROP TABLE IF EXISTS [Houses];
