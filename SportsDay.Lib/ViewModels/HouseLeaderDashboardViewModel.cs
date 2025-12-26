using SportsDay.Lib.Models;

namespace SportsDay.Lib.ViewModels;

/// <summary>
/// ViewModel for the House Leader Dashboard
/// </summary>
public class HouseLeaderDashboardViewModel
{
    public HouseLeader HouseLeader { get; set; } = null!;
    public House House { get; set; } = null!;
    public Tournament? ActiveTournament { get; set; }
    public List<Participant> Participants { get; set; } = new();
    public int TotalParticipants => Participants.Count;
    public int TotalPoints => Participants.Sum(p => p.Points);
    public int EventsWithParticipants { get; set; }
    public int TotalEvents { get; set; }
}

/// <summary>
/// ViewModel for displaying events with participant registration counts
/// </summary>
public class HouseLeaderEventViewModel
{
    public Event Event { get; set; } = null!;
    public int HouseParticipantCount { get; set; }
    public int TotalParticipantCount { get; set; }
    public List<Participant> HouseParticipants { get; set; } = new();
    public bool CanRegisterMore => Event.ParticipantLimit == 0 || HouseParticipantCount < Event.ParticipantLimit;
}

/// <summary>
/// ViewModel for the Events list page in House Leader area
/// </summary>
public class HouseLeaderEventsViewModel
{
    public HouseLeader HouseLeader { get; set; } = null!;
    public House House { get; set; } = null!;
    public Tournament ActiveTournament { get; set; } = null!;
    public List<HouseLeaderEventViewModel> Events { get; set; } = new();
    public int TotalEvents => Events.Count;
    public int EventsWithHouseParticipants => Events.Count(e => e.HouseParticipantCount > 0);
}

/// <summary>
/// ViewModel for registering a participant to an event
/// </summary>
public class RegisterParticipantToEventViewModel
{
    public Event Event { get; set; } = null!;
    public House House { get; set; } = null!;
    public List<Participant> AvailableParticipants { get; set; } = new();
    public List<Participant> RegisteredParticipants { get; set; } = new();
    public Guid SelectedParticipantId { get; set; }
}

/// <summary>
/// ViewModel for adding a new participant
/// </summary>
public class AddParticipantViewModel
{
    public HouseLeader HouseLeader { get; set; } = null!;
    public House House { get; set; } = null!;
    public Tournament ActiveTournament { get; set; } = null!;
    public Participant Participant { get; set; } = new();
}