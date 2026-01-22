using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sphere_Schedule_App.Core.Models
{
    public class Reminder
    {
        [Key]
        public Guid ReminderID { get; set; }

        [Required]
        public Guid UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        public Guid? TaskID { get; set; }

        [ForeignKey("TaskID")]
        public UserTask UserTask { get; set; }

        public Guid? AppointmentID { get; set; }

        [ForeignKey("AppointmentID")]
        public Appointment Appointment { get; set; }

        [Required]
        [MaxLength(20)]
        public string ReminderType { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string? Message { get; set; }

        [Required]
        public DateTime ReminderDateTime { get; set; }

        public bool NotifyViaEmail { get; set; } = true;
        public bool NotifyViaPush { get; set; } = true;

        [MaxLength(20)]
        public string Status { get; set; } = "pending";

        public bool IsRecurring { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SentAt { get; set; }
    }
}