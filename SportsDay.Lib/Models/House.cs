using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsDay.Lib.Models;

public class House : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    //Hex colour
    public string Color { get; set; } = string.Empty;
    [Required]
    [StringLength(20)]
    public string ColorName { get; set; } = string.Empty;

    [StringLength(200)]
    public string? LogoUrl { get; set; }

    // Navigation properties
    public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();
    public virtual ICollection<Result> Results { get; set; } = new List<Result>();
    public virtual ICollection<TournamentHouseSummary> TournamentSummaries { get; set; } = new List<TournamentHouseSummary>();
}
