using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsDay.Lib.Models;

public class House
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Color { get; set; } = string.Empty;

    [StringLength(255)]
    public string? LogoUrl { get; set; }

    // Navigation properties
    public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();
    public virtual ICollection<Result> Results { get; set; } = new List<Result>();
}
