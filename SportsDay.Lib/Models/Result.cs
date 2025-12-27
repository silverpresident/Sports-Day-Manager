using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SportsDay.Lib.Models;

public class Result : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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

    public bool IsDisqualified { get; set; }

    [MaxLength(10)]
    public string? ResultLabel { get; set; }

    [Required]
    public Guid TournamentId { get; set; }

    // Navigation properties
    [ForeignKey("EventId")]
    [ValidateNever]
    public virtual Event Event { get; set; } = null!;

    [ForeignKey("ParticipantId")]
    [ValidateNever]
    public virtual Participant Participant { get; set; } = null!;

    [ForeignKey("HouseId")]
    [ValidateNever]
    public virtual House House { get; set; } = null!;

    [ForeignKey("TournamentId")]
    [ValidateNever]
    public virtual Tournament Tournament { get; set; } = null!;
}
