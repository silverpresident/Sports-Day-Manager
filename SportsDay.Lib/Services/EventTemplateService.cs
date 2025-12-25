using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportsDay.Lib.Data;
using SportsDay.Lib.Enums;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Lib.Services;

/// <summary>
/// Service for managing event templates.
/// </summary>
public class EventTemplateService : IEventTemplateService
{
    private readonly SportsDayDbContext _context;
    private readonly ILogger<EventTemplateService> _logger;

    public EventTemplateService(SportsDayDbContext context, ILogger<EventTemplateService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<EventTemplate>> GetAllAsync()
    {
        _logger.LogInformation("Getting all event templates");
        return await _context.EventTemplates
            .AsNoTracking()
            .OrderBy(et => et.ClassGroup)
            .ThenBy(et => et.GenderGroup)
            .ThenBy(et => et.Name)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<EventTemplate>> GetActiveAsync()
    {
        _logger.LogInformation("Getting active event templates");
        return await _context.EventTemplates
            .AsNoTracking()
            .Where(et => et.IsActive)
            .OrderBy(et => et.ClassGroup)
            .ThenBy(et => et.GenderGroup)
            .ThenBy(et => et.Name)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<EventTemplate?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Getting event template with ID: {Id}", id);
        return await _context.EventTemplates
            .AsNoTracking()
            .FirstOrDefaultAsync(et => et.Id == id);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<EventTemplate>> GetByClassGroupAsync(EventClass classGroup)
    {
        _logger.LogInformation("Getting event templates for class group: {ClassGroup}", classGroup);
        return await _context.EventTemplates
            .AsNoTracking()
            .Where(et => et.ClassGroup == classGroup && et.IsActive)
            .OrderBy(et => et.GenderGroup)
            .ThenBy(et => et.Name)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<EventTemplate>> GetByGenderGroupAsync(DivisionType genderGroup)
    {
        _logger.LogInformation("Getting event templates for gender group: {GenderGroup}", genderGroup);
        return await _context.EventTemplates
            .AsNoTracking()
            .Where(et => et.GenderGroup == genderGroup && et.IsActive)
            .OrderBy(et => et.ClassGroup)
            .ThenBy(et => et.Name)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<EventTemplate>> GetByCategoryAsync(string category)
    {
        _logger.LogInformation("Getting event templates for category: {Category}", category);
        return await _context.EventTemplates
            .AsNoTracking()
            .Where(et => et.Category == category && et.IsActive)
            .OrderBy(et => et.ClassGroup)
            .ThenBy(et => et.GenderGroup)
            .ThenBy(et => et.Name)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<EventTemplate> CreateAsync(EventTemplate template)
    {
        _logger.LogInformation("Creating event template: {Name}", template.Name);
        
        template.Id = Guid.NewGuid();
        template.CreatedAt = DateTime.Now;
        
        _context.EventTemplates.Add(template);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Created event template with ID: {Id}", template.Id);
        return template;
    }

    /// <inheritdoc />
    public async Task<EventTemplate> UpdateAsync(EventTemplate template)
    {
        _logger.LogInformation("Updating event template: {Id}", template.Id);
        
        var existing = await _context.EventTemplates.FindAsync(template.Id);
        if (existing == null)
        {
            _logger.LogWarning("Event template not found: {Id}", template.Id);
            throw new InvalidOperationException($"Event template with ID {template.Id} not found.");
        }

        existing.Name = template.Name;
        existing.GenderGroup = template.GenderGroup;
        existing.ClassGroup = template.ClassGroup;
        existing.AgeGroup = template.AgeGroup;
        existing.ParticipantMaxAge = template.ParticipantMaxAge;
        existing.ParticipantLimit = template.ParticipantLimit;
        existing.Category = template.Category;
        existing.Type = template.Type;
        existing.Record = template.Record;
        existing.RecordHolder = template.RecordHolder;
        existing.RecordSettingYear = template.RecordSettingYear;
        existing.RecordNote = template.RecordNote;
        existing.PointSystem = template.PointSystem;
        existing.IsActive = template.IsActive;
        existing.Description = template.Description;
        existing.UpdatedAt = DateTime.Now;
        existing.UpdatedBy = template.UpdatedBy;

        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Updated event template: {Id}", template.Id);
        return existing;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("Deleting event template: {Id}", id);
        
        var template = await _context.EventTemplates.FindAsync(id);
        if (template == null)
        {
            _logger.LogWarning("Event template not found for deletion: {Id}", id);
            return false;
        }

        _context.EventTemplates.Remove(template);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Deleted event template: {Id}", id);
        return true;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Event>> ImportToTournamentAsync(IEnumerable<Guid> templateIds, Guid tournamentId, string createdBy)
    {
        _logger.LogInformation("Importing {Count} templates to tournament: {TournamentId}", 
            templateIds.Count(), tournamentId);
        
        var templates = await _context.EventTemplates
            .Where(et => templateIds.Contains(et.Id))
            .ToListAsync();

        if (!templates.Any())
        {
            _logger.LogWarning("No templates found for import");
            return Enumerable.Empty<Event>();
        }

        var nextEventNumber = await GetNextEventNumberAsync(tournamentId);
        var events = new List<Event>();

        foreach (var template in templates)
        {
            var newEvent = template.ToEvent(tournamentId, nextEventNumber++, createdBy);
            _context.Events.Add(newEvent);
            events.Add(newEvent);
        }

        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Imported {Count} events to tournament: {TournamentId}", 
            events.Count, tournamentId);
        
        return events;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Event>> ImportAllToTournamentAsync(Guid tournamentId, string createdBy)
    {
        _logger.LogInformation("Importing all active templates to tournament: {TournamentId}", tournamentId);
        
        var templates = await _context.EventTemplates
            .Where(et => et.IsActive)
            .OrderBy(et => et.ClassGroup)
            .ThenBy(et => et.GenderGroup)
            .ThenBy(et => et.Name)
            .ToListAsync();

        if (!templates.Any())
        {
            _logger.LogWarning("No active templates found for import");
            return Enumerable.Empty<Event>();
        }

        var nextEventNumber = await GetNextEventNumberAsync(tournamentId);
        var events = new List<Event>();

        foreach (var template in templates)
        {
            var newEvent = template.ToEvent(tournamentId, nextEventNumber++, createdBy);
            _context.Events.Add(newEvent);
            events.Add(newEvent);
        }

        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Imported {Count} events to tournament: {TournamentId}", 
            events.Count, tournamentId);
        
        return events;
    }

    /// <inheritdoc />
    public async Task<int> GetNextEventNumberAsync(Guid tournamentId)
    {
        var maxEventNumber = await _context.Events
            .Where(e => e.TournamentId == tournamentId)
            .MaxAsync(e => (int?)e.EventNumber) ?? 0;
        
        return maxEventNumber + 1;
    }
}