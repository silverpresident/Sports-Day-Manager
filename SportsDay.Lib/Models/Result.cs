using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsDay.Lib.Models;

public class Result : BaseEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid EventId { get; set; }

    [Required]
    public Guid ParticipantId { get; set; }

    [Required]
    public int HouseId { get; set; }

    public int? Placement { get; set; }

    public decimal? SpeedOrDistance { get; set; }

    public int Points { get; set; }

    public bool IsNewRecord { get; set; }

    [Required]
    public Guid TournamentId { get; set; }

    // Navigation properties
    [ForeignKey("EventId")]
    public virtual Event Event { get; set; } = null!;

    [ForeignKey("ParticipantId")]
    public virtual Participant Participant { get; set; } = null!;

    [ForeignKey("HouseId")]
    public virtual House House { get; set; } = null!;

    [ForeignKey("TournamentId")]
    public virtual Tournament Tournament { get; set; } = null!;
}
