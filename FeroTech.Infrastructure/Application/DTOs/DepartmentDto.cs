using System;
using System.ComponentModel.DataAnnotations;

namespace FeroTech.Infrastructure.Application.DTOs
{
    public class DepartmentDto
    {
        public Guid? DepartmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string? DepartmentName { get; set; }

        [StringLength(50)]
        public string? DepartmentCode { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
