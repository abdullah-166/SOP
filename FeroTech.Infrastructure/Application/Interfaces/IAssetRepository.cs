using FeroTech.Infrastructure.Application.DTOs;
using FeroTech.Infrastructure.Domain.Entities;

namespace FeroTech.Infrastructure.Application.Interfaces
{
    public interface IAssetRepository
    {
        Task<IEnumerable<Asset>> GetAllAsync();
        Task<Asset?> GetByIdAsync(Guid id);   
        Task Create(AssetDto model);
        Task UpdateAsync(Asset asset);
        Task DeleteAsync(Guid id);            
    }
}
