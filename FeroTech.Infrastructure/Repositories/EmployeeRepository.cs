using FeroTech.Infrastructure.Application.DTOs;
using FeroTech.Infrastructure.Application.Interfaces;
using FeroTech.Infrastructure.Data;
using FeroTech.Infrastructure.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeroTech.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            var employees = await _context.Employees
                .Select(e => new EmployeeDto
                {
                    EmployeeId = e.EmployeeId,
                    FullName = e.FullName,
                    Email = e.Email,
                    Phone = e.Phone,
                    DepartmentId = e.DepartmentId,
                    DepartmentName = _context.Departments
                        .Where(d => d.DepartmentId == e.DepartmentId)
                        .Select(d => d.DepartmentName)
                        .FirstOrDefault() ?? "—",
                    JobTitle = e.JobTitle,
                    IsActive = e.IsActive
                })
                .ToListAsync();

            return employees;
        }


        public async Task<Employee?> GetByIdAsync(Guid id)
        {
            return await _context.Employees.FindAsync(id);
        }

        public async Task Create(EmployeeDto model)
        {
            var employee = new Employee
            {
                EmployeeId = Guid.NewGuid(),
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
                DepartmentId = model.DepartmentId,
                JobTitle = model.JobTitle,
                IsActive = model.IsActive
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Employee model)
        {
            _context.Employees.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
        }
    }
}
