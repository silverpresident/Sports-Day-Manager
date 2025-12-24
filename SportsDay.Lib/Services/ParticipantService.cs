using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportsDay.Lib.Data;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Lib.Services;

public class ParticipantService : IParticipantService
{
    private readonly SportsDayDbContext _context;
    private readonly ILogger<ParticipantService> _logger;

    public ParticipantService(SportsDayDbContext context, ILogger<ParticipantService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Participant?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _context.Participants
                .Include(p => p.House)
                .Include(p => p.Tournament)
                .Include(p => p.Results)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving participant {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Participant>> GetAllAsync()
    {
        try
        {
            return await _context.Participants
                .Include(p => p.House)
                .Include(p => p.Tournament)
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all participants");
            throw;
        }
    }

    public async Task<IEnumerable<Participant>> GetByTournamentIdAsync(Guid tournamentId)
    {
        try
        {
            return await _context.Participants
                .Include(p => p.House)
                .Include(p => p.Tournament)
                .Where(p => p.TournamentId == tournamentId)
                .OrderBy(p => p.House!.Name)
                .ThenBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving participants for tournament {TournamentId}", tournamentId);
            throw;
        }
    }

    public async Task<IEnumerable<Participant>> GetByHouseIdAsync(int houseId)
    {
        try
        {
            return await _context.Participants
                .Include(p => p.House)
                .Include(p => p.Tournament)
                .Where(p => p.HouseId == houseId)
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving participants for house {HouseId}", houseId);
            throw;
        }
    }

    public async Task<IEnumerable<Participant>> GetByTournamentAndHouseAsync(Guid tournamentId, int houseId)
    {
        try
        {
            return await _context.Participants
                .Include(p => p.House)
                .Include(p => p.Tournament)
                .Where(p => p.TournamentId == tournamentId && p.HouseId == houseId)
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving participants for tournament {TournamentId} and house {HouseId}", 
                tournamentId, houseId);
            throw;
        }
    }

    public async Task<Participant> CreateAsync(Participant participant)
    {
        try
        {
            participant.CreatedAt = DateTime.Now;
            
            // Calculate age in years
            var today = DateTime.Now;
            var age = today.Year - participant.DateOfBirth.Year;
            if (participant.DateOfBirth.Date > today.AddYears(-age)) age--;
            participant.AgeInYears = age;

            _context.Participants.Add(participant);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Participant {FullName} created for tournament {TournamentId} and house {HouseId}", 
                participant.FullName, participant.TournamentId, participant.HouseId);

            return participant;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating participant {FullName}", participant.FullName);
            throw;
        }
    }

    public async Task UpdateAsync(Participant participant)
    {
        try
        {
            participant.UpdatedAt = DateTime.Now;
            
            // Recalculate age in years
            var today = DateTime.Now;
            var age = today.Year - participant.DateOfBirth.Year;
            if (participant.DateOfBirth.Date > today.AddYears(-age)) age--;
            participant.AgeInYears = age;

            _context.Participants.Update(participant);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Participant {Id} updated", participant.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating participant {Id}", participant.Id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        try
        {
            var participant = await GetByIdAsync(id);
            if (participant == null)
            {
                throw new InvalidOperationException($"Participant {id} not found");
            }

            _context.Participants.Remove(participant);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Participant {Id} deleted", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting participant {Id}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        try
        {
            return await _context.Participants.AnyAsync(p => p.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if participant {Id} exists", id);
            throw;
        }
    }
}