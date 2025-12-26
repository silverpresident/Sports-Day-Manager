using SportsDay.Lib.Enums;

namespace SportsDay.Lib.ViewModels;

/// <summary>
/// Represents a record entry that can be either an existing event record or a new record from a result.
/// </summary>
public class RecordViewModel
{
    /// <summary>
    /// The event ID associated with this record.
    /// </summary>
    public Guid EventId { get; set; }

    /// <summary>
    /// The result ID if this is a new record from a result, null for existing event records.
    /// </summary>
    public Guid? ResultId { get; set; }

    /// <summary>
    /// The name of the event.
    /// </summary>
    public string EventName { get; set; } = string.Empty;

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
    /// The record value (time in seconds or distance in meters).
    /// </summary>
    public decimal? RecordValue { get; set; }

    /// <summary>
    /// The name of the record holder.
    /// </summary>
    public string? RecordHolder { get; set; }

    /// <summary>
    /// The house name if this is a new record from a result.
    /// </summary>
    public string? HouseName { get; set; }

    /// <summary>
    /// The house color if this is a new record from a result.
    /// </summary>
    public string? HouseColor { get; set; }

    /// <summary>
    /// The year the record was set.
    /// </summary>
    public int? YearSet { get; set; }

    /// <summary>
    /// The tournament name if this is a new record.
    /// </summary>
    public string? TournamentName { get; set; }

    /// <summary>
    /// Indicates whether this is a new record from the current/recent tournament.
    /// </summary>
    public bool IsNewRecord { get; set; }

    /// <summary>
    /// The date when the record was set (for new records).
    /// </summary>
    public DateTime? DateSet { get; set; }

    /// <summary>
    /// Additional notes about the record.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets the formatted record value with appropriate unit.
    /// </summary>
    public string FormattedRecord => RecordValue.HasValue
        ? $"{RecordValue.Value:0.##} {(EventType == EventType.Speed ? "s" : "m")}"
        : "N/A";

    /// <summary>
    /// Gets the record type description.
    /// </summary>
    public string RecordType => IsNewRecord ? "New Record" : "Existing Record";
}

/// <summary>
/// ViewModel for the Records Index page containing both existing and new records.
/// </summary>
public class RecordsIndexViewModel
{
    /// <summary>
    /// Existing records from events.
    /// </summary>
    public IEnumerable<RecordViewModel> ExistingRecords { get; set; } = Enumerable.Empty<RecordViewModel>();

    /// <summary>
    /// New records from results in the active tournament.
    /// </summary>
    public IEnumerable<RecordViewModel> NewRecords { get; set; } = Enumerable.Empty<RecordViewModel>();

    /// <summary>
    /// All records combined (existing + new).
    /// </summary>
    public IEnumerable<RecordViewModel> AllRecords => ExistingRecords.Concat(NewRecords);

    /// <summary>
    /// Available categories for filtering.
    /// </summary>
    public IEnumerable<string> Categories { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// Available years for filtering.
    /// </summary>
    public IEnumerable<int> Years { get; set; } = Enumerable.Empty<int>();

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
    /// The currently selected year filter.
    /// </summary>
    public int? SelectedYear { get; set; }

    /// <summary>
    /// Filter to show only new records.
    /// </summary>
    public bool ShowNewRecordsOnly { get; set; }

    /// <summary>
    /// Filter to show only existing records.
    /// </summary>
    public bool ShowExistingRecordsOnly { get; set; }

    /// <summary>
    /// The active tournament name.
    /// </summary>
    public string? ActiveTournamentName { get; set; }

    /// <summary>
    /// Total count of existing records.
    /// </summary>
    public int ExistingRecordCount => ExistingRecords.Count();

    /// <summary>
    /// Total count of new records.
    /// </summary>
    public int NewRecordCount => NewRecords.Count();
}