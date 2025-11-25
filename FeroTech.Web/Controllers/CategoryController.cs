using FeroTech.Infrastructure.Application.DTOs;
using FeroTech.Infrastructure.Application.Interfaces;
using FeroTech.Web.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FeroTech.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _repo;
        private readonly INotificationRepository _notificationRepo; // Added
        private readonly IHubContext<NotificationHub> _hubContext;  // Added

        public CategoryController(
            ICategoryRepository repo,
            INotificationRepository notificationRepo,
            IHubContext<NotificationHub> hubContext)
        {
            _repo = repo;
            _notificationRepo = notificationRepo;
            _hubContext = hubContext;
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
            if (!ModelState.IsValid) return View(model);

            await _repo.AddAsync(model);

            // --- SignalR ---
            string msg = $"New category '{model.CategoryName}' created.";
            await _notificationRepo.AddAsync(msg, "Category", "Create");
            int count = await _notificationRepo.GetUnreadCountAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", msg, count);
            // ----------------

            TempData["Message"] = "Category added successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return NotFound();

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
            if (!ModelState.IsValid) return Json(new { success = false, message = "Invalid form submission" });

            await _repo.UpdateAsync(model);

            // --- SignalR ---
            string msg = $"Category '{model.CategoryName}' was updated.";
            await _notificationRepo.AddAsync(msg, "Category", "Update");
            int count = await _notificationRepo.GetUnreadCountAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", msg, count);
            // ----------------

            return Json(new { success = true, message = "Category updated successfully!" });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            string name = entity?.CategoryName ?? "Unknown";

            await _repo.DeleteAsync(id);

            // --- SignalR ---
            string msg = $"Category '{name}' was deleted.";
            await _notificationRepo.AddAsync(msg, "Category", "Delete");
            int count = await _notificationRepo.GetUnreadCountAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", msg, count);
            // ----------------

            TempData["Message"] = "Category deleted!";
            return RedirectToAction("Index");
        }
    }
}