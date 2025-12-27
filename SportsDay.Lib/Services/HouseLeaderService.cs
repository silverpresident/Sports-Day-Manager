using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportsDay.Lib.Data;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Lib.Services;

public class HouseLeaderService : IHouseLeaderService
{
    private readonly SportsDayDbContext _context;
    private readonly ILogger<HouseLeaderService> _logger;

    public HouseLeaderService(SportsDayDbContext context, ILogger<HouseLeaderService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<HouseLeader?> GetByUserIdAsync(string userId)
    {
        try
        {
            return await _context.HouseLeaders
                .Include(hl => hl.House)
                .FirstOrDefaultAsync(hl => hl.UserId == userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving house leader for user {UserId}", userId);
            throw;
        }
    }

    public async Task<HouseLeader?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _context.HouseLeaders
                .Include(hl => hl.House)
                .FirstOrDefaultAsync(hl => hl.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving house leader {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<HouseLeader>> GetAllAsync()
    {
        try
        {
            return await _context.HouseLeaders
                .Include(hl => hl.House)
                .OrderBy(hl => hl.House!.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all house leaders");
            throw;
        }
    }

    public async Task<IEnumerable<HouseLeader>> GetByHouseIdAsync(int houseId)
    {
        try
        {
            return await _context.HouseLeaders
                .Include(hl => hl.House)
                .Where(hl => hl.HouseId == houseId)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving house leaders for house {HouseId}", houseId);
            throw;
        }
    }

    public async Task<bool> IsUserHouseLeaderAsync(string userId)
    {
        try
        {
            return await _context.HouseLeaders.AnyAsync(hl => hl.UserId == userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} is a house leader", userId);
            throw;
        }
    }

    public async Task<bool> IsUserHouseLeaderForHouseAsync(string userId, int houseId)
    {
        try
        {
            return await _context.HouseLeaders
                .AnyAsync(hl => hl.UserId == userId && hl.HouseId == houseId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if user {UserId} is leader for house {HouseId}", userId, houseId);
            throw;
        }
    }

    public async Task<HouseLeader> CreateAsync(HouseLeader houseLeader)
    {
        try
        {
            // Check if user is already a house leader
            var existing = await GetByUserIdAsync(houseLeader.UserId);
            if (existing != null)
            {
                throw new InvalidOperationException($"User {houseLeader.UserId} is already a house leader for {existing.House?.Name}");
            }

            houseLeader.CreatedAt = DateTime.Now;
            houseLeader.CreatedBy = houseLeader.UserId;

            _context.HouseLeaders.Add(houseLeader);
            await _context.SaveChangesAsync();

            _logger.LogInformation("House leader created for user {UserId} and house {HouseId}", 
                houseLeader.UserId, houseLeader.HouseId);

            return houseLeader;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating house leader for user {UserId}", houseLeader.UserId);
            throw;
        }
    }

    public async Task UpdateAsync(HouseLeader houseLeader)
    {
        try
        {
            houseLeader.UpdatedAt = DateTime.Now;
            houseLeader.UpdatedBy = houseLeader.UserId;

            _context.HouseLeaders.Update(houseLeader);
            await _context.SaveChangesAsync();

            _logger.LogInformation("House leader {Id} updated", houseLeader.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating house leader {Id}", houseLeader.Id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        try
        {
            var houseLeader = await GetByIdAsync(id);
            if (houseLeader == null)
            {
                throw new InvalidOperationException($"House leader {id} not found");
            }

            _context.HouseLeaders.Remove(houseLeader);
            await _context.SaveChangesAsync();

            _logger.LogInformation("House leader {Id} deleted", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting house leader {Id}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        try
        {
            return await _context.HouseLeaders.AnyAsync(hl => hl.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if house leader {Id} exists", id);
            throw;
        }
    }

    public Task<IEnumerable<Guid>> GetHousesByUserIdAsync(string? userId)
    {
        ?
        throw new NotImplementedException();
    }
}