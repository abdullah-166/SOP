using FeroTech.Infrastructure.Application.DTOs;
using FeroTech.Infrastructure.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FeroTech.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentRepository _repo;

        public DepartmentController(IDepartmentRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var data = await _repo.GetAllAsync();
            return View(data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(DepartmentDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data submitted.");

            await _repo.AddAsync(model);
            return Json(new { success = true, message = "Department added successfully!" });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return NotFound();

            var dto = new DepartmentDto
            {
                DepartmentId = entity.DepartmentId,
                DepartmentName = entity.DepartmentName,
                Description = entity.Description,
                IsActive = entity.IsActive
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(DepartmentDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data submitted.");

            await _repo.UpdateAsync(model);
            return Json(new { success = true, message = "Department updated successfully!" });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return Json(new { success = false, message = "Department not found." });

            await _repo.DeleteAsync(id);
            return Json(new { success = true, message = "Department deleted successfully!" });
        }
    }
}
