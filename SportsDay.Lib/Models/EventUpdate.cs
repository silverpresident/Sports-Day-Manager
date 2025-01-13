using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsDay.Lib.Models;

public class EventUpdate : BaseEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(500)]
    public string Message { get; set; } = string.Empty;

    [Required]
    public Guid EventId { get; set; }

    [Required]
    public Guid TournamentId { get; set; }

    // Navigation properties
    [ForeignKey("EventId")]
    public virtual Event Event { get; set; } = null!;

    [ForeignKey("TournamentId")]
    public virtual Tournament Tournament { get; set; } = null!;
}
