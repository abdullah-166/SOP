using FeroTech.Infrastructure.Application.DTOs;
using FeroTech.Infrastructure.Application.Interfaces;
using FeroTech.Web.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FeroTech.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentRepository _repo;
        private readonly INotificationRepository _notificationRepo; // Added
        private readonly IHubContext<NotificationHub> _hubContext;  // Added

        public DepartmentController(
            IDepartmentRepository repo,
            INotificationRepository notificationRepo,
            IHubContext<NotificationHub> hubContext)
        {
            _repo = repo;
            _notificationRepo = notificationRepo;
            _hubContext = hubContext;
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
            if (!ModelState.IsValid) return BadRequest("Invalid data submitted.");

            await _repo.AddAsync(model);

            // --- SignalR ---
            string msg = $"New department '{model.DepartmentName}' created.";
            await _notificationRepo.AddAsync(msg, "Department", "Create");
            int count = await _notificationRepo.GetUnreadCountAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", msg, count);
            // ----------------

            return Json(new { success = true, message = "Department added successfully!" });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return NotFound();

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
            if (!ModelState.IsValid) return BadRequest("Invalid data submitted.");

            await _repo.UpdateAsync(model);

            // --- SignalR ---
            string msg = $"Department '{model.DepartmentName}' updated.";
            await _notificationRepo.AddAsync(msg, "Department", "Update");
            int count = await _notificationRepo.GetUnreadCountAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", msg, count);
            // ----------------

            return Json(new { success = true, message = "Department updated successfully!" });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return Json(new { success = false, message = "Department not found." });
            string name = existing.DepartmentName;

            await _repo.DeleteAsync(id);

            // --- SignalR ---
            string msg = $"Department '{name}' deleted.";
            await _notificationRepo.AddAsync(msg, "Department", "Delete");
            int count = await _notificationRepo.GetUnreadCountAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", msg, count);
            // ----------------

            return Json(new { success = true, message = "Department deleted successfully!" });
        }
    }
}