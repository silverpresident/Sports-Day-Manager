using SportsDay.Lib.Models;
using SportsDay.Lib.ViewModels;

namespace SportsDay.Lib.Services.Interfaces;

public interface IHouseService
{
    Task<House?> GetByIdAsync(int id);
    Task<IEnumerable<House>> GetAllAsync();
    Task<House?> GetByIdWithParticipantsAsync(int id);
    Task<House?> GetByIdWithResultsAsync(int id);
    Task<House?> GetByIdWithDetailsAsync(int id);
    Task<House?> GetByIdWithLeadersAsync(int id);
    Task<HouseDetailsViewModel?> GetHouseDetailsForActiveTournamentAsync(int id);
    Task<HouseResultsViewModel?> GetHouseMembersForActiveTournamentAsync(int id);
    Task<IEnumerable<HouseRankingViewModel>> GetAllHouseRankingsForActiveTournamentAsync();
    Task<IEnumerable<Event>> GetHouseEventsAsync(int houseId);
    Task<House> CreateAsync(House house);
    Task UpdateAsync(House house);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}