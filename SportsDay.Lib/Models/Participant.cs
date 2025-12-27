using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SportsDay.Lib.Enums;

namespace SportsDay.Lib.Models;

public class Participant : BaseEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
    public Guid TournamentId { get; set; }
    [ValidateNever]
    public Tournament? Tournament { get; set; }

    public int Points { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
    
    [Required]
    public DivisionType GenderGroup { get; set; }
    
    [NotMapped]
    public DivisionType Division => GenderGroup;
    
    public DateTime DateOfBirth { get; set; }
    public int AgeInYears { get; set; }

    [Required]
    [StringLength(50)]
    public string AgeGroup { get; set; } = string.Empty;
    
    public EventClass EventClassGroup { get; set; }
    
    [NotMapped]
    public EventClass ClassGroup => EventClassGroup;


    public virtual ICollection<Result> Results { get; set; } = new List<Result>();
}
