using SportsDay.Lib.Enums;

namespace SportsDay.Lib.ViewModels;

/// <summary>
/// Represents a result entry for display in views.
/// </summary>
public class ResultViewModel
{
    /// <summary>
    /// The result ID.
    /// </summary>
    public Guid ResultId { get; set; }

    /// <summary>
    /// The event ID associated with this result.
    /// </summary>
    public Guid EventId { get; set; }

    /// <summary>
    /// The name of the event.
    /// </summary>
    public string EventName { get; set; } = string.Empty;

    /// <summary>
    /// The event number.
    /// </summary>
    public int? EventNumber { get; set; }

    /// <summary>
    /// The participant ID.
    /// </summary>
    public Guid ParticipantId { get; set; }

    /// <summary>
    /// The participant's full name.
    /// </summary>
    public string ParticipantName { get; set; } = string.Empty;

    /// <summary>
    /// The division (gender group) of the event.
    /// </summary>
    public DivisionType Division { get; set; }

    /// <summary>
    /// The class group of the event.
    /// </summary>
    public EventClass ClassGroup { get; set; }

    /// <summary>
    /// The category of the event (Track or Field).
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// The type of event (Speed or Distance).
    /// </summary>
    public EventType EventType { get; set; }

    /// <summary>
    /// The placement (1st, 2nd, 3rd, etc.).
    /// </summary>
    public int? Placement { get; set; }

    /// <summary>
    /// The speed (time in seconds) or distance (in meters).
    /// </summary>
    public decimal? SpeedOrDistance { get; set; }

    /// <summary>
    /// The points earned for this result.
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// Indicates whether this result set a new record.
    /// </summary>
    public bool IsNewRecord { get; set; }

    /// <summary>
    /// Indicates whether the participant was disqualified.
    /// </summary>
    public bool IsDisqualified { get; set; }

    public bool IsPublished { get; set; }
    /// <summary>
    /// The result label (e.g., "1st", "2nd", "DQ", "DNS", "DNF").
    /// </summary>
    public string? ResultLabel { get; set; }

    /// <summary>
    /// The house ID.
    /// </summary>
    public int HouseId { get; set; }

    /// <summary>
    /// The house name.
    /// </summary>
    public string HouseName { get; set; } = string.Empty;

    /// <summary>
    /// The house color.
    /// </summary>
    public string HouseColor { get; set; } = string.Empty;

    /// <summary>
    /// The tournament ID.
    /// </summary>
    public Guid TournamentId { get; set; }

    /// <summary>
    /// The tournament name.
    /// </summary>
    public string TournamentName { get; set; } = string.Empty;

    /// <summary>
    /// The date when the result was recorded.
    /// </summary>
    public DateTime RecordedAt { get; set; }

    /// <summary>
    /// Gets the formatted speed/distance value with appropriate unit.
    /// </summary>
    public string FormattedValue => SpeedOrDistance.HasValue
        ? $"{SpeedOrDistance.Value:0.##} {(EventType == EventType.Speed ? "s" : "m")}"
        : "N/A";

    /// <summary>
    /// Gets the placement with ordinal suffix (1st, 2nd, 3rd, etc.).
    /// </summary>
    public string FormattedPlacement => Placement.HasValue
        ? GetOrdinal(Placement.Value)
        : "N/A";

    /// <summary>
    /// Gets the placement badge class based on position.
    /// </summary>
    public string PlacementBadgeClass => Placement switch
    {
        1 => "bg-warning text-dark",
        2 => "bg-secondary",
        3 => "bg-bronze",
        _ => "bg-light text-dark"
    };

    /// <summary>
    /// Gets the placement icon based on position.
    /// </summary>
    public string PlacementIcon => Placement switch
    {
        1 => "bi-trophy-fill",
        2 => "bi-award-fill",
        3 => "bi-award",
        _ => "bi-hash"
    };

    /// <summary>
    /// Converts a number to its ordinal representation.
    /// </summary>
    private static string GetOrdinal(int number)
    {
        if (number <= 0) return number.ToString();

        var suffix = (number % 100) switch
        {
            11 or 12 or 13 => "th",
            _ => (number % 10) switch
            {
                1 => "st",
                2 => "nd",
                3 => "rd",
                _ => "th"
            }
        };

        return $"{number}{suffix}";
    }
}

/// <summary>
/// ViewModel for the Results Index page containing results with filtering options.
/// </summary>
public class ResultsIndexViewModel
{
    /// <summary>
    /// All results matching the current filters.
    /// </summary>
    public IEnumerable<ResultViewModel> Results { get; set; } = Enumerable.Empty<ResultViewModel>();

    /// <summary>
    /// Available categories for filtering.
    /// </summary>
    public IEnumerable<string> Categories { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Available houses for filtering.
    /// </summary>
    public IEnumerable<(int Id, string Name)> Houses { get; set; } = Enumerable.Empty<(int, string)>();

    /// <summary>
    /// Available events for filtering.
    /// </summary>
    public IEnumerable<(Guid Id, string Name)> Events { get; set; } = Enumerable.Empty<(Guid, string)>();

    /// <summary>
    /// The currently selected division filter.
    /// </summary>
    public DivisionType? SelectedDivision { get; set; }

    /// <summary>
    /// The currently selected class filter.
    /// </summary>
    public EventClass? SelectedClass { get; set; }

    /// <summary>
    /// The currently selected category filter.
    /// </summary>
    public string? SelectedCategory { get; set; }

    /// <summary>
    /// The currently selected house filter.
    /// </summary>
    public int? SelectedHouseId { get; set; }

    /// <summary>
    /// The currently selected event filter.
    /// </summary>
    public Guid? SelectedEventId { get; set; }

    /// <summary>
    /// The active tournament name.
    /// </summary>
    public string? ActiveTournamentName { get; set; }

    /// <summary>
    /// Total count of results.
    /// </summary>
    public int TotalResultCount => Results.Count();

    /// <summary>
    /// Count of results with placements (podium finishers).
    /// </summary>
    public int PlacedResultCount => Results.Count(r => r.Placement.HasValue && r.Placement <= 3);

    /// <summary>
    /// Count of new records.
    /// </summary>
    public int NewRecordCount => Results.Count(r => r.IsNewRecord);

    /// <summary>
    /// Total points from all results.
    /// </summary>
    public int TotalPoints => Results.Sum(r => r.Points);
}

/// <summary>
/// ViewModel for result statistics.
/// </summary>
public class ResultStatisticsViewModel
{
    /// <summary>
    /// Total number of results recorded.
    /// </summary>
    public int TotalResults { get; set; }

    /// <summary>
    /// Number of events with results.
    /// </summary>
    public int EventsWithResults { get; set; }

    /// <summary>
    /// Number of participants with results.
    /// </summary>
    public int ParticipantsWithResults { get; set; }

    /// <summary>
    /// Number of new records set.
    /// </summary>
    public int NewRecordsSet { get; set; }

    /// <summary>
    /// Total points awarded.
    /// </summary>
    public int TotalPointsAwarded { get; set; }

    /// <summary>
    /// Results by division.
    /// </summary>
    public Dictionary<DivisionType, int> ResultsByDivision { get; set; } = new();

    /// <summary>
    /// Points by house.
    /// </summary>
    public Dictionary<string, int> PointsByHouse { get; set; } = new();

    /// <summary>
    /// The active tournament name.
    /// </summary>
    public string? ActiveTournamentName { get; set; }
}