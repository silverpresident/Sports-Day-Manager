using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsDay.Lib.Data;
using SportsDay.Lib.Models;

namespace SportsDay.Web.Areas.Admin.Controllers;

[Area("Admin")]
//[Authorize(Roles = "Administrator")]
public class HousesController : Controller
{
    private readonly SportsDayDbContext _context;

    public HousesController(SportsDayDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var houses = await _context.Houses
            .Include(h => h.Participants)
            .Include(h => h.Results)
            .OrderBy(h => h.Id)
            .ToListAsync();
        return View(houses);
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
            _context.Houses.Add(house);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(house);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var house = await _context.Houses.FindAsync(id);
        if (house == null)
        {
            return NotFound();
        }
        return View(house);
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
                _context.Update(house);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Houses.AnyAsync(h => h.Id == id))
                {
                    return NotFound();
                }
                throw;
            }
        }
        return View(house);
    }

    public async Task<IActionResult> Details(int id)
    {
        var house = await _context.Houses
            .Include(h => h.Participants)
            .Include(h => h.Results)
                .ThenInclude(r => r.Event)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (house == null)
        {
            return NotFound();
        }

        return View(house);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var house = await _context.Houses.FindAsync(id);
        if (house == null)
        {
            return NotFound();
        }

        try
        {
            _context.Houses.Remove(house);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "Cannot delete house because it has related records.";
            return RedirectToAction(nameof(Index));
        }
    }
}
