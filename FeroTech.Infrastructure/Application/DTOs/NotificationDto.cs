using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeroTech.Infrastructure.Application.DTOs
{
    public class NotificationDto
    {
        public class Notification
        {
            [Key]
            public Guid NotificationId { get; set; }

            [Required]
            public string Message { get; set; } = string.Empty;

            public string? ActionType { get; set; } //  "Create", "Update", "Delete"

            public string? Module { get; set; } // "Employee", "Asset"

            public bool IsRead { get; set; } = false;

            public DateTime CreatedAt { get; set; } = DateTime.Now;
        }
    }
}