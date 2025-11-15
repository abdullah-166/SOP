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
    public class AssetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAssetRepository _rep;
        private readonly INotificationRepository _notificationRepo;
        private readonly ICategoryRepository _categoryRepo;

        public AssetController(
            ApplicationDbContext context,
            IAssetRepository rep,
            INotificationRepository notificationRepo,
            ICategoryRepository categoryRepo)
        {
            _context = context;
            _rep = rep;
            _notificationRepo = notificationRepo;
            _categoryRepo = categoryRepo;
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

            await _notificationRepo.AddAsync(
                message: $"New asset '{model.Brand} {model.Modell}' was created.",
                module: "Asset",
                actionType: "Create"
            );

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

            if (asset == null)
                return Json(new { success = false, message = "Asset not found." });

            return Json(new { success = true, data = asset });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Asset model)
        {
            var asset = await _context.Assets.FindAsync(model.AssetId);

            if (asset == null)
                return Json(new { success = false, message = "Asset not found." });

            asset.Category = model.Category;
            asset.Brand = model.Brand;
            asset.Modell = model.Modell;
            asset.PurchasePrice = model.PurchasePrice;
            asset.Quantity = model.Quantity;
            asset.IsActive = model.IsActive;

            _context.Update(asset);
            await _context.SaveChangesAsync();

            await _notificationRepo.AddAsync(
                message: $"Asset '{asset.Brand} {asset.Modell}' was updated.",
                module: "Asset",
                actionType: "Update"
            );

            return Json(new { success = true, message = "Asset updated successfully!" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var asset = await _context.Assets.FindAsync(id);

            if (asset == null)
                return Json(new { success = false, message = "Asset not found." });

            _context.Assets.Remove(asset);
            await _context.SaveChangesAsync();

            await _notificationRepo.AddAsync(
                message: $"Asset '{asset.Brand} {asset.Modell}' was deleted.",
                module: "Asset",
                actionType: "Delete"
            );

            return Json(new { success = true, message = "Asset deleted successfully!" });
        }
    }
}
