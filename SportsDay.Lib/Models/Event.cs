using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SportsDay.Lib.Enums;

namespace SportsDay.Lib.Models;

public class Event : BaseEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public EventClass Class { get; set; }

    [Required]
    [StringLength(10)]
    public string AgeGroup { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string GenderGroup { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;

    [Required]
    public EventType Type { get; set; }

    public decimal? Record { get; set; }

    [StringLength(100)]
    public string? RecordHolderName { get; set; }

    [Required]
    [StringLength(20)]
    public string PointSystem { get; set; } = "9,7,6,5,4,3,2,1"; // Default point system

    [Required]
    public Guid DivisionId { get; set; }

    [Required]
    public Guid TournamentId { get; set; }

    public DateTime? ScheduledTime { get; set; }

    public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed, Cancelled

    // Navigation properties
    [ForeignKey("DivisionId")]
    public virtual Division Division { get; set; } = null!;

    [ForeignKey("TournamentId")]
    public virtual Tournament Tournament { get; set; } = null!;

    public virtual ICollection<Result> Results { get; set; } = new List<Result>();
    public virtual ICollection<EventUpdate> Updates { get; set; } = new List<EventUpdate>();
}
