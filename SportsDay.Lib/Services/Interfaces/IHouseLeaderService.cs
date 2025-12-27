using SportsDay.Lib.Models;

namespace SportsDay.Lib.Services.Interfaces;

public interface IHouseLeaderService
{
    Task<HouseLeader?> GetByUserIdAsync(string userId);
    Task<HouseLeader?> GetByIdAsync(Guid id);
    Task<IEnumerable<HouseLeader>> GetAllAsync();
    Task<IEnumerable<HouseLeader>> GetByHouseIdAsync(int houseId);
    Task<bool> IsUserHouseLeaderAsync(string userId);
    Task<bool> IsUserHouseLeaderForHouseAsync(string userId, int houseId);
    Task<HouseLeader> CreateAsync(HouseLeader houseLeader);
    Task UpdateAsync(HouseLeader houseLeader);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<IEnumerable<Guid>> GetHousesByUserIdAsync(string? userId);
}