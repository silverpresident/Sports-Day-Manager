using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsDay.Lib.Models;

/// <summary>
/// Represents a class group for events and participants.
/// Class groups are age-based categories (Open, Class 1-4).
/// </summary>
public class EventClassGroup : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int ClassGroupNumber { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Maximum age for participants in this class group.
    /// 0 means no age limit (Open class).
    /// </summary>
    public int MaxParticipantAge { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }
}
