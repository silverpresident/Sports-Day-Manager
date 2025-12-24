using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsDay.Lib.Models
{
    public class HouseLeader : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public int HouseId { get; set; }
        public House? House { get; set; }

        [Required]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;
    }
}
