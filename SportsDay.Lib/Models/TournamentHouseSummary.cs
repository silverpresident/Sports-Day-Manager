using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsDay.Lib.Models
{
    public class TournamentHouseSummary
    {
        public Guid Id { get; set; }

        public Guid TournamentId { get; set; }
        public Tournament Tournament { get; set; }

        public int HouseId { get; set; }
        public House House { get; set; }

        public string AgeGroup { get; set; }
        public string Gender { get; set; }

        public int Points { get; set; }
    }
}
