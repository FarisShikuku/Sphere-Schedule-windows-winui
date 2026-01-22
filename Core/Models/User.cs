using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sphere_Schedule_App.Core.Models
{
    public class User
    {
        [Key]
        public Guid UserID { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(100)]
        public string? Username { get; set; }

        [MaxLength(512)]
        public string? PasswordHash { get; set; }

        [MaxLength(512)]
        public string? PasswordSalt { get; set; }

        [MaxLength(100)]
        public string? DisplayName { get; set; }

        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? AvatarURL { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public bool EmailVerified { get; set; } = false;
        public bool TwoFactorEnabled { get; set; } = false;
        public bool LockoutEnabled { get; set; } = false;
        public DateTime? LockoutEnd { get; set; }
        public int AccessFailedCount { get; set; } = 0;

        [MaxLength(20)]
        public string AccountType { get; set; } = "free";

        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }

        [MaxLength(255)]
        public string? GoogleID { get; set; }

        [MaxLength(255)]
        public string? MicrosoftID { get; set; }

        [MaxLength(255)]
        public string? FacebookID { get; set; }

        public string Preferences { get; set; } = @"{
            ""theme"": ""light"",
            ""timezone"": ""UTC"",
            ""language"": ""en"",
            ""notificationSettings"": {
                ""email"": true,
                ""push"": true,
                ""sms"": false
            },
            ""workHours"": {
                ""start"": ""09:00"",
                ""end"": ""17:00""
            },
            ""weekStartDay"": 1
        }";

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public DateTime? LastActivityAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}