using FeroTech.Infrastructure.Application.Interfaces;
using FeroTech.Web.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FeroTech.Web.Controllers
{
    public class NotificationController : Controller
    {
        private readonly INotificationRepository _repo;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationController(INotificationRepository repo, IHubContext<NotificationHub> hubContext)
        {
            _repo = repo;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> Index()
        {
            var notifications = await _repo.GetAllAsync();
            return View(notifications);
        }

        // THIS IS THE METHOD USED BY THE BUTTON
        [HttpPost]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            // 1. Mark specific notification as read in DB
            await _repo.MarkAsReadAsync(id);

            // 2. Get updated count
            int count = await _repo.GetUnreadCountAsync();

            // 3. Update the red badge for all users immediately via SignalR
            await _hubContext.Clients.All.SendAsync("UpdateCount", count);

            // 4. Reload the page
            return RedirectToAction("Index");
        }

        // ... (Keep GetUnreadCount and MarkAllAsRead if you want) ...
        [HttpGet]
        public async Task<JsonResult> GetUnreadCount()
        {
            var count = await _repo.GetUnreadCountAsync();
            return Json(new { count });
        }
    }
}