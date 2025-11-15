using FeroTech.Infrastructure.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FeroTech.Web.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationRepository _repo;

        public NotificationController(INotificationRepository repo)
        {
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            var notifications = await _repo.GetAllAsync();
            return View(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _repo.MarkAsReadAsync(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _repo.MarkAllAsReadAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<JsonResult> GetUnreadCount()
        {
            var count = await _repo.GetUnreadCountAsync();
            return Json(new { count });
        }
    }
}
