using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsDay.Lib.Models
{
    public class TournamentHouseSummary
    {
        public TournamentHouseSummary(Tournament tournament, House house, string ageGroup, string gender)
        {
            Tournament = tournament ?? throw new ArgumentNullException(nameof(tournament));
            House = house ?? throw new ArgumentNullException(nameof(house));
            AgeGroup = ageGroup ?? throw new ArgumentNullException(nameof(ageGroup));
            Gender = gender ?? throw new ArgumentNullException(nameof(gender));
            
            TournamentId = tournament.Id;
            HouseId = house.Id;
        }

        protected TournamentHouseSummary() { } // For EF Core

        public Guid Id { get; set; }

        public Guid TournamentId { get; set; }
        public Tournament? Tournament { get; set; }

        public int HouseId { get; set; }
        public House? House { get; set; }

        public string? AgeGroup { get; set; }
        public string? Gender { get; set; }

        public int Points { get; set; }
    }
}
