using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sphere_Schedule_App.Core.Models
{
    public class Appointment
    {
        [Key]
        public Guid AppointmentID { get; set; }

        [Required]
        public Guid UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [MaxLength(30)]
        public string AppointmentType { get; set; } = "general";

        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public DateTime EndDateTime { get; set; }

        public bool AllDayEvent { get; set; } = false;

        [MaxLength(500)]
        public string? Location { get; set; }

        public bool IsVirtual { get; set; } = false;

        [MaxLength(500)]
        public string? MeetingLink { get; set; }

        [MaxLength(50)]
        public string? MeetingPlatform { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "scheduled";

        public int ReminderMinutesBefore { get; set; } = 15;
        public bool IsRecurring { get; set; } = false;
        public string? RecurrencePattern { get; set; }

        [MaxLength(7)]
        public string CalendarColor { get; set; } = "#2196F3";

        [MaxLength(255)]
        public string? ExternalEventID { get; set; }

        [MaxLength(20)]
        public string ExternalSyncStatus { get; set; } = "not_synced";

        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}