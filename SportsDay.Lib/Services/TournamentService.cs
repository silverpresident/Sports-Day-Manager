using Microsoft.EntityFrameworkCore;
using SportsDay.Lib.Data;
using SportsDay.Lib.Models;

namespace SportsDay.Lib.Services;

public interface ITournamentService
{
    Task<Tournament?> GetActiveTournamentAsync();
    Task<Tournament?> GetTournamentByIdAsync(Guid id);
    Task<bool> SetActiveTournamentAsync(Guid tournamentId);
    Task<bool> DeactivateTournamentAsync(Guid tournamentId);
    Task<IEnumerable<Tournament>> GetAllTournamentsAsync();
}

public class TournamentService : ITournamentService
{
    private readonly SportsDayDbContext _context;

    public TournamentService(SportsDayDbContext context)
    {
        _context = context;
    }

    public async Task<Tournament?> GetActiveTournamentAsync()
    {
        return await _context.Tournaments
            .Include(t => t.Divisions)
            .FirstOrDefaultAsync(t => t.IsActive);
    }

    public async Task<Tournament?> GetTournamentByIdAsync(Guid id)
    {
        return await _context.Tournaments
            .Include(t => t.Divisions)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<bool> SetActiveTournamentAsync(Guid tournamentId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Deactivate current active tournament if any
            var currentActive = await _context.Tournaments
                .FirstOrDefaultAsync(t => t.IsActive);
            
            if (currentActive != null)
            {
                currentActive.IsActive = false;
                _context.Tournaments.Update(currentActive);
            }

            // Activate the new tournament
            var tournament = await _context.Tournaments
                .FirstOrDefaultAsync(t => t.Id == tournamentId);

            if (tournament == null)
            {
                return false;
            }

            tournament.IsActive = true;
            _context.Tournaments.Update(tournament);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> DeactivateTournamentAsync(Guid tournamentId)
    {
        var tournament = await _context.Tournaments
            .FirstOrDefaultAsync(t => t.Id == tournamentId && t.IsActive);

        if (tournament == null)
        {
            return false;
        }

        tournament.IsActive = false;
        _context.Tournaments.Update(tournament);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Tournament>> GetAllTournamentsAsync()
    {
        return await _context.Tournaments
            .Include(t => t.Divisions)
            .OrderByDescending(t => t.TournamentDate)
            .ToListAsync();
    }
}
