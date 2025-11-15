using FeroTech.Infrastructure.Application.Interfaces;
using FeroTech.Infrastructure.Data;
using FeroTech.Infrastructure.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FeroTech.Infrastructure.Repositories
{
    public class DistributedAssetRepository : IDistributedAssetRepository
    {
        private readonly ApplicationDbContext _context;

        public DistributedAssetRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DistributedAsset>> GetAllAsync()
        {
            return await _context.DistributedAssets.ToListAsync();
        }

        public async Task<DistributedAsset?> GetByIdAsync(Guid id)
        {
            return await _context.DistributedAssets.FindAsync(id);
        }

        public async Task AddAsync(DistributedAsset entity)
        {
            await _context.DistributedAssets.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DistributedAsset entity)
        {
            _context.DistributedAssets.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var item = await _context.DistributedAssets.FindAsync(id);
            if (item != null)
            {
                _context.DistributedAssets.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
