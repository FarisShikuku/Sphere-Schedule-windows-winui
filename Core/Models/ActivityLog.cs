using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sphere_Schedule_App.Core.Models
{
    public class ActivityLog
    {
        [Key]
        public long LogID { get; set; }

        public Guid? UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        [Required]
        [MaxLength(50)]
        public string ActivityType { get; set; }

        [MaxLength(50)]
        public string? EntityType { get; set; }

        public Guid? EntityID { get; set; }

        [MaxLength(45)]
        public string? IPAddress { get; set; }

        [MaxLength(500)]
        public string? UserAgent { get; set; }

        public string? Details { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "success";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}