using SportsDay.Lib.Enums;
using SportsDay.Lib.Models;

namespace SportsDay.Lib.ViewModels;

public class HouseDetailsViewModel
{
    public House House { get; set; } = null!;
    public Tournament? ActiveTournament { get; set; }
    public IEnumerable<HouseLeader> HouseLeaders { get; set; } = new List<HouseLeader>();
    public int TotalPoints { get; set; }
    public int OverallRank { get; set; }
    public int TotalHouses { get; set; }
    public IEnumerable<HouseEventResultViewModel> EventResults { get; set; } = new List<HouseEventResultViewModel>();
    public IEnumerable<DivisionSummaryViewModel> DivisionSummaries { get; set; } = new List<DivisionSummaryViewModel>();
    public IEnumerable<HouseRankingViewModel> AllHouseRankings { get; set; } = new List<HouseRankingViewModel>();
}

public class HouseEventResultViewModel
{
    public Guid EventId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public int EventNumber { get; set; }
    public DivisionType Division { get; set; }
    public EventClass ClassGroup { get; set; }
    public int? Placement { get; set; }
    public int Points { get; set; }
    public string? ParticipantName { get; set; }
    public bool IsNewRecord { get; set; }
}

public class DivisionSummaryViewModel
{
    public DivisionType Division { get; set; }
    public int TotalPoints { get; set; }
    public int Rank { get; set; }
    public int TotalHouses { get; set; }
}

public class HouseRankingViewModel
{
    public int HouseId { get; set; }
    public string HouseName { get; set; } = string.Empty;
    public string HouseColor { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public int Rank { get; set; }
}