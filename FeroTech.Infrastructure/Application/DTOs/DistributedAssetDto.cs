using FeroTech.Infrastructure.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace FeroTech.Infrastructure.Application.DTOs
{
    public class DistributedAssetListDto
    {
        public Guid DistributedAssetId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        [Required]
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required]
        public Guid AssetId { get; set; }
        public Asset? Asset { get; set; }

        [Required]
        public DateTime AssignedDate { get; set; }

        public DateTime? EndDate { get; set; }

        [StringLength(255)]
        public string? Notes { get; set; }
        public string EmployeeName { get; set; } = "";
        public string EmployeePhone { get; set; } = "";
        public string AssetName { get; set; } = "";
        public string CategoryName { get; set; } = "";
        public string? QRCodePath { get; set; }
    }
}
