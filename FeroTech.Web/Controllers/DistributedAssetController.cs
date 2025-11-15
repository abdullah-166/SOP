using FeroTech.Infrastructure.Application.DTOs;
using FeroTech.Infrastructure.Application.Interfaces;
using FeroTech.Infrastructure.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FeroTech.Web.Controllers
{
    [Authorize]
    public class DistributedAssetController : Controller
    {
        private readonly IDistributedAssetRepository _distributedRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IAssetRepository _assetRepo;
        private readonly ICategoryRepository _categoryRepo;

        public DistributedAssetController(
            IDistributedAssetRepository distributedRepo,
            IEmployeeRepository employeeRepo,
            IAssetRepository assetRepo,
            ICategoryRepository categoryRepo)
        {
            _distributedRepo = distributedRepo;
            _employeeRepo = employeeRepo;
            _assetRepo = assetRepo;
            _categoryRepo = categoryRepo;
        }

        public async Task<IActionResult> Index()
        {
            var list = new List<DistributedAssetListDto>();

            var distributed = await _distributedRepo.GetAllAsync();
            var employees = await _employeeRepo.GetAllAsync();
            var assets = await _assetRepo.GetAllAsync();
            var categories = await _categoryRepo.GetAllAsync();

            foreach (var item in distributed)
            {
                var emp = employees.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                var asset = assets.FirstOrDefault(x => x.AssetId == item.AssetId);
                var cat = categories.FirstOrDefault(x => x.CategoryId == item.CategoryId);

                list.Add(new DistributedAssetListDto
                {
                    DistributedAssetId = item.DistributedAssetId,
                    EmployeeName = emp?.FullName ?? "",
                    EmployeePhone = emp?.Phone ?? "",
                    AssetName = $"{asset?.Brand} {asset?.Modell}",
                    CategoryName = cat?.CategoryName ?? "",
                    AssignedDate = item.AssignedDate,
                    EndDate = item.EndDate
                });
            }

            return View(list);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.EmployeeList = await _employeeRepo.GetAllAsync();
            ViewBag.CategoryList = await _categoryRepo.GetAllAsync();
            return View(new DistributedAsset());
        }

        [HttpPost]
        public async Task<IActionResult> Create(DistributedAsset model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Validation failed." });

            var asset = await _assetRepo.GetByIdAsync(model.AssetId);
            if (asset == null)
                return Json(new { success = false, message = "Asset not found." });

            if (asset.Quantity <= 0)
                return Json(new { success = false, message = "No stock available for this asset." });

            asset.Quantity -= 1;
            await _assetRepo.UpdateAsync(asset);

            await _distributedRepo.AddAsync(model);

            return Json(new { success = true, message = "Asset assigned successfully!" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _distributedRepo.GetByIdAsync(id);
            if (item == null)
                return Json(new { success = false, message = "Record not found." });

            var asset = await _assetRepo.GetByIdAsync(item.AssetId);
            if (asset != null)
            {
                asset.Quantity += 1;
                await _assetRepo.UpdateAsync(asset);
            }

            await _distributedRepo.DeleteAsync(id);

            return Json(new { success = true, message = "Deleted successfully!" });
        }

        [HttpGet]
        public async Task<IActionResult> GetAssetsByCategory(Guid categoryId)
        {
            var assets = await _assetRepo.GetAllAsync();

            var filtered = assets
                .Where(x => x.CategoryId == categoryId && x.IsActive && x.Quantity > 0)
                .Select(x => new
                {
                    assetId = x.AssetId,
                    name = $"{x.Brand} {x.Modell}"
                })
                .ToList();

            return Json(filtered);
        }
        [HttpGet("DistributedAsset/Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var item = await _distributedRepo.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            ViewBag.EmployeeList = await _employeeRepo.GetAllAsync();
            ViewBag.CategoryList = await _categoryRepo.GetAllAsync();
            ViewBag.AssetList = (await _assetRepo.GetAllAsync())
                                .Where(a => a.CategoryId == item.CategoryId)
                                .ToList();

            return View(item);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Guid DistributedAssetId,
                                     Guid EmployeeId,
                                     Guid CategoryId,
                                     Guid AssetId,
                                     DateTime AssignedDate,
                                     DateTime? EndDate,
                                     string? Notes)
        {
            var existing = await _distributedRepo.GetByIdAsync(DistributedAssetId);
            if (existing == null)
                return Json(new { success = false, message = "Record not found." });
            var oldAssetId = existing.AssetId;
            existing.EmployeeId = EmployeeId;
            existing.CategoryId = CategoryId;
            existing.AssetId = AssetId;
            existing.AssignedDate = AssignedDate == DateTime.MinValue ? DateTime.Today : AssignedDate;
            existing.EndDate = EndDate;
            existing.Notes = Notes;

            await _distributedRepo.UpdateAsync(existing);

            var assetRepo = HttpContext.RequestServices.GetRequiredService<IAssetRepository>();

            if (oldAssetId != AssetId)
            {

                var oldAsset = await assetRepo.GetByIdAsync(oldAssetId);
                if (oldAsset != null)
                {
                    oldAsset.Quantity += 1;
                    await assetRepo.UpdateAsync(oldAsset);
                }

                var newAsset = await assetRepo.GetByIdAsync(AssetId);
                if (newAsset != null)
                {
                    if (newAsset.Quantity <= 0)
                    {
                        return Json(new { success = false, message = "Selected asset is out of stock." });
                    }

                    newAsset.Quantity -= 1;
                    await assetRepo.UpdateAsync(newAsset);
                }
            }

            return Json(new { success = true, message = "Updated successfully!" });
        }

    }
}
