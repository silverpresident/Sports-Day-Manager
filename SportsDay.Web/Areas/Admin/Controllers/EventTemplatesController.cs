using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SportsDay.Lib.Data;
using SportsDay.Lib.Enums;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;
using SportsDay.Web.Hubs;

namespace SportsDay.Web.Areas.Admin.Controllers;

/// <summary>
/// Controller for managing event templates in the admin area.
/// </summary>
[Area("Admin")]
//[Authorize(Roles = "Administrator")]
public class EventTemplatesController : Controller
{
    private readonly IEventTemplateService _eventTemplateService;
    private readonly ITournamentService _tournamentService;
    private readonly IHubContext<SportsHub> _hubContext;
    private readonly ILogger<EventTemplatesController> _logger;

    public EventTemplatesController(
        IEventTemplateService eventTemplateService,
        ITournamentService tournamentService,
        IHubContext<SportsHub> hubContext,
        ILogger<EventTemplatesController> logger)
    {
        _eventTemplateService = eventTemplateService;
        _tournamentService = tournamentService;
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <summary>
    /// Displays the list of all event templates.
    /// </summary>
    public async Task<IActionResult> Index(string? classFilter, string? genderFilter, string? categoryFilter)
    {
        _logger.LogInformation("Loading event templates index with filters - Class: {ClassFilter}, Gender: {GenderFilter}, Category: {CategoryFilter}",
            classFilter, genderFilter, categoryFilter);

        var templates = await _eventTemplateService.GetAllAsync();

        // Apply filters
        if (!string.IsNullOrEmpty(classFilter) && Enum.TryParse<EventClass>(classFilter, out var parsedClass))
        {
            templates = templates.Where(t => t.ClassGroup == parsedClass);
        }

        if (!string.IsNullOrEmpty(genderFilter) && Enum.TryParse<DivisionType>(genderFilter, out var parsedGender))
        {
            templates = templates.Where(t => t.GenderGroup == parsedGender);
        }

        if (!string.IsNullOrEmpty(categoryFilter))
        {
            templates = templates.Where(t => t.Category.Equals(categoryFilter, StringComparison.OrdinalIgnoreCase));
        }

        ViewBag.CurrentClassFilter = classFilter;
        ViewBag.CurrentGenderFilter = genderFilter;
        ViewBag.CurrentCategoryFilter = categoryFilter;
        PopulateFilterDropdowns();

        return View(templates.ToList());
    }

    /// <summary>
    /// Displays the create event template form.
    /// </summary>
    public IActionResult Create()
    {
        PopulateViewBag();
        return View(new EventTemplate());
    }

    /// <summary>
    /// Creates a new event template.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EventTemplate template)
    {
        if (ModelState.IsValid)
        {
            try
            {
                template.CreatedBy = User.Identity?.Name ?? "system";
                await _eventTemplateService.CreateAsync(template);
                
                TempData["Success"] = $"Event template '{template.Name}' created successfully.";
                _logger.LogInformation("Created event template: {Name} ({Id})", template.Name, template.Id);
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event template: {Name}", template.Name);
                ModelState.AddModelError("", "An error occurred while creating the template.");
            }
        }

        PopulateViewBag();
        return View(template);
    }

    /// <summary>
    /// Displays the edit event template form.
    /// </summary>
    public async Task<IActionResult> Edit(Guid id)
    {
        var template = await _eventTemplateService.GetByIdAsync(id);
        if (template == null)
        {
            _logger.LogWarning("Event template not found: {Id}", id);
            return NotFound();
        }

        PopulateViewBag();
        return View(template);
    }

    /// <summary>
    /// Updates an existing event template.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EventTemplate template)
    {
        if (id != template.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                template.UpdatedBy = User.Identity?.Name ?? "system";
                await _eventTemplateService.UpdateAsync(template);
                
                TempData["Success"] = $"Event template '{template.Name}' updated successfully.";
                _logger.LogInformation("Updated event template: {Name} ({Id})", template.Name, template.Id);
                
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Event template not found for update: {Id}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event template: {Id}", id);
                ModelState.AddModelError("", "An error occurred while updating the template.");
            }
        }

        PopulateViewBag();
        return View(template);
    }

    /// <summary>
    /// Displays the details of an event template.
    /// </summary>
    public async Task<IActionResult> Details(Guid id)
    {
        var template = await _eventTemplateService.GetByIdAsync(id);
        if (template == null)
        {
            _logger.LogWarning("Event template not found: {Id}", id);
            return NotFound();
        }

        return View(template);
    }

    /// <summary>
    /// Deletes an event template.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _eventTemplateService.DeleteAsync(id);
        if (!result)
        {
            _logger.LogWarning("Event template not found for deletion: {Id}", id);
            TempData["Error"] = "Event template not found.";
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "Event template deleted successfully.";
        _logger.LogInformation("Deleted event template: {Id}", id);
        
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Displays the import templates page.
    /// </summary>
    public async Task<IActionResult> Import()
    {
        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            TempData["Error"] = "No active tournament found. Please activate a tournament first.";
            return RedirectToAction("Index", "Tournaments", new { area = "Admin" });
        }

        var templates = await _eventTemplateService.GetActiveAsync();
        
        ViewBag.ActiveTournament = activeTournament;
        PopulateFilterDropdowns();
        
        return View(templates.ToList());
    }

