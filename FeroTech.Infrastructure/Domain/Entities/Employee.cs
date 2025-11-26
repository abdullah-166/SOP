using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeroTech.Infrastructure.Domain.Entities
{
    public class Employee
    {
        [Key]
        public Guid EmployeeId { get; set; }

        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Phone { get; set; }

        public Guid? DepartmentId { get; set; }

        [StringLength(100)]
        public string? JobTitle { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
