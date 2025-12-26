using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SportsDay.Lib.Enums;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Web.Controllers;

/// <summary>
/// Controller for displaying event results.
/// Results represent participant placements and scores in events.
/// </summary>
public class ResultController : Controller
{
    private readonly IResultService _resultService;
    private readonly ILogger<ResultController> _logger;

    public ResultController(IResultService resultService, ILogger<ResultController> logger)
    {
        _resultService = resultService;
        _logger = logger;
    }

    /// <summary>
    /// Displays all results with optional filtering.
    /// </summary>
    /// <param name="division">Optional division filter (Boys, Girls, Open).</param>
    /// <param name="eventClass">Optional event class filter (Open, Class1-4).</param>
    /// <param name="category">Optional category filter (Track, Field).</param>
    /// <param name="houseId">Optional house filter.</param>
    /// <param name="eventId">Optional event filter.</param>
    /// <returns>The results index view.</returns>
    public async Task<IActionResult> Index(
        DivisionType? division,
        EventClass? eventClass,
        string? category,
        int? houseId,
        Guid? eventId)
    {
        try
        {
            _logger.LogInformation(
                "Loading results with filters - Division: {Division}, Class: {Class}, Category: {Category}, House: {HouseId}, Event: {EventId}",
                division, eventClass, category, houseId, eventId);

            var viewModel = await _resultService.GetResultsIndexAsync(
                division,
                eventClass,
                category,
                houseId,
                eventId);

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading results");
            return View("Error");
        }
    }

    /// <summary>
    /// Displays details for a specific result by ID.
    /// </summary>
    /// <param name="id">The result ID.</param>
    /// <returns>The result details view.</returns>
    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            _logger.LogInformation("Loading result details for {Id}", id);

            var result = await _resultService.GetResultByIdAsync(id);
            if (result == null)
            {
                _logger.LogWarning("Result not found: {Id}", id);
                return NotFound();
            }

            return View(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading result details for {Id}", id);
            return View("Error");
        }
    }

    /// <summary>
    /// Displays results for a specific event.
    /// </summary>
    /// <param name="id">The event ID.</param>
    /// <returns>The event results view.</returns>
    public async Task<IActionResult> ByEvent(Guid id)
    {
        try
        {
            _logger.LogInformation("Loading results for event {Id}", id);

            var results = await _resultService.GetResultsByEventAsync(id);
            ViewBag.EventId = id;
            
            return View(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading results for event {Id}", id);
            return View("Error");
        }
    }

    /// <summary>
    /// Displays results for a specific house.
    /// </summary>
    /// <param name="id">The house ID.</param>
    /// <returns>The house results view.</returns>
    public async Task<IActionResult> ByHouse(int id)
    {
        try
        {
            _logger.LogInformation("Loading results for house {Id}", id);

            var results = await _resultService.GetResultsByHouseAsync(id);
            ViewBag.HouseId = id;
            
            return View(results);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading results for house {Id}", id);
            return View("Error");
        }
    }

    /// <summary>
    /// Displays result statistics for the active tournament.
    /// </summary>
    /// <returns>The statistics view.</returns>
    public async Task<IActionResult> Statistics()
    {
        try
        {
            _logger.LogInformation("Loading result statistics");

            var statistics = await _resultService.GetResultStatisticsAsync();
            return View(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading result statistics");
            return View("Error");
        }
    }
}