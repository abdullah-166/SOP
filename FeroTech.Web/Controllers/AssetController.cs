using FeroTech.Infrastructure.Application.DTOs;
using FeroTech.Infrastructure.Application.Interfaces;
using FeroTech.Infrastructure.Data;
using FeroTech.Infrastructure.Domain.Entities;
using FeroTech.Web.Hubs; // <--- Import Hub Namespace
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR; // <--- Import SignalR
using Microsoft.EntityFrameworkCore;

namespace FeroTech.Web.Controllers
{
    [Authorize]
    public class AssetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAssetRepository _rep;
        private readonly INotificationRepository _notificationRepo;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IHubContext<NotificationHub> _hubContext; // <--- 1. Add Hub Context

        public AssetController(
            ApplicationDbContext context,
            IAssetRepository rep,
            INotificationRepository notificationRepo,
            ICategoryRepository categoryRepo,
            IHubContext<NotificationHub> hubContext) // <--- 2. Inject Hub Context
        {
            _context = context;
            _rep = rep;
            _notificationRepo = notificationRepo;
            _categoryRepo = categoryRepo;
            _hubContext = hubContext; // <--- 3. Assign Hub Context
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.CategoryList = await _categoryRepo.GetAllAsync();
            return View(new AssetDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(AssetDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.CategoryList = await _categoryRepo.GetAllAsync();
                return View(model);
            }

            await _rep.Create(model);

            // --- NOTIFICATION & SIGNALR ---
            string msg = $"New asset '{model.Brand} {model.Modell}' was created.";
            await _notificationRepo.AddAsync(msg, "Asset", "Create");

            // Broadcast to clients
            int count = await _notificationRepo.GetUnreadCountAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", msg, count);
            // -----------------------------

            TempData["SuccessMessage"] = "Asset created successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await _context.Assets.ToListAsync();
            return Json(new { data });
        }

        [HttpGet]
        public async Task<IActionResult> GetById(Guid id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null) return Json(new { success = false, message = "Asset not found." });
            return Json(new { success = true, data = asset });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Asset model)
        {
            var asset = await _context.Assets.FindAsync(model.AssetId);
            if (asset == null) return Json(new { success = false, message = "Asset not found." });

            asset.Category = model.Category;
            asset.Brand = model.Brand;
            asset.Modell = model.Modell;
            asset.PurchasePrice = model.PurchasePrice;
            asset.Quantity = model.Quantity;
            asset.IsActive = model.IsActive;

            _context.Update(asset);
            await _context.SaveChangesAsync();

            // --- NOTIFICATION & SIGNALR ---
            string msg = $"Asset '{asset.Brand} {asset.Modell}' was updated.";
            await _notificationRepo.AddAsync(msg, "Asset", "Update");

            int count = await _notificationRepo.GetUnreadCountAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", msg, count);
            // -----------------------------

            return Json(new { success = true, message = "Asset updated successfully!" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var asset = await _context.Assets.FindAsync(id);
            if (asset == null) return Json(new { success = false, message = "Asset not found." });

            string assetName = $"{asset.Brand} {asset.Modell}"; // Capture name before delete
            _context.Assets.Remove(asset);
            await _context.SaveChangesAsync();

            // --- NOTIFICATION & SIGNALR ---
            string msg = $"Asset '{assetName}' was deleted.";
            await _notificationRepo.AddAsync(msg, "Asset", "Delete");

            int count = await _notificationRepo.GetUnreadCountAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", msg, count);
            // -----------------------------

            return Json(new { success = true, message = "Asset deleted successfully!" });
        }
    }
}