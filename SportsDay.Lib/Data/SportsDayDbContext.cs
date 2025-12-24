using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportsDay.Lib.Models;

namespace SportsDay.Lib.Data;

public class SportsDayDbContext : IdentityDbContext
{
    public SportsDayDbContext(DbContextOptions<SportsDayDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tournament> Tournaments { get; set; } = null!;
    public DbSet<Division> Divisions { get; set; } = null!;
    public DbSet<House> Houses { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<Participant> Participants { get; set; } = null!;
    public DbSet<Result> Results { get; set; } = null!;
    public DbSet<Announcement> Announcements { get; set; } = null!;
    public DbSet<EventUpdate> EventUpdates { get; set; } = null!;
    public DbSet<TournamentHouseSummary> TournamentHouseSummaries { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Tournament relationships
        modelBuilder.Entity<Tournament>()
            .HasMany(t => t.Divisions)
            .WithOne(d => d.Tournament)
            .HasForeignKey(d => d.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Tournament>()
            .HasMany(t => t.Events)
            .WithOne(e => e.Tournament)
            .HasForeignKey(e => e.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Tournament>()
            .HasMany(t => t.Participants)
            .WithOne(p => p.Tournament)
            .HasForeignKey(p => p.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Tournament>()
            .HasMany(t => t.Results)
            .WithOne(r => r.Tournament)
            .HasForeignKey(r => r.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Tournament>()
            .HasMany(t => t.Announcements)
            .WithOne(a => a.Tournament)
            .HasForeignKey(a => a.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Tournament>()
            .HasMany(t => t.Updates)
            .WithOne(u => u.Tournament)
            .HasForeignKey(u => u.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Tournament>()
            .HasMany(t => t.HouseSummaries)
            .WithOne(s => s.Tournament)
            .HasForeignKey(s => s.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Division relationships
        modelBuilder.Entity<Division>()
            .HasMany(d => d.Events)
            .WithOne(e => e.Division)
            .HasForeignKey(e => e.DivisionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Division>()
            .HasMany(d => d.Participants)
            .WithOne(p => p.Division)
            .HasForeignKey(p => p.DivisionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Division>()
            .HasMany(d => d.HouseSummaries)
            .WithOne(s => s.Division)
            .HasForeignKey(s => s.DivisionId)
            .OnDelete(DeleteBehavior.Cascade);

        // House relationships
        modelBuilder.Entity<House>()
            .HasMany(h => h.Participants)
            .WithOne(p => p.House)
            .HasForeignKey(p => p.HouseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<House>()
            .HasMany(h => h.Results)
            .WithOne(r => r.House)
            .HasForeignKey(r => r.HouseId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<House>()
            .HasMany(h => h.TournamentSummaries)
            .WithOne(s => s.House)
            .HasForeignKey(s => s.HouseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Event relationships
        modelBuilder.Entity<Event>()
            .HasMany(e => e.Results)
            .WithOne(r => r.Event)
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Event>()
            .HasMany(e => e.Updates)
            .WithOne(u => u.Event)
            .HasForeignKey(u => u.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        // Participant relationships
        modelBuilder.Entity<Participant>()
            .HasMany(p => p.Results)
            .WithOne(r => r.Participant)
            .HasForeignKey(r => r.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
