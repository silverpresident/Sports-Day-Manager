using System;

namespace SportsDay.Lib.Models
{
    public class BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public BaseEntity()
        {
            CreatedAt = DateTime.Now;
            CreatedBy = "system";
            UpdatedBy = "system";
        }
    }
}
