using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using SportsDay.Lib.Enums;

namespace SportsDay.Lib.Models
{
    public class TournamentHouseSummary : BaseEntity
    {
        public TournamentHouseSummary(Tournament tournament, House house, DivisionType division)
        {
            Tournament = tournament ?? throw new ArgumentNullException(nameof(tournament));
            House = house ?? throw new ArgumentNullException(nameof(house));
            Division = division;

            TournamentId = tournament.Id;
            HouseId = house.Id;
        }

        protected TournamentHouseSummary() { } // For EF Core

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid TournamentId { get; set; }
        [ForeignKey("TournamentId")]
        [ValidateNever]
        public Tournament? Tournament { get; set; }

        public int HouseId { get; set; }
        
        [ValidateNever]
        [ForeignKey("HouseId")]

        public House? House { get; set; }

        public DivisionType Division { get; set; }

        /// <summary>
        /// The class group number (0-4) for this summary.
        /// 0 = Open, 1 = Class 1, 2 = Class 2, 3 = Class 3, 4 = Class 4
        /// </summary>
        public int ClassGroupNumber { get; set; }

        public int Points { get; set; }
    }
}
