using FeroTech.Infrastructure.Application.DTOs;
using FeroTech.Infrastructure.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FeroTech.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _repo;

        public CategoryController(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _repo.GetAllAsync();
            return View(data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CategoryDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _repo.AddAsync(model);

            TempData["Message"] = "Category added successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return NotFound();

            var dto = new CategoryDto
            {
                CategoryId = entity.CategoryId,
                CategoryName = entity.CategoryName
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryDto model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid form submission" });

            await _repo.UpdateAsync(model);

            return Json(new { success = true, message = "Category updated successfully!" });
        }


        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _repo.DeleteAsync(id);
            TempData["Message"] = "Category deleted!";
            return RedirectToAction("Index");
        }
    }
}
