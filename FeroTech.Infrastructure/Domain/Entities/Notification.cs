using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeroTech.Infrastructure.Domain.Entities
{
    
    
        public class Notification
        {
            [Key]
            public Guid NotificationId { get; set; }

            [Required]
            public string Message { get; set; } = string.Empty;

            public string? ActionType { get; set; } // e.g., "Create", "Update", "Delete"

            public string? Module { get; set; } // e.g., "Employee", "Asset"

            public bool IsRead { get; set; } = false;

            public DateTime CreatedAt { get; set; } = DateTime.Now;
        }
    }
