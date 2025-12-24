using System;
using System.ComponentModel.DataAnnotations.Schema;
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
        public Tournament? Tournament { get; set; }

        public int HouseId { get; set; }
        public House? House { get; set; }

        public DivisionType Division { get; set; }

        public int Points { get; set; }
    }
}
