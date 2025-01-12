using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportsDay.Lib.Models;

namespace SportsDay.Lib.Data
{
    public class SportsDayDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public SportsDayDbContext(DbContextOptions<SportsDayDbContext> options)
            : base(options)
        {
        }

        public DbSet<House> Houses { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<EventUpdate> EventUpdates { get; set; }
        public DbSet<TournamentHouseSummary> TournamentHouseSummaries { get; set; }
    }
}
