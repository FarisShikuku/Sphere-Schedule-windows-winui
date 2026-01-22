using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sphere_Schedule_App.Core.Models
{
    public class SystemSetting
    {
        [Key]
        public Guid SettingID { get; set; }

        [Required]
        [MaxLength(100)]
        public string SettingKey { get; set; }

        public string? SettingValue { get; set; }

        [MaxLength(20)]
        public string SettingType { get; set; } = "string";

        [MaxLength(50)]
        public string? Category { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsEditable { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}