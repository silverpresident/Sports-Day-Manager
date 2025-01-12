using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsDay.Lib.Models;

public class Participant
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public int HouseId { get; set; }
    public House? House { get; set; }

    [Required]
    public Guid DivisionId { get; set; }
    public Division? Division { get; set; }

    [Required]
    public Guid TournamentId { get; set; }
    public Tournament? Tournament { get; set; }

    public int Points { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public virtual ICollection<Result> Results { get; set; } = new List<Result>();
}
