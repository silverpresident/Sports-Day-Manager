using System.ComponentModel.DataAnnotations;

namespace SportsDay.Lib.Models;

public class Tournament
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public int Year { get; set; }

    [Required]
    public bool IsActive { get; set; }

    // Navigation properties
    public virtual ICollection<Division> Divisions { get; set; } = new List<Division>();
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
    public virtual ICollection<Result> Results { get; set; } = new List<Result>();
    public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
    public virtual ICollection<EventUpdate> Updates { get; set; } = new List<EventUpdate>();
}

// Enums for Event-related properties
public enum EventClass
{
    Open = 0,
    Class1 = 1,
    Class2 = 2,
    Class3 = 3,
    Class4 = 4
}

public enum EventType
{
    Distance,
    Speed
}

public enum AnnouncementPriority
{
    Info,
    Warning,
    Danger,
    Primary,
    Secondary
}
