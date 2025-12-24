using SportsDay.Lib.Models;

namespace SportsDay.Lib.Services.Interfaces;

public interface IHouseService
{
    Task<House?> GetByIdAsync(int id);
    Task<IEnumerable<House>> GetAllAsync();
    Task<House?> GetByIdWithParticipantsAsync(int id);
    Task<House?> GetByIdWithResultsAsync(int id);
    Task<House?> GetByIdWithDetailsAsync(int id);
    Task<House> CreateAsync(House house);
    Task UpdateAsync(House house);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}