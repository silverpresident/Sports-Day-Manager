using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;
using HouseLeaderModel = SportsDay.Lib.Models.HouseLeader;

namespace SportsDay.Web.Areas.Admin.Controllers;

[Area("Admin")]
//[Authorize(Roles = "Administrator")]
public class HousesController : Controller
{
    private readonly IHouseService _houseService;
    private readonly IHouseLeaderService _houseLeaderService;
    private readonly ILogger<HousesController> _logger;

    public HousesController(
        IHouseService houseService,
        IHouseLeaderService houseLeaderService,
        ILogger<HousesController> logger)
    {
        _houseService = houseService;
        _houseLeaderService = houseLeaderService;
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

    public async Task<IActionResult> Events(int id)
    {
        try
        {
            var house = await _houseService.GetByIdAsync(id);
            if (house == null)
            {
                return NotFound();
            }

            var events = await _houseService.GetHouseEventsAsync(id);
            ViewBag.House = house;
            return View(events);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving events for house {Id}", id);
            TempData["Error"] = "Error retrieving events for house.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    public async Task<IActionResult> AddLeader(int id)
    {
        try
        {
            var house = await _houseService.GetByIdAsync(id);
            if (house == null)
            {
                return NotFound();
            }

            ViewBag.House = house;
            return View();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading add leader page for house {Id}", id);
            TempData["Error"] = "Error loading page.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddLeader(int id, string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                TempData["Error"] = "User ID is required.";
                return RedirectToAction(nameof(AddLeader), new { id });
            }

            var house = await _houseService.GetByIdAsync(id);
            if (house == null)
            {
                return NotFound();
            }

            // Check if user is already a house leader
            var existingLeader = await _houseLeaderService.GetByUserIdAsync(userId);
            if (existingLeader != null)
            {
                TempData["Error"] = $"User {userId} is already a house leader for {existingLeader.House?.Name}.";
                return RedirectToAction(nameof(AddLeader), new { id });
            }

            var houseLeader = new HouseLeaderModel
            {
                HouseId = id,
                UserId = userId,
                CreatedBy = User.Identity?.Name ?? "system"
            };

            await _houseLeaderService.CreateAsync(houseLeader);
            _logger.LogInformation("House leader {UserId} added to house {HouseId} by {User}",
                userId, id, User.Identity?.Name);

            TempData["Success"] = $"House leader {userId} added successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding house leader to house {Id}", id);
            TempData["Error"] = "Error adding house leader.";
            return RedirectToAction(nameof(AddLeader), new { id });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveLeader(Guid id)
    {
        try
        {
            var houseLeader = await _houseLeaderService.GetByIdAsync(id);
            if (houseLeader == null)
            {
                TempData["Error"] = "House leader not found.";
                return RedirectToAction(nameof(Index));
            }

            var houseId = houseLeader.HouseId;
            await _houseLeaderService.DeleteAsync(id);
            
            _logger.LogInformation("House leader {Id} removed from house {HouseId} by {User}",
                id, houseId, User.Identity?.Name);

            TempData["Success"] = "House leader removed successfully.";
            return RedirectToAction(nameof(Details), new { id = houseId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing house leader {Id}", id);
            TempData["Error"] = "Error removing house leader.";
            return RedirectToAction(nameof(Index));
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
