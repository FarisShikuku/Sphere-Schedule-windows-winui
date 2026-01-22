using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sphere_Schedule_App.Core.Models
{
    public class DailyStat
    {
        [Key]
        public Guid StatID { get; set; }

        [Required]
        public Guid UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        [Required]
        public DateTime StatDate { get; set; }

        public int TotalTasks { get; set; } = 0;
        public int CompletedTasks { get; set; } = 0;
        public int IncompleteTasks { get; set; } = 0;
        public int OverdueTasks { get; set; } = 0;
        public int PersonalTasks { get; set; } = 0;
        public int JobTasks { get; set; } = 0;
        public int UnspecifiedTasks { get; set; } = 0;
        public int AppointmentTasks { get; set; } = 0;
        public int TotalAppointments { get; set; } = 0;
        public int CompletedAppointments { get; set; } = 0;
        public int CancelledAppointments { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        public decimal? ProductivityScore { get; set; }

        public int CurrentStreakDays { get; set; } = 0;
        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
    }
}