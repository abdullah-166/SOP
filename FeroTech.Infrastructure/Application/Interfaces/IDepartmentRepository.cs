using FeroTech.Infrastructure.Application.DTOs;
using FeroTech.Infrastructure.Domain.Entities;

public interface IDepartmentRepository
{
    Task<IEnumerable<DepartmentDto>> GetAllAsync();
    Task<Department?> GetByIdAsync(Guid id);
    Task AddAsync(DepartmentDto dto);
    Task UpdateAsync(DepartmentDto dto);
    Task DeleteAsync(Guid id);
}
