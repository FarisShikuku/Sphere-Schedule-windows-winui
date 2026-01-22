using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sphere_Schedule_App.Core.Models
{
    public class UserTask
    {
        [Key]
        public Guid TaskID { get; set; }

        [Required]
        public Guid UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [MaxLength(20)]
        public string Category { get; set; } = "unspecified";

        [MaxLength(30)]
        public string TaskType { get; set; } = "general";

        [MaxLength(10)]
        public string PriorityLevel { get; set; } = "medium";

        [MaxLength(20)]
        public string Status { get; set; } = "pending";

        [Range(0, 100)]
        public int CompletionPercentage { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public TimeSpan? DueTime { get; set; }
        public DateTime? StartDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public DateTime? EndDate { get; set; }
        public TimeSpan? EndTime { get; set; }
        public DateTime? CompletedAt { get; set; }

        [MaxLength(255)]
        public string? LocationName { get; set; }

        [MaxLength(500)]
        public string? LocationAddress { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public int? EstimatedDurationMinutes { get; set; }
        public int? ActualDurationMinutes { get; set; }
        public int TimeSpentMinutes { get; set; } = 0;

        public bool IsRecurring { get; set; } = false;
        public string? RecurrenceRule { get; set; }
        public Guid? ParentTaskID { get; set; }

        [MaxLength(255)]
        public string? ExternalID { get; set; }

        [MaxLength(50)]
        public string? ExternalSource { get; set; }

        [MaxLength(20)]
        public string ExternalSyncStatus { get; set; } = "not_synced";

        [MaxLength(500)]
        public string? Tags { get; set; }

        public string? Notes { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}