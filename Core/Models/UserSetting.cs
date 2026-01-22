using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sphere_Schedule_App.Core.Models
{
    public class UserSetting
    {
        [Key]
        public Guid UserSettingID { get; set; }

        [Required]
        public Guid UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        [Required]
        [MaxLength(100)]
        public string SettingKey { get; set; }

        public string? SettingValue { get; set; }

        [MaxLength(20)]
        public string SettingType { get; set; } = "string";

        public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;
    }
}