using SportsDay.Lib.Models;

namespace SportsDay.Lib.ViewModels;

/// <summary>
/// ViewModel for the Admin Dashboard page
/// </summary>
public class AdminDashboardViewModel
{
    /// <summary>
    /// The currently active tournament, if any
    /// </summary>
    public Tournament? ActiveTournament { get; set; }

    /// <summary>
    /// Total number of tournaments in the system
    /// </summary>
    public int TotalTournaments { get; set; }

    /// <summary>
    /// Total number of events (across all tournaments or active tournament)
    /// </summary>
    public int TotalEvents { get; set; }

    /// <summary>
    /// Total number of participants (across all tournaments or active tournament)
    /// </summary>
    public int TotalParticipants { get; set; }

    /// <summary>
    /// Total number of results recorded
    /// </summary>
    public int TotalResults { get; set; }

    /// <summary>
    /// Number of currently active announcements
    /// </summary>
    public int ActiveAnnouncements { get; set; }

    /// <summary>
    /// Number of houses in the system
    /// </summary>
    public int TotalHouses { get; set; }

    /// <summary>
    /// Number of event templates available
    /// </summary>
    public int TotalEventTemplates { get; set; }

    /// <summary>
    /// Recent event updates for the dashboard feed
    /// </summary>
    public List<EventUpdate> RecentUpdates { get; set; } = new();

    /// <summary>
    /// Recent results for the dashboard feed
    /// </summary>
    public List<Result> RecentResults { get; set; } = new();

    /// <summary>
    /// Statistics for the active tournament
    /// </summary>
    public ActiveTournamentStats? ActiveTournamentStats { get; set; }
}

/// <summary>
/// Statistics specific to the active tournament
/// </summary>
public class ActiveTournamentStats
{
    /// <summary>
    /// Number of events in the active tournament
    /// </summary>
    public int EventCount { get; set; }

    /// <summary>
    /// Number of completed events
    /// </summary>
    public int CompletedEvents { get; set; }

    /// <summary>
    /// Number of events in progress
    /// </summary>
    public int InProgressEvents { get; set; }

    /// <summary>
    /// Number of scheduled events
    /// </summary>
    public int ScheduledEvents { get; set; }

    /// <summary>
    /// Number of participants in the active tournament
    /// </summary>
    public int ParticipantCount { get; set; }

    /// <summary>
    /// Number of results in the active tournament
    /// </summary>
    public int ResultCount { get; set; }

    /// <summary>
    /// Number of new records set in the active tournament
    /// </summary>
    public int NewRecords { get; set; }
}