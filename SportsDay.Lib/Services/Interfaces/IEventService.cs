using SportsDay.Lib.Models;

namespace SportsDay.Lib.Services.Interfaces;

public interface IEventService
{
    Task<Event?> GetByIdAsync(Guid id);
    Task<IEnumerable<Event>> GetAllAsync();
    Task<IEnumerable<Event>> GetByTournamentIdAsync(Guid tournamentId);
    Task<IEnumerable<Event>> GetByActiveTournamentAsync();
    Task<Event?> GetByIdWithResultsAsync(Guid id);
    Task<Event?> GetByIdWithDetailsAsync(Guid id);
    Task<Event> CreateAsync(Event evt);
    Task UpdateAsync(Event evt);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<int> GetNextEventNumberAsync(Guid tournamentId);
    Task<Event?> GetNextEventAsync(Guid currentEventId);
    Task<Event?> GetPreviousEventAsync(Guid currentEventId);
}