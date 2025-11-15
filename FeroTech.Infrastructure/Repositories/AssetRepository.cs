using FeroTech.Infrastructure.Application.DTOs;
using FeroTech.Infrastructure.Application.Interfaces;
using FeroTech.Infrastructure.Data;
using FeroTech.Infrastructure.Domain.Entities;
using FeroTech.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace FeroTech.Infrastructure.Repositories
{
    public class AssetRepository : IAssetRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly QRCodeService _qrCodeService;

        public AssetRepository(ApplicationDbContext context)
        {
            _context = context;
            _qrCodeService = new QRCodeService(context);
        }

        public async Task<IEnumerable<Asset>> GetAllAsync()
        {
            return await _context.Assets.ToListAsync();
        }

        public async Task<Asset?> GetByIdAsync(Guid id)
        {
            return await _context.Assets.FindAsync(id);
        }

        public async Task Create(AssetDto model)
        {
            var category = await _context.Categories.FindAsync(model.CategoryId);

            var asset = new Asset
            {
                AssetId = Guid.NewGuid(),
                CategoryId = model.CategoryId,
                Category = category?.CategoryName, 
                Brand = model.Brand,
                Modell = model.Modell,
                PurchaseDate = model.PurchaseDate,
                PurchaseOrderNo = model.PurchaseOrderNo,
                Supplier = model.Supplier,
                PurchasePrice = model.PurchasePrice,
                WarrantyEndDate = model.WarrantyEndDate,
                Quantity = model.Quantity,
                Status = model.Status,
                Notes = model.Notes,
                IsActive = model.IsActive
            };

            _context.Assets.Add(asset);
            await _context.SaveChangesAsync();

            await _qrCodeService.GenerateAndSaveQRCodesAsync(asset);
        }

        public async Task UpdateAsync(Asset asset)
        {
            var existing = await _context.Assets.FindAsync(asset.AssetId);
            if (existing != null)
            {
                existing.CategoryId = asset.CategoryId;
                var category = await _context.Categories.FindAsync(asset.CategoryId);
                existing.Category = category?.CategoryName;

                existing.Brand = asset.Brand;
                existing.Modell = asset.Modell;
                existing.PurchasePrice = asset.PurchasePrice;
                existing.Quantity = asset.Quantity;
                existing.Status = asset.Status;
                existing.Notes = asset.Notes;
                existing.IsActive = asset.IsActive;

                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteAsync(Guid id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset != null)
            {
                _context.Assets.Remove(asset);
                await _context.SaveChangesAsync();
            }
        }
    }
}
