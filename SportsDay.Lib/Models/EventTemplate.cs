using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SportsDay.Lib.Enums;

namespace SportsDay.Lib.Models;

/// <summary>
/// Represents a template for events that can be copied to create events in a tournament.
/// Event templates are independent of tournaments and store standard event configurations.
/// </summary>
public class EventTemplate : BaseEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Event Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Gender Group")]
    public DivisionType GenderGroup { get; set; }

    [Required]
    [Display(Name = "Class Group")]
    public EventClass ClassGroup { get; set; }

    [Required]
    [StringLength(10)]
    [Display(Name = "Age Group")]
    public string AgeGroup { get; set; } = string.Empty;

    [Display(Name = "Max Age")]
    public int ParticipantMaxAge { get; set; } = 0;

    [Display(Name = "Participant Limit")]
    public int ParticipantLimit { get; set; } = 0; // 0 means no limit

    [NotMapped]
    public DivisionType Division => GenderGroup;

    [Required]
    [StringLength(50)]
    [Display(Name = "Category")]
    public string Category { get; set; } = string.Empty; // Track or Field

    [Required]
    [Display(Name = "Event Type")]
    public EventType Type { get; set; } // Speed or Distance

    [Display(Name = "Record")]
    public decimal? Record { get; set; }

    [StringLength(100)]
    [Display(Name = "Record Holder")]
    public string? RecordHolder { get; set; }

    [Display(Name = "Record Setting Year")]
    public int? RecordSettingYear { get; set; }

    [StringLength(500)]
    [Display(Name = "Record Note")]
    public string? RecordNote { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "Point System")]
    public string PointSystem { get; set; } = "9,7,6,5,4,3,2,1"; // Default point system

    [Display(Name = "Active")]
    public bool IsActive { get; set; } = true;

    [StringLength(500)]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    /// <summary>
    /// Creates an Event from this template for the specified tournament.
    /// </summary>
    /// <param name="tournamentId">The tournament to create the event for.</param>
    /// <param name="eventNumber">The event number in the tournament.</param>
    /// <param name="createdBy">The user creating the event.</param>
    /// <returns>A new Event based on this template.</returns>
    public Event ToEvent(Guid tournamentId, int eventNumber, string createdBy)
    {
        return new Event
        {
            Id = Guid.NewGuid(),
            EventNumber = eventNumber,
            Name = Name,
            GenderGroup = GenderGroup,
            ClassGroup = ClassGroup,
            AgeGroup = AgeGroup,
            ParticipantMaxAge = ParticipantMaxAge,
            ParticipantLimit = ParticipantLimit,
            Category = Category,
            Type = Type,
            Record = Record,
            RecordHolder = RecordHolder,
            PointSystem = PointSystem,
            TournamentId = tournamentId,
            Status = EventStatus.Scheduled,
            CreatedAt = DateTime.Now,
            CreatedBy = createdBy
        };
    }
}