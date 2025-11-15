using FeroTech.Infrastructure.Application.DTOs;
using FeroTech.Infrastructure.Application.Interfaces;
using FeroTech.Infrastructure.Data;
using FeroTech.Infrastructure.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FeroTech.Web.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmployeeRepository _rep;
        private readonly INotificationRepository _notificationRepo;
        private readonly IDepartmentRepository _departmentRepo;

        public EmployeeController(
            ApplicationDbContext context,
            IEmployeeRepository rep,
            INotificationRepository notificationRepo,
            IDepartmentRepository departmentRepo)
        {
            _context = context;
            _rep = rep;
            _notificationRepo = notificationRepo;
            _departmentRepo = departmentRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _rep.GetAllAsync();
            return Json(employees);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var departments = await _departmentRepo.GetAllAsync();
            ViewBag.Departments = departments ?? new List<DepartmentDto>();
            return View(new EmployeeDto());
        }


        [HttpPost]
        public async Task<IActionResult> Create(EmployeeDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Departments = await _departmentRepo.GetAllAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = false, message = "Validation failed. Please check inputs." });

                return View(model);
            }

            await _rep.Create(model);

            await _notificationRepo.AddAsync(
                message: $"New employee '{model.FullName}' was created.",
                module: "Employee",
                actionType: "Create"
            );

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(new { success = true, message = "Employee created successfully!" });

            TempData["Message"] = "Employee created successfully!";
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var employee = await _rep.GetByIdAsync(id);
            if (employee == null)
                return NotFound();

            var model = new EmployeeDto
            {
                EmployeeId = employee.EmployeeId,
                FullName = employee.FullName,
                Email = employee.Email,
                Phone = employee.Phone,
                DepartmentId = employee.DepartmentId,
                JobTitle = employee.JobTitle,
                IsActive = employee.IsActive
            };

            ViewBag.Departments = await _departmentRepo.GetAllAsync();

            return View(model);   
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Employee model)
        {
            var employee = await _context.Employees.FindAsync(model.EmployeeId);
            if (employee == null)
                return Json(new { success = false, message = "Employee not found." });

            employee.FullName = model.FullName;
            employee.Email = model.Email;
            employee.Phone = model.Phone;
            employee.DepartmentId = model.DepartmentId;
            employee.JobTitle = model.JobTitle;
            employee.IsActive = model.IsActive;

            _context.Update(employee);
            await _context.SaveChangesAsync();

            await _notificationRepo.AddAsync(
                message: $"Employee '{employee.FullName}' was updated.",
                module: "Employee",
                actionType: "Update"
            );

            return Json(new { success = true, message = "Employee updated successfully!" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return Json(new { success = false, message = "Employee not found." });

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            await _notificationRepo.AddAsync(
                message: $"Employee '{employee.FullName}' was deleted.",
                module: "Employee",
                actionType: "Delete"
            );

            return Json(new { success = true, message = "Employee deleted successfully!" });
        }
    }
}
