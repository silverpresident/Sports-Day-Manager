-- =============================================
-- Sports Day Manager - Event Templates Population
-- =============================================
-- This script populates the EventTemplates table with standard
-- Jamaican track and field events for Classes 1-4 and Open,
-- across Boys, Girls, and Open divisions.
-- =============================================

-- Clear existing templates (optional - comment out if you want to preserve existing data)
-- DELETE FROM [dbo].[EventTemplates];
-- GO

-- =============================================
-- CLASS 4 EVENTS (Youngest - typically ages 12-13)
-- =============================================

-- Class 4 Boys - Track Events
INSERT INTO [dbo].[EventTemplates] ([Id], [Name], [GenderGroup], [ClassGroup], [AgeGroup], [ParticipantMaxAge], [Category], [Type], [PointSystem], [IsActive], [Description])
VALUES 
    (NEWID(), '100m', 'Boys', 'Class4', 'U14', 14, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 4 Boys 100 meters sprint'),
    (NEWID(), '200m', 'Boys', 'Class4', 'U14', 14, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 4 Boys 200 meters sprint'),
    (NEWID(), '400m', 'Boys', 'Class4', 'U14', 14, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 4 Boys 400 meters'),
    (NEWID(), '800m', 'Boys', 'Class4', 'U14', 14, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 4 Boys 800 meters'),
    (NEWID(), '1500m', 'Boys', 'Class4', 'U14', 14, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 4 Boys 1500 meters'),
    (NEWID(), '70m Hurdles', 'Boys', 'Class4', 'U14', 14, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 4 Boys 70 meters hurdles'),
    (NEWID(), '4x100m Relay', 'Boys', 'Class4', 'U14', 14, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Class 4 Boys 4x100 meters relay');

-- Class 4 Boys - Field Events
INSERT INTO [dbo].[EventTemplates] ([Id], [Name], [GenderGroup], [ClassGroup], [AgeGroup], [ParticipantMaxAge], [Category], [Type], [PointSystem], [IsActive], [Description])
VALUES 
    (NEWID(), 'High Jump', 'Boys', 'Class4', 'U14', 14, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 4 Boys High Jump'),
    (NEWID(), 'Long Jump', 'Boys', 'Class4', 'U14', 14, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 4 Boys Long Jump'),
    (NEWID(), 'Shot Put', 'Boys', 'Class4', 'U14', 14, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 4 Boys Shot Put'),
    (NEWID(), 'Discus', 'Boys', 'Class4', 'U14', 14, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 4 Boys Discus Throw');

-- Class 4 Girls - Track Events
INSERT INTO [dbo].[EventTemplates] ([Id], [Name], [GenderGroup], [ClassGroup], [AgeGroup], [ParticipantMaxAge], [Category], [Type], [PointSystem], [IsActive], [Description])
VALUES 
    (NEWID(), '100m', 'Girls', 'Class4', 'U14', 14, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 4 Girls 100 meters sprint'),
    (NEWID(), '200m', 'Girls', 'Class4', 'U14', 14, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 4 Girls 200 meters sprint'),
    (NEWID(), '400m', 'Girls', 'Class4', 'U14', 14, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 4 Girls 400 meters'),
    (NEWID(), '800m', 'Girls', 'Class4', 'U14', 14, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 4 Girls 800 meters'),
    (NEWID(), '1500m', 'Girls', 'Class4', 'U14', 14, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 4 Girls 1500 meters'),
    (NEWID(), '70m Hurdles', 'Girls', 'Class4', 'U14', 14, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 4 Girls 70 meters hurdles'),
    (NEWID(), '4x100m Relay', 'Girls', 'Class4', 'U14', 14, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Class 4 Girls 4x100 meters relay');

-- Class 4 Girls - Field Events
INSERT INTO [dbo].[EventTemplates] ([Id], [Name], [GenderGroup], [ClassGroup], [AgeGroup], [ParticipantMaxAge], [Category], [Type], [PointSystem], [IsActive], [Description])
VALUES 
    (NEWID(), 'High Jump', 'Girls', 'Class4', 'U14', 14, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 4 Girls High Jump'),
    (NEWID(), 'Long Jump', 'Girls', 'Class4', 'U14', 14, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 4 Girls Long Jump'),
    (NEWID(), 'Shot Put', 'Girls', 'Class4', 'U14', 14, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 4 Girls Shot Put'),
    (NEWID(), 'Discus', 'Girls', 'Class4', 'U14', 14, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 4 Girls Discus Throw');

-- =============================================
-- CLASS 3 EVENTS (typically ages 14-15)
-- =============================================

-- Class 3 Boys - Track Events
INSERT INTO [dbo].[EventTemplates] ([Id], [Name], [GenderGroup], [ClassGroup], [AgeGroup], [ParticipantMaxAge], [Category], [Type], [PointSystem], [IsActive], [Description])
VALUES 
    (NEWID(), '100m', 'Boys', 'Class3', 'U16', 16, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 3 Boys 100 meters sprint'),
    (NEWID(), '200m', 'Boys', 'Class3', 'U16', 16, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 3 Boys 200 meters sprint'),
    (NEWID(), '400m', 'Boys', 'Class3', 'U16', 16, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 3 Boys 400 meters'),
    (NEWID(), '800m', 'Boys', 'Class3', 'U16', 16, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 3 Boys 800 meters'),
    (NEWID(), '1500m', 'Boys', 'Class3', 'U16', 16, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 3 Boys 1500 meters'),
    (NEWID(), '80m Hurdles', 'Boys', 'Class3', 'U16', 16, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 3 Boys 80 meters hurdles'),
    (NEWID(), '4x100m Relay', 'Boys', 'Class3', 'U16', 16, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Class 3 Boys 4x100 meters relay'),
    (NEWID(), '4x400m Relay', 'Boys', 'Class3', 'U16', 16, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Class 3 Boys 4x400 meters relay');

-- Class 3 Boys - Field Events
INSERT INTO [dbo].[EventTemplates] ([Id], [Name], [GenderGroup], [ClassGroup], [AgeGroup], [ParticipantMaxAge], [Category], [Type], [PointSystem], [IsActive], [Description])
VALUES 
    (NEWID(), 'High Jump', 'Boys', 'Class3', 'U16', 16, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 3 Boys High Jump'),
    (NEWID(), 'Long Jump', 'Boys', 'Class3', 'U16', 16, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 3 Boys Long Jump'),
    (NEWID(), 'Triple Jump', 'Boys', 'Class3', 'U16', 16, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 3 Boys Triple Jump'),
    (NEWID(), 'Shot Put', 'Boys', 'Class3', 'U16', 16, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 3 Boys Shot Put'),
    (NEWID(), 'Discus', 'Boys', 'Class3', 'U16', 16, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 3 Boys Discus Throw'),
    (NEWID(), 'Javelin', 'Boys', 'Class3', 'U16', 16, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 3 Boys Javelin Throw');

-- Class 3 Girls - Track Events
INSERT INTO [dbo].[EventTemplates] ([Id], [Name], [GenderGroup], [ClassGroup], [AgeGroup], [ParticipantMaxAge], [Category], [Type], [PointSystem], [IsActive], [Description])
VALUES 
    (NEWID(), '100m', 'Girls', 'Class3', 'U16', 16, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 3 Girls 100 meters sprint'),
    (NEWID(), '200m', 'Girls', 'Class3', 'U16', 16, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 3 Girls 200 meters sprint'),
    (NEWID(), '400m', 'Girls', 'Class3', 'U16', 16, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 3 Girls 400 meters'),
    (NEWID(), '800m', 'Girls', 'Class3', 'U16', 16, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 3 Girls 800 meters'),
    (NEWID(), '1500m', 'Girls', 'Class3', 'U16', 16, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 3 Girls 1500 meters'),
    (NEWID(), '80m Hurdles', 'Girls', 'Class3', 'U16', 16, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 3 Girls 80 meters hurdles'),
    (NEWID(), '4x100m Relay', 'Girls', 'Class3', 'U16', 16, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Class 3 Girls 4x100 meters relay'),
    (NEWID(), '4x400m Relay', 'Girls', 'Class3', 'U16', 16, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Class 3 Girls 4x400 meters relay');

-- Class 3 Girls - Field Events
INSERT INTO [dbo].[EventTemplates] ([Id], [Name], [GenderGroup], [ClassGroup], [AgeGroup], [ParticipantMaxAge], [Category], [Type], [PointSystem], [IsActive], [Description])
VALUES 
    (NEWID(), 'High Jump', 'Girls', 'Class3', 'U16', 16, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 3 Girls High Jump'),
    (NEWID(), 'Long Jump', 'Girls', 'Class3', 'U16', 16, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 3 Girls Long Jump'),
    (NEWID(), 'Triple Jump', 'Girls', 'Class3', 'U16', 16, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 3 Girls Triple Jump'),
    (NEWID(), 'Shot Put', 'Girls', 'Class3', 'U16', 16, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 3 Girls Shot Put'),
    (NEWID(), 'Discus', 'Girls', 'Class3', 'U16', 16, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 3 Girls Discus Throw'),
    (NEWID(), 'Javelin', 'Girls', 'Class3', 'U16', 16, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 3 Girls Javelin Throw');

-- =============================================
-- CLASS 2 EVENTS (typically ages 16-17)
-- =============================================

-- Class 2 Boys - Track Events
INSERT INTO [dbo].[EventTemplates] ([Id], [Name], [GenderGroup], [ClassGroup], [AgeGroup], [ParticipantMaxAge], [Category], [Type], [PointSystem], [IsActive], [Description])
VALUES 
    (NEWID(), '100m', 'Boys', 'Class2', 'U18', 18, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 2 Boys 100 meters sprint'),
    (NEWID(), '200m', 'Boys', 'Class2', 'U18', 18, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 2 Boys 200 meters sprint'),
    (NEWID(), '400m', 'Boys', 'Class2', 'U18', 18, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 2 Boys 400 meters'),
    (NEWID(), '800m', 'Boys', 'Class2', 'U18', 18, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 2 Boys 800 meters'),
    (NEWID(), '1500m', 'Boys', 'Class2', 'U18', 18, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 2 Boys 1500 meters'),
    (NEWID(), '3000m', 'Boys', 'Class2', 'U18', 18, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 2 Boys 3000 meters'),
    (NEWID(), '100m Hurdles', 'Boys', 'Class2', 'U18', 18, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 2 Boys 100 meters hurdles'),
    (NEWID(), '400m Hurdles', 'Boys', 'Class2', 'U18', 18, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 2 Boys 400 meters hurdles'),
    (NEWID(), '4x100m Relay', 'Boys', 'Class2', 'U18', 18, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Class 2 Boys 4x100 meters relay'),
    (NEWID(), '4x400m Relay', 'Boys', 'Class2', 'U18', 18, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Class 2 Boys 4x400 meters relay'),
    (NEWID(), 'Medley Relay', 'Boys', 'Class2', 'U18', 18, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Class 2 Boys Medley Relay (200-200-400-800)');

-- Class 2 Boys - Field Events
INSERT INTO [dbo].[EventTemplates] ([Id], [Name], [GenderGroup], [ClassGroup], [AgeGroup], [ParticipantMaxAge], [Category], [Type], [PointSystem], [IsActive], [Description])
VALUES 
    (NEWID(), 'High Jump', 'Boys', 'Class2', 'U18', 18, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 2 Boys High Jump'),
    (NEWID(), 'Long Jump', 'Boys', 'Class2', 'U18', 18, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 2 Boys Long Jump'),
    (NEWID(), 'Triple Jump', 'Boys', 'Class2', 'U18', 18, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 2 Boys Triple Jump'),
    (NEWID(), 'Shot Put', 'Boys', 'Class2', 'U18', 18, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 2 Boys Shot Put'),
    (NEWID(), 'Discus', 'Boys', 'Class2', 'U18', 18, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 2 Boys Discus Throw'),
    (NEWID(), 'Javelin', 'Boys', 'Class2', 'U18', 18, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 2 Boys Javelin Throw');

-- Class 2 Girls - Track Events
INSERT INTO [dbo].[EventTemplates] ([Id], [Name], [GenderGroup], [ClassGroup], [AgeGroup], [ParticipantMaxAge], [Category], [Type], [PointSystem], [IsActive], [Description])
VALUES 
    (NEWID(), '100m', 'Girls', 'Class2', 'U18', 18, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 2 Girls 100 meters sprint'),
    (NEWID(), '200m', 'Girls', 'Class2', 'U18', 18, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 2 Girls 200 meters sprint'),
    (NEWID(), '400m', 'Girls', 'Class2', 'U18', 18, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 2 Girls 400 meters'),
    (NEWID(), '800m', 'Girls', 'Class2', 'U18', 18, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 2 Girls 800 meters'),
    (NEWID(), '1500m', 'Girls', 'Class2', 'U18', 18, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 2 Girls 1500 meters'),
    (NEWID(), '3000m', 'Girls', 'Class2', 'U18', 18, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 2 Girls 3000 meters'),
    (NEWID(), '100m Hurdles', 'Girls', 'Class2', 'U18', 18, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 2 Girls 100 meters hurdles'),
    (NEWID(), '400m Hurdles', 'Girls', 'Class2', 'U18', 18, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 2 Girls 400 meters hurdles'),
    (NEWID(), '4x100m Relay', 'Girls', 'Class2', 'U18', 18, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Class 2 Girls 4x100 meters relay'),
    (NEWID(), '4x400m Relay', 'Girls', 'Class2', 'U18', 18, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Class 2 Girls 4x400 meters relay'),
    (NEWID(), 'Medley Relay', 'Girls', 'Class2', 'U18', 18, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Class 2 Girls Medley Relay (200-200-400-800)');

-- Class 2 Girls - Field Events
INSERT INTO [dbo].[EventTemplates] ([Id], [Name], [GenderGroup], [ClassGroup], [AgeGroup], [ParticipantMaxAge], [Category], [Type], [PointSystem], [IsActive], [Description])
VALUES 
    (NEWID(), 'High Jump', 'Girls', 'Class2', 'U18', 18, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 2 Girls High Jump'),
    (NEWID(), 'Long Jump', 'Girls', 'Class2', 'U18', 18, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 2 Girls Long Jump'),
    (NEWID(), 'Triple Jump', 'Girls', 'Class2', 'U18', 18, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 2 Girls Triple Jump'),
    (NEWID(), 'Shot Put', 'Girls', 'Class2', 'U18', 18, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 2 Girls Shot Put'),
    (NEWID(), 'Discus', 'Girls', 'Class2', 'U18', 18, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 2 Girls Discus Throw'),
    (NEWID(), 'Javelin', 'Girls', 'Class2', 'U18', 18, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 2 Girls Javelin Throw');

-- =============================================
-- CLASS 1 EVENTS (Oldest - typically ages 18-19)
-- =============================================

-- Class 1 Boys - Track Events
INSERT INTO [dbo].[EventTemplates] ([Id], [Name], [GenderGroup], [ClassGroup], [AgeGroup], [ParticipantMaxAge], [Category], [Type], [PointSystem], [IsActive], [Description])
VALUES 
    (NEWID(), '100m', 'Boys', 'Class1', 'U20', 20, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 1 Boys 100 meters sprint'),
    (NEWID(), '200m', 'Boys', 'Class1', 'U20', 20, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 1 Boys 200 meters sprint'),
    (NEWID(), '400m', 'Boys', 'Class1', 'U20', 20, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 1 Boys 400 meters'),
    (NEWID(), '800m', 'Boys', 'Class1', 'U20', 20, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 1 Boys 800 meters'),
    (NEWID(), '1500m', 'Boys', 'Class1', 'U20', 20, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 1 Boys 1500 meters'),
    (NEWID(), '3000m', 'Boys', 'Class1', 'U20', 20, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 1 Boys 3000 meters'),
    (NEWID(), '110m Hurdles', 'Boys', 'Class1', 'U20', 20, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 1 Boys 110 meters hurdles'),
    (NEWID(), '400m Hurdles', 'Boys', 'Class1', 'U20', 20, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 1 Boys 400 meters hurdles'),
    (NEWID(), '4x100m Relay', 'Boys', 'Class1', 'U20', 20, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Class 1 Boys 4x100 meters relay'),
    (NEWID(), '4x400m Relay', 'Boys', 'Class1', 'U20', 20, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Class 1 Boys 4x400 meters relay'),
    (NEWID(), 'Medley Relay', 'Boys', 'Class1', 'U20', 20, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Class 1 Boys Medley Relay (200-200-400-800)');

-- Class 1 Boys - Field Events
INSERT INTO [dbo].[EventTemplates] ([Id], [Name], [GenderGroup], [ClassGroup], [AgeGroup], [ParticipantMaxAge], [Category], [Type], [PointSystem], [IsActive], [Description])
VALUES 
    (NEWID(), 'High Jump', 'Boys', 'Class1', 'U20', 20, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 1 Boys High Jump'),
    (NEWID(), 'Long Jump', 'Boys', 'Class1', 'U20', 20, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 1 Boys Long Jump'),
    (NEWID(), 'Triple Jump', 'Boys', 'Class1', 'U20', 20, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 1 Boys Triple Jump'),
    (NEWID(), 'Shot Put', 'Boys', 'Class1', 'U20', 20, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 1 Boys Shot Put'),
    (NEWID(), 'Discus', 'Boys', 'Class1', 'U20', 20, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 1 Boys Discus Throw'),
    (NEWID(), 'Javelin', 'Boys', 'Class1', 'U20', 20, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 1 Boys Javelin Throw'),
    (NEWID(), 'Decathlon', 'Boys', 'Class1', 'U20', 20, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 1 Boys Decathlon (Multi-event)');

-- Class 1 Girls - Track Events
INSERT INTO [dbo].[EventTemplates] ([Id], [Name], [GenderGroup], [ClassGroup], [AgeGroup], [ParticipantMaxAge], [Category], [Type], [PointSystem], [IsActive], [Description])
VALUES 
    (NEWID(), '100m', 'Girls', 'Class1', 'U20', 20, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 1 Girls 100 meters sprint'),
    (NEWID(), '200m', 'Girls', 'Class1', 'U20', 20, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 1 Girls 200 meters sprint'),
    (NEWID(), '400m', 'Girls', 'Class1', 'U20', 20, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 1 Girls 400 meters'),
    (NEWID(), '800m', 'Girls', 'Class1', 'U20', 20, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 1 Girls 800 meters'),
    (NEWID(), '1500m', 'Girls', 'Class1', 'U20', 20, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 1 Girls 1500 meters'),
    (NEWID(), '3000m', 'Girls', 'Class1', 'U20', 20, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 1 Girls 3000 meters'),
    (NEWID(), '100m Hurdles', 'Girls', 'Class1', 'U20', 20, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 1 Girls 100 meters hurdles'),
    (NEWID(), '400m Hurdles', 'Girls', 'Class1', 'U20', 20, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Class 1 Girls 400 meters hurdles'),
    (NEWID(), '4x100m Relay', 'Girls', 'Class1', 'U20', 20, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Class 1 Girls 4x100 meters relay'),
    (NEWID(), '4x400m Relay', 'Girls', 'Class1', 'U20', 20, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Class 1 Girls 4x400 meters relay'),
    (NEWID(), 'Medley Relay', 'Girls', 'Class1', 'U20', 20, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Class 1 Girls Medley Relay (200-200-400-800)');

-- Class 1 Girls - Field Events
INSERT INTO [dbo].[EventTemplates] ([Id], [Name], [GenderGroup], [ClassGroup], [AgeGroup], [ParticipantMaxAge], [Category], [Type], [PointSystem], [IsActive], [Description])
VALUES 
    (NEWID(), 'High Jump', 'Girls', 'Class1', 'U20', 20, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 1 Girls High Jump'),
    (NEWID(), 'Long Jump', 'Girls', 'Class1', 'U20', 20, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 1 Girls Long Jump'),
    (NEWID(), 'Triple Jump', 'Girls', 'Class1', 'U20', 20, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 1 Girls Triple Jump'),
    (NEWID(), 'Shot Put', 'Girls', 'Class1', 'U20', 20, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 1 Girls Shot Put'),
    (NEWID(), 'Discus', 'Girls', 'Class1', 'U20', 20, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 1 Girls Discus Throw'),
    (NEWID(), 'Javelin', 'Girls', 'Class1', 'U20', 20, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 1 Girls Javelin Throw'),
    (NEWID(), 'Heptathlon', 'Girls', 'Class1', 'U20', 20, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Class 1 Girls Heptathlon (Multi-event)');

-- =============================================
-- OPEN CLASS EVENTS (All ages eligible)
-- =============================================

-- Open Boys - Track Events
INSERT INTO [dbo].[EventTemplates] ([Id], [Name], [GenderGroup], [ClassGroup], [AgeGroup], [ParticipantMaxAge], [Category], [Type], [PointSystem], [IsActive], [Description])
VALUES 
    (NEWID(), '100m', 'Boys', 'Open', 'Open', 0, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Open Boys 100 meters sprint'),
    (NEWID(), '200m', 'Boys', 'Open', 'Open', 0, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Open Boys 200 meters sprint'),
    (NEWID(), '400m', 'Boys', 'Open', 'Open', 0, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Open Boys 400 meters'),
    (NEWID(), '800m', 'Boys', 'Open', 'Open', 0, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Open Boys 800 meters'),
    (NEWID(), '1500m', 'Boys', 'Open', 'Open', 0, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Open Boys 1500 meters'),
    (NEWID(), '3000m', 'Boys', 'Open', 'Open', 0, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Open Boys 3000 meters'),
    (NEWID(), '110m Hurdles', 'Boys', 'Open', 'Open', 0, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Open Boys 110 meters hurdles'),
    (NEWID(), '400m Hurdles', 'Boys', 'Open', 'Open', 0, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Open Boys 400 meters hurdles'),
    (NEWID(), '4x100m Relay', 'Boys', 'Open', 'Open', 0, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Open Boys 4x100 meters relay'),
    (NEWID(), '4x400m Relay', 'Boys', 'Open', 'Open', 0, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Open Boys 4x400 meters relay');

-- Open Boys - Field Events
INSERT INTO [dbo].[EventTemplates] ([Id], [Name], [GenderGroup], [ClassGroup], [AgeGroup], [ParticipantMaxAge], [Category], [Type], [PointSystem], [IsActive], [Description])
VALUES 
    (NEWID(), 'High Jump', 'Boys', 'Open', 'Open', 0, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Open Boys High Jump'),
    (NEWID(), 'Long Jump', 'Boys', 'Open', 'Open', 0, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Open Boys Long Jump'),
    (NEWID(), 'Triple Jump', 'Boys', 'Open', 'Open', 0, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Open Boys Triple Jump'),
    (NEWID(), 'Shot Put', 'Boys', 'Open', 'Open', 0, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Open Boys Shot Put'),
    (NEWID(), 'Discus', 'Boys', 'Open', 'Open', 0, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Open Boys Discus Throw'),
    (NEWID(), 'Javelin', 'Boys', 'Open', 'Open', 0, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Open Boys Javelin Throw');

-- Open Girls - Track Events
INSERT INTO [dbo].[EventTemplates] ([Id], [Name], [GenderGroup], [ClassGroup], [AgeGroup], [ParticipantMaxAge], [Category], [Type], [PointSystem], [IsActive], [Description])
VALUES 
    (NEWID(), '100m', 'Girls', 'Open', 'Open', 0, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Open Girls 100 meters sprint'),
    (NEWID(), '200m', 'Girls', 'Open', 'Open', 0, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Open Girls 200 meters sprint'),
    (NEWID(), '400m', 'Girls', 'Open', 'Open', 0, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Open Girls 400 meters'),
    (NEWID(), '800m', 'Girls', 'Open', 'Open', 0, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Open Girls 800 meters'),
    (NEWID(), '1500m', 'Girls', 'Open', 'Open', 0, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Open Girls 1500 meters'),
    (NEWID(), '3000m', 'Girls', 'Open', 'Open', 0, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Open Girls 3000 meters'),
    (NEWID(), '100m Hurdles', 'Girls', 'Open', 'Open', 0, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Open Girls 100 meters hurdles'),
    (NEWID(), '400m Hurdles', 'Girls', 'Open', 'Open', 0, 'Track', 'Speed', '9,7,6,5,4,3,2,1', 1, 'Open Girls 400 meters hurdles'),
    (NEWID(), '4x100m Relay', 'Girls', 'Open', 'Open', 0, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Open Girls 4x100 meters relay'),
    (NEWID(), '4x400m Relay', 'Girls', 'Open', 'Open', 0, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Open Girls 4x400 meters relay');

-- Open Girls - Field Events
INSERT INTO [dbo].[EventTemplates] ([Id], [Name], [GenderGroup], [ClassGroup], [AgeGroup], [ParticipantMaxAge], [Category], [Type], [PointSystem], [IsActive], [Description])
VALUES 
    (NEWID(), 'High Jump', 'Girls', 'Open', 'Open', 0, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Open Girls High Jump'),
    (NEWID(), 'Long Jump', 'Girls', 'Open', 'Open', 0, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Open Girls Long Jump'),
    (NEWID(), 'Triple Jump', 'Girls', 'Open', 'Open', 0, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Open Girls Triple Jump'),
    (NEWID(), 'Shot Put', 'Girls', 'Open', 'Open', 0, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Open Girls Shot Put'),
    (NEWID(), 'Discus', 'Girls', 'Open', 'Open', 0, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Open Girls Discus Throw'),
    (NEWID(), 'Javelin', 'Girls', 'Open', 'Open', 0, 'Field', 'Distance', '9,7,6,5,4,3,2,1', 1, 'Open Girls Javelin Throw');

-- Open Mixed - Special Events
INSERT INTO [dbo].[EventTemplates] ([Id], [Name], [GenderGroup], [ClassGroup], [AgeGroup], [ParticipantMaxAge], [Category], [Type], [PointSystem], [IsActive], [Description])
VALUES 
    (NEWID(), 'Mixed 4x100m Relay', 'Open', 'Open', 'Open', 0, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Open Mixed 4x100 meters relay (2 boys, 2 girls)'),
    (NEWID(), 'Mixed 4x400m Relay', 'Open', 'Open', 'Open', 0, 'Track', 'Speed', '12,10,9,8,7,6,1', 1, 'Open Mixed 4x400 meters relay (2 boys, 2 girls)');

-- =============================================
-- Summary
-- =============================================
PRINT 'Event Templates population completed.';
PRINT 'Total templates created: Approximately 150+ events across all classes and genders.';
GO

-- Verify the count
SELECT 
    ClassGroup,
    GenderGroup,
    COUNT(*) as EventCount
FROM [dbo].[EventTemplates]
GROUP BY ClassGroup, GenderGroup
ORDER BY ClassGroup, GenderGroup;
GO