using System;
using System.ComponentModel.DataAnnotations;

namespace FeroTech.Infrastructure.Domain.Entities
{
    public class QRCodeDto
    {
        public Guid QRCodeId { get; set; }

        public Guid AssetId { get; set; }  // logical link

        [StringLength(128)]
        public string? QRCodeValue { get; set; }

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        public bool IsPrinted { get; set; } = false;

        public string? Notes { get; set; }
    }
}
