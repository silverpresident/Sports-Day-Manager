using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Web.Areas.Admin.Controllers;

[Area("Admin")]
//[Authorize(Roles = "Administrator")]
public class HousesController : Controller
{
    private readonly IHouseService _houseService;
    private readonly ILogger<HousesController> _logger;

    public HousesController(IHouseService houseService, ILogger<HousesController> logger)
    {
        _houseService = houseService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var houses = await _houseService.GetAllAsync();
            return View(houses);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving houses for admin index");
            TempData["Error"] = "Error retrieving houses.";
            return View(new List<House>());
        }
    }

    public IActionResult Create()
    {
        return View(new House());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(House house)
    {
        if (ModelState.IsValid)
        {
            try
            {
                house.CreatedBy = User.Identity?.Name ?? "system";
                await _houseService.CreateAsync(house);
                TempData["Success"] = $"House {house.Name} created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating house {Name}", house.Name);
                ModelState.AddModelError("", "Error creating house.");
            }
        }
        return View(house);
    }

    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var house = await _houseService.GetByIdAsync(id);
            if (house == null)
            {
                return NotFound();
            }
            return View(house);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving house {Id} for edit", id);
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, House house)
    {
        if (id != house.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                house.UpdatedBy = User.Identity?.Name ?? "system";
                await _houseService.UpdateAsync(house);
                TempData["Success"] = $"House {house.Name} updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _houseService.ExistsAsync(id))
                {
                    return NotFound();
                }
                _logger.LogError("Concurrency error updating house {Id}", id);
                ModelState.AddModelError("", "The house was modified by another user. Please try again.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating house {Id}", id);
                ModelState.AddModelError("", "Error updating house.");
            }
        }
        return View(house);
    }

    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var house = await _houseService.GetByIdWithDetailsAsync(id);

            if (house == null)
            {
                return NotFound();
            }

            return View(house);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving house {Id} details", id);
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var house = await _houseService.GetByIdAsync(id);
            if (house == null)
            {
                return NotFound();
            }

            await _houseService.DeleteAsync(id);
            TempData["Success"] = $"House {house.Name} deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error deleting house {Id} - has related records", id);
            TempData["Error"] = "Cannot delete house because it has related records.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting house {Id}", id);
            TempData["Error"] = "Error deleting house.";
            return RedirectToAction(nameof(Index));
        }
    }
}
