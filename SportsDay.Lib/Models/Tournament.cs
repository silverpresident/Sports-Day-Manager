using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SportsDay.Lib.Models
{
    public class Tournament : BaseEntity
    {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    
        [Required]
        public DateTime TournamentDate { get; set; }
        
        [NotMapped]
        public DateTime StartDate => TournamentDate;
        
        [NotMapped]
        public DateTime EndDate => TournamentDate;
    
        public bool IsActive { get; set; }

        // Navigation properties
        public virtual ICollection<Event> Events { get; set; } = new List<Event>();
        public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();
        public virtual ICollection<Result> Results { get; set; } = new List<Result>();
        public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
        public virtual ICollection<EventUpdate> Updates { get; set; } = new List<EventUpdate>();
        public virtual ICollection<TournamentHouseSummary> HouseSummaries { get; set; } = new List<TournamentHouseSummary>();
    }
}
