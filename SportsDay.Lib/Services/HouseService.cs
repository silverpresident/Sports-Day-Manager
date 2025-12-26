using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SportsDay.Lib.Data;
using SportsDay.Lib.Enums;
using SportsDay.Lib.Models;
using SportsDay.Lib.Services.Interfaces;
using SportsDay.Lib.ViewModels;

namespace SportsDay.Lib.Services;

public class HouseService : IHouseService
{
    private readonly SportsDayDbContext _context;
    private readonly ITournamentService _tournamentService;
    private readonly ILogger<HouseService> _logger;

    public HouseService(
        SportsDayDbContext context,
        ITournamentService tournamentService,
        ILogger<HouseService> logger)
    {
        _context = context;
        _tournamentService = tournamentService;
        _logger = logger;
    }

    public async Task<House?> GetByIdAsync(int id)
    {
        try
        {
            return await _context.Houses
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving house {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<House>> GetAllAsync()
    {
        try
        {
            return await _context.Houses
                .AsNoTracking()
                .OrderBy(h => h.Name)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all houses");
            throw;
        }
    }

    public async Task<House?> GetByIdWithParticipantsAsync(int id)
    {
        try
        {
            return await _context.Houses
                .Include(h => h.Participants)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving house {Id} with participants", id);
            throw;
        }
    }

    public async Task<House?> GetByIdWithResultsAsync(int id)
    {
        try
        {
            return await _context.Houses
                .Include(h => h.Results)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving house {Id} with results", id);
            throw;
        }
    }

    public async Task<House?> GetByIdWithDetailsAsync(int id)
    {
        try
        {
            return await _context.Houses
                .Include(h => h.Participants)
                .Include(h => h.Results)
                    .ThenInclude(r => r.Event)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving house {Id} with details", id);
            throw;
        }
    }

    public async Task<House> CreateAsync(House house)
    {
        try
        {
            house.CreatedAt = DateTime.Now;
            house.CreatedBy = house.CreatedBy ?? "system";

            _context.Houses.Add(house);
            await _context.SaveChangesAsync();

            _logger.LogInformation("House {Name} created with ID {Id}", house.Name, house.Id);

            return house;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating house {Name}", house.Name);
            throw;
        }
    }

    public async Task UpdateAsync(House house)
    {
        try
        {
            house.UpdatedAt = DateTime.Now;
            house.UpdatedBy = house.UpdatedBy ?? "system";

            _context.Houses.Update(house);
            await _context.SaveChangesAsync();

            _logger.LogInformation("House {Id} updated", house.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating house {Id}", house.Id);
            throw;
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var house = await GetByIdAsync(id);
            if (house == null)
            {
                throw new InvalidOperationException($"House {id} not found");
            }

            _context.Houses.Remove(house);
            await _context.SaveChangesAsync();

            _logger.LogInformation("House {Id} deleted", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting house {Id}", id);
            throw;
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        try
        {
            return await _context.Houses.AnyAsync(h => h.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if house {Id} exists", id);
            throw;
        }
    }

    public async Task<House?> GetByIdWithLeadersAsync(int id)
    {
        try
        {
            return await _context.Houses
                .Include(h => h.HouseLeaders)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving house {Id} with leaders", id);
            throw;
        }
    }

    public async Task<HouseDetailsViewModel?> GetHouseDetailsForActiveTournamentAsync(int id)
    {
        try
        {
            var activeTournament = await _tournamentService.GetActiveTournamentAsync();
            
            var house = await _context.Houses
                .Include(h => h.HouseLeaders)
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id);

            if (house == null)
            {
                _logger.LogWarning("House {Id} not found", id);
                return null;
            }

            // Get results for this house in the active tournament
            var houseResults = activeTournament != null
                ? await _context.Results
                    .Include(r => r.Event)
                    .Include(r => r.Participant)
                    .Where(r => r.HouseId == id && r.TournamentId == activeTournament.Id)
                    .AsNoTracking()
                    .ToListAsync()
                : new List<Result>();

            // Calculate total points
            var totalPoints = houseResults.Sum(r => r.Points);

            // Get all house rankings for the active tournament
            var allHouseRankings = await GetAllHouseRankingsForActiveTournamentAsync();
            var overallRank = allHouseRankings.FirstOrDefault(r => r.HouseId == id)?.Rank ?? 0;

            // Build event results
            var eventResults = houseResults
                .GroupBy(r => r.EventId)
                .Select(g => {
                    var firstResult = g.First();
                    var bestResult = g.OrderBy(r => r.Placement).First();
                    return new HouseEventResultViewModel
                    {
                        EventId = firstResult.EventId,
                        EventName = firstResult.Event?.Name ?? "Unknown",
                        EventNumber = firstResult.Event?.EventNumber ?? 0,
                        Division = firstResult.Event?.GenderGroup ?? DivisionType.Open,
                        ClassGroup = firstResult.Event?.ClassGroup ?? EventClass.Open,
                        Placement = bestResult.Placement,
                        Points = g.Sum(r => r.Points),
                        ParticipantName = bestResult.Participant?.FullName,
                        IsNewRecord = g.Any(r => r.IsNewRecord)
                    };
                })
                .OrderBy(e => e.EventNumber)
                .ToList();

            // Calculate division summaries
            var divisionSummaries = new List<DivisionSummaryViewModel>();
            foreach (DivisionType division in Enum.GetValues(typeof(DivisionType)))
            {
                var divisionPoints = houseResults
                    .Where(r => r.Event?.GenderGroup == division)
                    .Sum(r => r.Points);

                if (divisionPoints > 0 || activeTournament != null)
                {
                    // Get rankings for this division
                    var divisionRankings = await GetDivisionRankingsAsync(activeTournament?.Id, division);
                    var divisionRank = divisionRankings.FirstOrDefault(r => r.HouseId == id)?.Rank ?? 0;

                    divisionSummaries.Add(new DivisionSummaryViewModel
                    {
                        Division = division,
                        TotalPoints = divisionPoints,
                        Rank = divisionRank,
                        TotalHouses = divisionRankings.Count()
                    });
                }
            }

            return new HouseDetailsViewModel
            {
                House = house,
                ActiveTournament = activeTournament,
                HouseLeaders = house.HouseLeaders,
                TotalPoints = totalPoints,
                OverallRank = overallRank,
                TotalHouses = allHouseRankings.Count(),
                EventResults = eventResults,
                DivisionSummaries = divisionSummaries,
                AllHouseRankings = allHouseRankings
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving house details for house {Id}", id);
            throw;
        }
    }

    public async Task<HouseResultsViewModel?> GetHouseMembersForActiveTournamentAsync(int id)
    {
        try
        {
            var activeTournament = await _tournamentService.GetActiveTournamentAsync();
            
            var house = await _context.Houses
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.Id == id);

            if (house == null)
            {
                _logger.LogWarning("House {Id} not found", id);
                return null;
            }

            // Get participants for this house in the active tournament
            var participants = activeTournament != null
                ? await _context.Participants
                    .Include(p => p.Results)
                        .ThenInclude(r => r.Event)
                    .Where(p => p.HouseId == id && p.TournamentId == activeTournament.Id)
                    .AsNoTracking()
                    .ToListAsync()
                : new List<Participant>();

            var participantViewModels = participants
                .Select(p => new ParticipantDetailsViewModel
                {
                    ParticipantId = p.Id,
                    FullName = p.FullName,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Division = p.GenderGroup,
                    ClassGroup = p.EventClassGroup,
                    AgeGroup = p.AgeGroup,
                    TotalPoints = p.Results.Sum(r => r.Points),
                    EventCount = p.Results.Count,
                    EventResults = p.Results
                        .Select(r => new ParticipantEventResultViewModel
                        {
                            EventId = r.EventId,
                            EventName = r.Event?.Name ?? "Unknown",
                            EventNumber = r.Event?.EventNumber ?? 0,
                            Placement = r.Placement,
                            Points = r.Points,
                            IsNewRecord = r.IsNewRecord,
                            SpeedOrDistance = r.SpeedOrDistance
                        })
                        .OrderBy(e => e.EventNumber)
                        .ToList()
                })
                .OrderByDescending(p => p.TotalPoints)
                .ThenBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .ToList();

            return new HouseResultsViewModel
            {
                House = house,
                ActiveTournament = activeTournament,
                Participants = participantViewModels,
                TotalParticipants = participantViewModels.Count,
                TotalPoints = participantViewModels.Sum(p => p.TotalPoints)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving house members for house {Id}", id);
            throw;
        }
    }

    public async Task<IEnumerable<HouseRankingViewModel>> GetAllHouseRankingsForActiveTournamentAsync()
    {
        try
        {
            var activeTournament = await _tournamentService.GetActiveTournamentAsync();
            
            var houses = await _context.Houses
                .AsNoTracking()
                .ToListAsync();

            if (activeTournament == null)
            {
                return houses.Select((h, index) => new HouseRankingViewModel
                {
                    HouseId = h.Id,
                    HouseName = h.Name,
                    HouseColor = h.Color,
                    TotalPoints = 0,
                    Rank = index + 1
                }).ToList();
            }

            var results = await _context.Results
                .Where(r => r.TournamentId == activeTournament.Id)
                .AsNoTracking()
                .ToListAsync();

            var housePoints = houses.Select(h => new
            {
                House = h,
                TotalPoints = results.Where(r => r.HouseId == h.Id).Sum(r => r.Points)
            })
            .OrderByDescending(hp => hp.TotalPoints)
            .ToList();

            var rankings = new List<HouseRankingViewModel>();
            var currentRank = 1;
            var previousPoints = -1;

            for (int i = 0; i < housePoints.Count; i++)
            {
                var hp = housePoints[i];
                if (hp.TotalPoints != previousPoints)
                {
                    currentRank = i + 1;
                }
                
                rankings.Add(new HouseRankingViewModel
                {
                    HouseId = hp.House.Id,
                    HouseName = hp.House.Name,
                    HouseColor = hp.House.Color,
                    TotalPoints = hp.TotalPoints,
                    Rank = currentRank
                });

                previousPoints = hp.TotalPoints;
            }

            return rankings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all house rankings");
            throw;
        }
    }

    private async Task<IEnumerable<HouseRankingViewModel>> GetDivisionRankingsAsync(Guid? tournamentId, DivisionType division)
    {
        try
        {
            var houses = await _context.Houses
                .AsNoTracking()
                .ToListAsync();

            if (tournamentId == null)
            {
                return houses.Select((h, index) => new HouseRankingViewModel
                {
                    HouseId = h.Id,
                    HouseName = h.Name,
                    HouseColor = h.Color,
                    TotalPoints = 0,
                    Rank = index + 1
                }).ToList();
            }

            var results = await _context.Results
                .Include(r => r.Event)
                .Where(r => r.TournamentId == tournamentId && r.Event.GenderGroup == division)
                .AsNoTracking()
                .ToListAsync();

            var housePoints = houses.Select(h => new
            {
                House = h,
                TotalPoints = results.Where(r => r.HouseId == h.Id).Sum(r => r.Points)
            })
            .OrderByDescending(hp => hp.TotalPoints)
            .ToList();

            var rankings = new List<HouseRankingViewModel>();
            var currentRank = 1;
            var previousPoints = -1;

            for (int i = 0; i < housePoints.Count; i++)
            {
                var hp = housePoints[i];
                if (hp.TotalPoints != previousPoints)
                {
                    currentRank = i + 1;
                }
                
                rankings.Add(new HouseRankingViewModel
                {
                    HouseId = hp.House.Id,
                    HouseName = hp.House.Name,
                    HouseColor = hp.House.Color,
                    TotalPoints = hp.TotalPoints,
                    Rank = currentRank
                });

                previousPoints = hp.TotalPoints;
            }

            return rankings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving division rankings for {Division}", division);
            throw;
        }
    }
}