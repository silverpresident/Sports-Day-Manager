using System;
using System.ComponentModel.DataAnnotations;

namespace SportsDay.Lib.Models
{
    public class BaseEntity
    {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        [StringLength(256)]
        public string CreatedBy { get; set; }
        [StringLength(256)]
        public string UpdatedBy { get; set; }

        public BaseEntity()
        {
            CreatedAt = DateTime.Now;
            CreatedBy = "system";
            UpdatedBy = "system";
        }
    }
}
