using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sphere_Schedule_App.Core.Models
{
    public class Participant
    {
        [Key]
        public Guid ParticipantID { get; set; }

        [Required]
        public Guid AppointmentID { get; set; }

        [ForeignKey("AppointmentID")]
        public Appointment Appointment { get; set; }

        public Guid? UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(100)]
        public string? FullName { get; set; }

        [MaxLength(20)]
        public string InvitationStatus { get; set; } = "pending";

        public DateTime? ResponseReceivedAt { get; set; }

        [MaxLength(20)]
        public string ParticipantRole { get; set; } = "attendee";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}