using FeroTech.Infrastructure.Application.DTOs;
using FeroTech.Infrastructure.Application.Interfaces;
using FeroTech.Infrastructure.Data;
using FeroTech.Infrastructure.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FeroTech.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    IsActive = c.IsActive
                }).ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(Guid id)
            => await _context.Categories.FindAsync(id);

        public async Task AddAsync(CategoryDto model)
        {
            var entity = new Category
            {
                CategoryId = Guid.NewGuid(),
                CategoryName = model.CategoryName,
                IsActive = model.IsActive
            };

            _context.Categories.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CategoryDto model)
        {
            var entity = await _context.Categories.FindAsync(model.CategoryId);
            if (entity == null) return;

            entity.CategoryName = model.CategoryName;
            entity.IsActive = model.IsActive;

            _context.Categories.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _context.Categories.FindAsync(id);
            if (entity == null) return;

            _context.Categories.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
