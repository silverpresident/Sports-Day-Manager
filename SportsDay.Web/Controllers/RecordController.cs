using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SportsDay.Lib.Enums;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Web.Controllers;

/// <summary>
/// Controller for displaying event records.
/// Records come from two sources:
/// 1. Existing records stored in Events (historical records)
/// 2. New records from Results where IsNewRecord = true
/// </summary>
public class RecordController : Controller
{
    private readonly IRecordService _recordService;
    private readonly ILogger<RecordController> _logger;

    public RecordController(IRecordService recordService, ILogger<RecordController> logger)
    {
        _recordService = recordService;
        _logger = logger;
    }

    /// <summary>
    /// Displays all records with optional filtering.
    /// Shows both existing records from events and new records from results.
    /// </summary>
    /// <param name="division">Optional division filter (Boys, Girls, Open).</param>
    /// <param name="eventClass">Optional event class filter (Open, Class1-4).</param>
    /// <param name="category">Optional category filter (Track, Field).</param>
    /// <param name="year">Optional year filter.</param>
    /// <param name="showNewOnly">Show only new records from results.</param>
    /// <param name="showExistingOnly">Show only existing records from events.</param>
    /// <returns>The records index view.</returns>
    public async Task<IActionResult> Index(
        DivisionType? division,
        EventClass? eventClass,
        string? category,
        int? year,
        bool showNewOnly = false,
        bool showExistingOnly = false)
    {
        try
        {
            _logger.LogInformation(
                "Loading records with filters - Division: {Division}, Class: {Class}, Category: {Category}, Year: {Year}, NewOnly: {NewOnly}, ExistingOnly: {ExistingOnly}",
                division, eventClass, category, year, showNewOnly, showExistingOnly);

            var viewModel = await _recordService.GetRecordsIndexAsync(
                division,
                eventClass,
                category,
                year,
                showNewOnly,
                showExistingOnly);

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading records");
            return View("Error");
        }
    }

    /// <summary>
    /// Displays details for a specific record by event ID.
    /// </summary>
    /// <param name="id">The event ID.</param>
    /// <returns>The record details view.</returns>
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            _logger.LogInformation("Loading record details for event {Id}", id);

            var record = await _recordService.GetRecordByEventIdAsync(id);
            if (record == null)
            {
                _logger.LogWarning("Record not found for event: {Id}", id);
                return NotFound();
            }

            return View(record);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading record details for event {Id}", id);
            return View("Error");
        }
    }
}
