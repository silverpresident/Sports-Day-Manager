using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportsDay.Lib.Data;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Lib.Services;

public class HouseService : IHouseService
{
    private readonly SportsDayDbContext _context;
    private readonly ILogger<HouseService> _logger;

    public HouseService(SportsDayDbContext context, ILogger<HouseService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<House?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Houses
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving house {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<House>> GetAllAsync()
    {
        try
        {
            return await _context.Houses
                .AsNoTracking()
                .OrderBy(h => h.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all houses");
            throw;
        }
    }

    public async Task<House?> GetByIdWithParticipantsAsync(int id)
    {
        try
        {
            return await _context.Houses
                .Include(h => h.Participants)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving house {Id} with participants", id);
            throw;
        }
    }

    public async Task<House?> GetByIdWithResultsAsync(int id)
    {
        try
        {
            return await _context.Houses
                .Include(h => h.Results)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving house {Id} with results", id);
            throw;
        }
    }

    public async Task<House?> GetByIdWithDetailsAsync(int id)
    {
        try
        {
            return await _context.Houses
                .Include(h => h.Participants)
                .Include(h => h.Results)
                    .ThenInclude(r => r.Event)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving house {Id} with details", id);
            throw;
        }
    }

    public async Task<House> CreateAsync(House house)
    {
        try
        {
            house.CreatedAt = DateTime.Now;
            house.CreatedBy = house.CreatedBy ?? "system";

            _context.Houses.Add(house);
            await _context.SaveChangesAsync();

            _logger.LogInformation("House {Name} created with ID {Id}", house.Name, house.Id);

            return house;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating house {Name}", house.Name);
            throw;
        }
    }

    public async Task UpdateAsync(House house)
    {
        try
        {
            house.UpdatedAt = DateTime.Now;
            house.UpdatedBy = house.UpdatedBy ?? "system";

            _context.Houses.Update(house);
            await _context.SaveChangesAsync();

            _logger.LogInformation("House {Id} updated", house.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating house {Id}", house.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var house = await GetByIdAsync(id);
            if (house == null)
            {
                throw new InvalidOperationException($"House {id} not found");
            }

            _context.Houses.Remove(house);
            await _context.SaveChangesAsync();

            _logger.LogInformation("House {Id} deleted", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting house {Id}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        try
        {
            return await _context.Houses.AnyAsync(h => h.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if house {Id} exists", id);
            throw;
        }
    }
}