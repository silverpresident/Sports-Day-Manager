using SportsDay.Lib.Models;

namespace SportsDay.Lib.Services.Interfaces;

public interface IParticipantService
{
    Task<Participant?> GetByIdAsync(Guid id);
    Task<IEnumerable<Participant>> GetAllAsync();
    Task<IEnumerable<Participant>> GetByTournamentIdAsync(Guid tournamentId);
    Task<IEnumerable<Participant>> GetByHouseIdAsync(int houseId);
    Task<IEnumerable<Participant>> GetByTournamentAndHouseAsync(Guid tournamentId, int houseId);
    Task<Participant> CreateAsync(Participant participant);
    Task UpdateAsync(Participant participant);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}