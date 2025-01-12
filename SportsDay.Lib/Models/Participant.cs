using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsDay.Lib.Models;

public class Participant
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int HouseId { get; set; }

    [Required]
    public Guid DivisionId { get; set; }

    [Required]
    [StringLength(10)]
    public string Gender { get; set; } = string.Empty;

    [Required]
    public EventClass Class { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey("HouseId")]
    public virtual House House { get; set; } = null!;

    [ForeignKey("DivisionId")]
    public virtual Division Division { get; set; } = null!;

    public virtual ICollection<Result> Results { get; set; } = new List<Result>();
}
