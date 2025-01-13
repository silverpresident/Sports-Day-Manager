using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsDay.Lib.Models
{
    public class TournamentHouseSummary : BaseEntity
    {
        public TournamentHouseSummary(Tournament tournament, House house, Division division)
        {
            Tournament = tournament ?? throw new ArgumentNullException(nameof(tournament));
            House = house ?? throw new ArgumentNullException(nameof(house));
            Division = division ?? throw new ArgumentNullException(nameof(division));
            
            TournamentId = tournament.Id;
            HouseId = house.Id;
            DivisionId = division.Id;
        }

        protected TournamentHouseSummary() { } // For EF Core

        public Guid Id { get; set; }

        public Guid TournamentId { get; set; }
        public Tournament? Tournament { get; set; }

        public int HouseId { get; set; }
        public House? House { get; set; }

        public Guid DivisionId { get; set; }
        public Division? Division { get; set; }

        public int Points { get; set; }
    }
}
