using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsDay.Lib.Models;

public class Division : BaseEntity
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Guid TournamentId { get; set; }

    // Navigation properties
    [ForeignKey("TournamentId")]
    public virtual Tournament? Tournament { get; set; }
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();
    public virtual ICollection<TournamentHouseSummary> HouseSummaries { get; set; } = new List<TournamentHouseSummary>();
}
