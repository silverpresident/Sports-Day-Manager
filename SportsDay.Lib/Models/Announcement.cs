using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SportsDay.Lib.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SportsDay.Lib.Models;

public class Announcement : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Required]
    [StringLength(500)]
    public string Body { get; set; } = string.Empty;

    [StringLength(100)]
    public string Tag { get; set; } = string.Empty;

    [Required]
    public AnnouncementPriority Priority { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public bool IsEnabled { get; set; } = true;

    [Required]
    public Guid TournamentId { get; set; }

    // Navigation properties
    [ForeignKey("TournamentId")]
    [ValidateNever]
    public virtual Tournament Tournament { get; set; } = null!;
}
