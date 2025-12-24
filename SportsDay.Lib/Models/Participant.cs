using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SportsDay.Lib.Enums;

namespace SportsDay.Lib.Models;

public class Participant : BaseEntity
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    [Required]
    public int HouseId { get; set; }
    public House? House { get; set; }

    [Required]
    public DivisionType Division { get; set; }

    [Required]
    public Guid TournamentId { get; set; }
    public Tournament? Tournament { get; set; }

    public int Points { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
    [Required]
    [StringLength(10)]
    public string GenderGroup { get; set; } = string.Empty;
    [Required]
    [StringLength(10)]
    public string AgeGroup { get; set; } = string.Empty;
    public int ClassGroup { get; set; }


    public virtual ICollection<Result> Results { get; set; } = new List<Result>();
}
