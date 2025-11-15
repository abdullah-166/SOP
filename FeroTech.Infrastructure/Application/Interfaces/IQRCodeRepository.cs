using FeroTech.Infrastructure.Application.DTOs;
using FeroTech.Infrastructure.Domain.Entities;
using FeroTech.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeroTech.Infrastructure.Application.Interfaces
{
    public interface IQRCodeRepository
    {
        Task<IEnumerable<QRCode>> GetAllAsync();
        Task<QRCode?> GetByIdAsync(int id);
        Task Create(QRCodeDto model);
        Task UpdateAsync(QRCode asset);
        Task DeleteAsync(int id);
    }
}