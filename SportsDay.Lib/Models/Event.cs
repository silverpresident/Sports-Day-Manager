using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SportsDay.Lib.Enums;

namespace SportsDay.Lib.Models;

public class Event : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    public int EventNumber { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DivisionType GenderGroup { get; set; }

    [Required]
    public EventClass ClassGroup { get; set; }

    [Required]
    [StringLength(10)]
    public string AgeGroup { get; set; } = string.Empty;

    public int ParticipantMaxAge { get; set; } = 0;

    public int ParticipantLimit { get; set; } = 0; // 0 means no limit
    
    [NotMapped]
    public DivisionType Division => GenderGroup;
    
    [NotMapped]
    public DivisionType DivisionId => GenderGroup;
    
    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty; //track or field

    [Required]
    public EventType Type { get; set; } //speed of distance

    public decimal? Record { get; set; }

    [StringLength(100)]
    public string? RecordHolder { get; set; }

    [Required]
    [StringLength(20)]
    public string PointSystem { get; set; } = "9,7,6,5,4,3,2,1"; // Default point system

    [Required]
    public Guid TournamentId { get; set; }

    public DateTime? ScheduledTime { get; set; }

    public EventStatus Status { get; set; } = EventStatus.Scheduled; // Scheduled, InProgress, Completed, Cancelled

    // Navigation properties
    [ForeignKey("TournamentId")]
    [ValidateNever]
    public virtual Tournament Tournament { get; set; } = null!;

    public virtual ICollection<Result> Results { get; set; } = new List<Result>();
    public virtual ICollection<EventUpdate> Updates { get; set; } = new List<EventUpdate>();
}
