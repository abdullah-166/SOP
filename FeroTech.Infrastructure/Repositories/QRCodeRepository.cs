using FeroTech.Infrastructure.Application.DTOs;
using FeroTech.Infrastructure.Application.Interfaces;
using FeroTech.Infrastructure.Data;
using FeroTech.Infrastructure.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeroTech.Infrastructure.Repositories
{
    public class QRCodeRepository : IQRCodeRepository
    {
        private readonly ApplicationDbContext _context;
        public QRCodeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<QRCode>> GetAllAsync()
        {
            return await _context.QRCodes.ToListAsync();
        }

        public async Task<QRCode?> GetByIdAsync(int id)
        {
            return await _context.QRCodes.FindAsync(id);
        }

        public async Task Create(QRCodeDto model)
        {
            var assets = new QRCode
            {
                QRCodeId = Guid.NewGuid(),
                AssetId = model.AssetId,
                QRCodeValue = model.QRCodeValue,
                GeneratedAt = DateTime.UtcNow,
                IsPrinted = model.IsPrinted,
                Notes = model.Notes
            };
            _context.QRCodes.Add(assets);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(QRCode asset)
        {
            _context.QRCodes.Update(asset);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.DistributedAssets.FindAsync(id);
            if (product != null) _context.DistributedAssets.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}