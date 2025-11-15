using FeroTech.Infrastructure.Application.DTOs;
using FeroTech.Infrastructure.Domain.Entities;

namespace FeroTech.Infrastructure.Application.Interfaces{
    public interface ICategoryRepository{
        Task<IEnumerable<CategoryDto>> GetAllAsync();
        Task<Category?> GetByIdAsync(Guid id);
        Task AddAsync(CategoryDto model);
        Task UpdateAsync(CategoryDto model);
        Task DeleteAsync(Guid id);
    }
}
