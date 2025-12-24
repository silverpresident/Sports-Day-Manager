using Microsoft.EntityFrameworkCore;
using SportsDay.Lib.Data;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportsDay.Lib.Services
{
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
            _context.Tournaments.Add(tournament);
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
            var tournamentToActivate = await _context.Tournaments.FindAsync(id);
            if (tournamentToActivate != null)
            {
                var currentlyActive = await _context.Tournaments.FirstOrDefaultAsync(t => t.IsActive);
                if (currentlyActive != null)
                {
                    currentlyActive.IsActive = false;
                }
                tournamentToActivate.IsActive = !tournamentToActivate.IsActive;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Tournament> GetActiveTournamentAsync()
        {
            return await _context.Tournaments.FirstOrDefaultAsync(t => t.IsActive);
        }
    }
}
