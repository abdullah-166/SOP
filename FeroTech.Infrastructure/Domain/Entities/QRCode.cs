using System;
using System.ComponentModel.DataAnnotations;

namespace FeroTech.Infrastructure.Domain.Entities
{
    public class QRCode
    {
        [Key]
        public Guid QRCodeId { get; set; }

        public Guid AssetId { get; set; }  

        [StringLength(128)]
        public string? QRCodeValue { get; set; }

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        public bool IsPrinted { get; set; } = false;

        public string? Notes { get; set; }
    }
}
