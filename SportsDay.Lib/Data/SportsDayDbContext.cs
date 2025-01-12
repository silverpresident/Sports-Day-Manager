using Microsoft.EntityFrameworkCore;
using SportsDay.Lib.Models;

namespace SportsDay.Lib.Data;

public class SportsDayDbContext : DbContext
{
    public SportsDayDbContext(DbContextOptions<SportsDayDbContext> options)
        : base(options)
    {
    }

    public DbSet<House> Houses { get; set; } = null!;
    public DbSet<Tournament> Tournaments { get; set; } = null!;
    public DbSet<Division> Divisions { get; set; } = null!;
    public DbSet<Event> Events { get; set; } = null!;
    public DbSet<Participant> Participants { get; set; } = null!;
    public DbSet<Result> Results { get; set; } = null!;
    public DbSet<Announcement> Announcements { get; set; } = null!;
    public DbSet<EventUpdate> EventUpdates { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure House entity
        modelBuilder.Entity<House>()
            .HasMany(h => h.Participants)
            .WithOne(p => p.House)
            .HasForeignKey(p => p.HouseId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<House>()
            .HasMany(h => h.Results)
            .WithOne(r => r.House)
            .HasForeignKey(r => r.HouseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Division entity
        modelBuilder.Entity<Division>()
            .HasMany(d => d.Events)
            .WithOne(e => e.Division)
            .HasForeignKey(e => e.DivisionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Division>()
            .HasMany(d => d.Participants)
            .WithOne(p => p.Division)
            .HasForeignKey(p => p.DivisionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure Tournament entity
        modelBuilder.Entity<Tournament>()
            .HasMany(t => t.Events)
            .WithOne(e => e.Tournament)
            .HasForeignKey(e => e.TournamentId)
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

        // Configure Event entity
        modelBuilder.Entity<Event>()
            .HasMany(e => e.Results)
            .WithOne(r => r.Event)
            .HasForeignKey(r => r.EventId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Event>()
            .HasMany(e => e.Updates)
            .WithOne(u => u.Event)
            .HasForeignKey(u => u.EventId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Participant entity
        modelBuilder.Entity<Participant>()
            .HasMany(p => p.Results)
            .WithOne(r => r.Participant)
            .HasForeignKey(r => r.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed default houses
        modelBuilder.Entity<House>().HasData(
            new House { Id = 1, Name = "Beckford", Color = "Red" },
            new House { Id = 2, Name = "Bell", Color = "Green" },
            new House { Id = 3, Name = "Campbell", Color = "Orange" },
            new House { Id = 4, Name = "Nutall", Color = "Purple" },
            new House { Id = 5, Name = "Smith", Color = "Blue" },
            new House { Id = 6, Name = "Wortley", Color = "Yellow" }
        );

        // Seed default divisions
        modelBuilder.Entity<Division>().HasData(
            new Division { Id = Guid.NewGuid(), Name = "BOYS" },
            new Division { Id = Guid.NewGuid(), Name = "GIRLS" }
        );
    }
}
