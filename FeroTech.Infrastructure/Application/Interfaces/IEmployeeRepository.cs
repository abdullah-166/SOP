using FeroTech.Infrastructure.Application.DTOs;
using FeroTech.Infrastructure.Domain.Entities;

namespace FeroTech.Infrastructure.Application.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<EmployeeDto>> GetAllAsync();
        Task<Employee?> GetByIdAsync(Guid id);
        Task Create(EmployeeDto model);
        Task UpdateAsync(Employee model);
        Task DeleteAsync(Guid id);
    }
}
