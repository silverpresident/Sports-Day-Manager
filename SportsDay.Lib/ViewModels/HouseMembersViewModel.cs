using SportsDay.Lib.Enums;
using SportsDay.Lib.Models;

namespace SportsDay.Lib.ViewModels;

public class HouseMembersViewModel
{
    public House House { get; set; } = null!;
    public Tournament? ActiveTournament { get; set; }
    public IEnumerable<ParticipantDetailsViewModel> Participants { get; set; } = new List<ParticipantDetailsViewModel>();
    public int TotalParticipants { get; set; }
    public int TotalPoints { get; set; }
}

public class ParticipantDetailsViewModel
{
    public Guid ParticipantId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DivisionType Division { get; set; }
    public EventClass ClassGroup { get; set; }
    public string AgeGroup { get; set; } = string.Empty;
    public int TotalPoints { get; set; }
    public int EventCount { get; set; }
    public IEnumerable<ParticipantEventResultViewModel> EventResults { get; set; } = new List<ParticipantEventResultViewModel>();
}

public class ParticipantEventResultViewModel
{
    public Guid EventId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public int EventNumber { get; set; }
    public int? Placement { get; set; }
    public int Points { get; set; }
    public bool IsNewRecord { get; set; }
    public decimal? SpeedOrDistance { get; set; }
}