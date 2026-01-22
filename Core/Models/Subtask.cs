using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sphere_Schedule_App.Core.Models
{
    public class Subtask
    {
        [Key]
        public Guid SubtaskID { get; set; }

        [Required]
        public Guid TaskID { get; set; }

        [ForeignKey("TaskID")]
        public UserTask UserTask { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "pending";

        [MaxLength(10)]
        public string Priority { get; set; } = "medium";

        public DateTime? DueDate { get; set; }
        public TimeSpan? DueTime { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int SubtaskOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }
}