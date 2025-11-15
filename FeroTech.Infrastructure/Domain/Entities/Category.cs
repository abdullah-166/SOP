using System;
using System.ComponentModel.DataAnnotations;

namespace FeroTech.Infrastructure.Domain.Entities{
    public class Category{
        [Key]
        public Guid CategoryId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
