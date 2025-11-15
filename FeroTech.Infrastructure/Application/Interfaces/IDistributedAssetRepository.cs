using FeroTech.Infrastructure.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeroTech.Infrastructure.Application.Interfaces
{
    public interface IDistributedAssetRepository
    {
        Task<IEnumerable<DistributedAsset>> GetAllAsync();
        Task<DistributedAsset?> GetByIdAsync(Guid id);
        Task AddAsync(DistributedAsset distributedAsset);
        Task UpdateAsync(DistributedAsset distributedAsset);
        Task DeleteAsync(Guid id);
    }
}
