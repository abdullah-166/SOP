using System;
using System.ComponentModel.DataAnnotations;

namespace FeroTech.Infrastructure.Application.DTOs{
    public class CategoryDto{
        public Guid? CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
