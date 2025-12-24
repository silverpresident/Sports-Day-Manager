using SportsDay.Lib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsDay.Lib.Services.Interfaces
{
    public interface ITournamentService
    {
        Task<List<Tournament>> GetTournamentsAsync();
        Task<Tournament> GetTournamentByIdAsync(Guid id);
        Task CreateTournamentAsync(Tournament tournament);
        Task UpdateTournamentAsync(Tournament tournament);
        Task DeleteTournamentAsync(Guid id);
        Task SetActiveTournamentAsync(Guid id);
        Task<Tournament> GetActiveTournamentAsync();
    }
}