    /// <summary>
    /// Imports selected templates into the active tournament.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportSelected(List<Guid> selectedTemplates)
    {
        if (selectedTemplates == null || !selectedTemplates.Any())
        {
            TempData["Error"] = "Please select at least one template to import.";
            return RedirectToAction(nameof(Import));
        }

        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            TempData["Error"] = "No active tournament found. Please activate a tournament first.";
            return RedirectToAction("Index", "Tournaments", new { area = "Admin" });
        }

        try
        {
            var createdBy = User.Identity?.Name ?? "system";
            var events = await _eventTemplateService.ImportToTournamentAsync(
                selectedTemplates, 
                activeTournament.Id, 
                createdBy);

            var count = events.Count();
            TempData["Success"] = $"Successfully imported {count} event(s) into the tournament.";
            _logger.LogInformation("Imported {Count} events from templates to tournament {TournamentId}", 
                count, activeTournament.Id);

            // Send real-time update
            await _hubContext.Clients.All.SendAsync("ReceiveEventUpdate", 
                "Event Templates", 
                $"{count} events imported");

            return RedirectToAction("Index", "Events", new { area = "Admin" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing templates to tournament");
            TempData["Error"] = "An error occurred while importing templates.";
            return RedirectToAction(nameof(Import));
        }
    }

    /// <summary>
    /// Imports all active templates into the active tournament.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ImportAll()
    {
        var activeTournament = await _tournamentService.GetActiveTournamentAsync();
        if (activeTournament == null)
        {
            TempData["Error"] = "No active tournament found. Please activate a tournament first.";
            return RedirectToAction("Index", "Tournaments", new { area = "Admin" });
        }

        try
        {
            var createdBy = User.Identity?.Name ?? "system";
            var events = await _eventTemplateService.ImportAllToTournamentAsync(
                activeTournament.Id, 
                createdBy);

            var count = events.Count();
            TempData["Success"] = $"Successfully imported all {count} active event template(s) into the tournament.";
            _logger.LogInformation("Imported all {Count} active templates to tournament {TournamentId}", 
                count, activeTournament.Id);

            // Send real-time update
            await _hubContext.Clients.All.SendAsync("ReceiveEventUpdate", 
                "Event Templates", 
                $"All {count} templates imported");

            return RedirectToAction("Index", "Events", new { area = "Admin" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing all templates to tournament");
            TempData["Error"] = "An error occurred while importing templates.";
            return RedirectToAction(nameof(Import));
        }
    }

    /// <summary>
    /// Toggles the active status of an event template.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(Guid id)
    {
        var template = await _eventTemplateService.GetByIdAsync(id);
        if (template == null)
        {
            return NotFound();
        }

        template.IsActive = !template.IsActive;
        template.UpdatedBy = User.Identity?.Name ?? "system";
        await _eventTemplateService.UpdateAsync(template);

        var status = template.IsActive ? "activated" : "deactivated";
        TempData["Success"] = $"Event template '{template.Name}' {status}.";
        _logger.LogInformation("Toggled event template {Id} active status to {IsActive}", id, template.IsActive);

        return RedirectToAction(nameof(Index));
    }

    private void PopulateViewBag()
    {
        // Gender groups
        ViewBag.GenderGroups = new SelectList(
            Enum.GetValues(typeof(DivisionType))
                .Cast<DivisionType>()
                .Select(d => new { Id = d.ToString(), Name = d.ToString() }),
            "Id", "Name");

        // Event classes
        ViewBag.ClassGroups = new SelectList(
            Enum.GetValues(typeof(EventClass))
                .Cast<EventClass>()
                .Select(c => new
                {
                    Id = c.ToString(),
                    Name = c == EventClass.Open ? "Open" : $"Class {(int)c}"
                }),
            "Id", "Name");

        // Event types
        ViewBag.EventTypes = new SelectList(
            Enum.GetValues(typeof(EventType))
                .Cast<EventType>()
                .Select(t => new { Id = t.ToString(), Name = t.ToString() }),
            "Id", "Name");

        // Categories
        ViewBag.Categories = new SelectList(new[]
        {
            new { Id = "Track", Name = "Track" },
            new { Id = "Field", Name = "Field" }
        }, "Id", "Name");

        // Point systems
        ViewBag.PointSystems = new List<SelectListItem>
        {
            new SelectListItem { Value = "9,7,6,5,4,3,2,1", Text = "9,7,6,5,4,3,2,1 (Standard)" },
            new SelectListItem { Value = "12,10,9,8,7,6,1", Text = "12,10,9,8,7,6,1 (Relay)" }
        };
    }

    private void PopulateFilterDropdowns()
    {
        // Class filter
        ViewBag.ClassFilters = new SelectList(
            new[] { new { Id = "", Name = "All Classes" } }
                .Concat(Enum.GetValues(typeof(EventClass))
                    .Cast<EventClass>()
                    .Select(c => new
                    {
                        Id = c.ToString(),
                        Name = c == EventClass.Open ? "Open" : $"Class {(int)c}"
                    })),
            "Id", "Name");

        // Gender filter
        ViewBag.GenderFilters = new SelectList(
            new[] { new { Id = "", Name = "All Genders" } }
                .Concat(Enum.GetValues(typeof(DivisionType))
                    .Cast<DivisionType>()
                    .Select(d => new { Id = d.ToString(), Name = d.ToString() })),
            "Id", "Name");

        // Category filter
        ViewBag.CategoryFilters = new SelectList(new[]
        {
            new { Id = "", Name = "All Categories" },
            new { Id = "Track", Name = "Track" },
            new { Id = "Field", Name = "Field" }
        }, "Id", "Name");
    }
}