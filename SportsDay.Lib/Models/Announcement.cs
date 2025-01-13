using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SportsDay.Lib.Enums;

namespace SportsDay.Lib.Models;

public class Announcement : BaseEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(500)]
    public string Body { get; set; } = string.Empty;

    [Required]
    public AnnouncementPriority Priority { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public bool IsEnabled { get; set; } = true;

    [Required]
    public Guid TournamentId { get; set; }

    // Navigation properties
    [ForeignKey("TournamentId")]
    public virtual Tournament Tournament { get; set; } = null!;
}
