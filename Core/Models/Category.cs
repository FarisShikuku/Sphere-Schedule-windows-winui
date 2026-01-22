using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sphere_Schedule_App.Core.Models
{
    public class Category
    {
        [Key]
        public Guid CategoryID { get; set; }

        [Required]
        public Guid UserID { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; }

        [MaxLength(20)]
        public string CategoryType { get; set; } = "custom";

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(7)]
        public string ColorCode { get; set; } = "#4CAF50";

        [MaxLength(50)]
        public string? IconName { get; set; }

        public int CategoryOrder { get; set; } = 0;
        public bool IsDefault { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}