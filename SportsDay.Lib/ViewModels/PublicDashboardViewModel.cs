using SportsDay.Lib.Models;

namespace SportsDay.Lib.ViewModels;

/// <summary>
/// ViewModel for the Public Dashboard page
/// </summary>
public class PublicDashboardViewModel
{
    /// <summary>
    /// The currently active tournament
    /// </summary>
    public Tournament? ActiveTournament { get; set; }

    /// <summary>
    /// Active announcements for the tournament
    /// </summary>
    public List<Announcement> Announcements { get; set; } = new();

    /// <summary>
    /// Recent event updates
    /// </summary>
    public List<EventUpdate> Updates { get; set; } = new();

    /// <summary>
    /// House summaries for leaderboard display
    /// </summary>
    public List<TournamentHouseSummary> Summaries { get; set; } = new();

    /// <summary>
    /// Number of active divisions
    /// </summary>
    public int DivisionCount { get; set; }

    /// <summary>
    /// Total number of events in the tournament
    /// </summary>
    public int TotalEvents { get; set; }

    /// <summary>
    /// Number of completed events
    /// </summary>
    public int CompletedEvents { get; set; }

    /// <summary>
    /// Total number of participants
    /// </summary>
    public int TotalParticipants { get; set; }

    /// <summary>
    /// Number of results recorded
    /// </summary>
    public int TotalResults { get; set; }

    /// <summary>
    /// Number of new records set
    /// </summary>
    public int NewRecords { get; set; }
}