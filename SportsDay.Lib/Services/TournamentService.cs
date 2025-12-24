using Microsoft.EntityFrameworkCore;
using SportsDay.Lib.Data;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;

namespace SportsDay.Lib.Services;

public class TournamentService : ITournamentService
{
    private readonly SportsDayDbContext _context;

    public TournamentService(SportsDayDbContext context)
    {
        _context = context;
    }

    public async Task<List<Tournament>> GetTournamentsAsync()
    {
        return await _context.Tournaments.ToListAsync();
    }

    public async Task<Tournament> GetTournamentByIdAsync(Guid id)
    {
        return await _context.Tournaments.FindAsync(id);
    }

    public async Task CreateTournamentAsync(Tournament tournament)
    {
        await _context.Tournaments.AddAsync(tournament);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateTournamentAsync(Tournament tournament)
    {
        _context.Entry(tournament).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTournamentAsync(Guid id)
    {
        var tournament = await _context.Tournaments.FindAsync(id);
        if (tournament != null)
        {
            _context.Tournaments.Remove(tournament);
            await _context.SaveChangesAsync();
        }
    }

    public async Task SetActiveTournamentAsync(Guid id)
    {
        var currentActive = await _context.Tournaments.FirstOrDefaultAsync(t => t.IsActive);
        if (currentActive != null)
        {
            currentActive.IsActive = false;
        }

        var newActive = await _context.Tournaments.FindAsync(id);
        if (newActive != null)
        {
            newActive.IsActive = true;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<Tournament> GetActiveTournamentAsync()
    {
        return await _context.Tournaments.FirstOrDefaultAsync(t => t.IsActive);
    }
}
