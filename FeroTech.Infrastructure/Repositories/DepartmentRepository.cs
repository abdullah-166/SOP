using FeroTech.Infrastructure.Application.DTOs;
using FeroTech.Infrastructure.Application.Interfaces;
using FeroTech.Infrastructure.Data;
using FeroTech.Infrastructure.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FeroTech.Infrastructure.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var result = await _context.Departments
                .Select(d => new DepartmentDto
                {
                    DepartmentId = d.DepartmentId,
                    DepartmentName = d.DepartmentName,
                    Description = d.Description,
                    IsActive = d.IsActive
                })
                .ToListAsync();

            return result;
        }

        public async Task<Department?> GetByIdAsync(Guid id)
        {
            return await _context.Departments.FindAsync(id);
        }

        public async Task AddAsync(DepartmentDto dto)
        {
            var entity = new Department
            {
                DepartmentId = Guid.NewGuid(),
                DepartmentName = dto.DepartmentName,
                Description = dto.Description,
                IsActive = dto.IsActive
            };

            _context.Departments.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DepartmentDto dto)
        {
            var entity = await _context.Departments.FindAsync(dto.DepartmentId);
            if (entity == null) return;

            entity.DepartmentName = dto.DepartmentName;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;

            _context.Departments.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Departments.FindAsync(id);
            if (entity == null) return;

            _context.Departments.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
