using System;
using System.ComponentModel.DataAnnotations;

namespace FeroTech.Infrastructure.Domain.Entities
{
    public class Department
    {
        [Key]
        public Guid DepartmentId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string? DepartmentName { get; set; }
        [StringLength(50)]
        public string? DepartmentCode { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
