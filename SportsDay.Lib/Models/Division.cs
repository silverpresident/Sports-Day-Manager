using System.ComponentModel.DataAnnotations;

namespace SportsDay.Lib.Models;

public class Division
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    // Navigation properties
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();
    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
}
