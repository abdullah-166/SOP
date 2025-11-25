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

        // ==========================================================
        // INDEX - ONE ROW PER EMPLOYEE, ALL ASSETS COMBINED
        // ==========================================================
        public async Task<IActionResult> Index()
        {
            var distributed = (await _distributedRepo.GetAllAsync()).ToList();
            var employees = (await _employeeRepo.GetAllAsync()).ToList();
            var assets = (await _assetRepo.GetAllAsync()).ToList();
            var categories = (await _categoryRepo.GetAllAsync()).ToList();

            var list = distributed
                .Select(d =>
                {
                    var emp = employees.FirstOrDefault(x => x.EmployeeId == d.EmployeeId);
                    var asset = assets.FirstOrDefault(x => x.AssetId == d.AssetId);
                    var category = categories.FirstOrDefault(x => x.CategoryId == d.CategoryId);

                    return new DistributedAssetListDto
                    {
                        DistributedAssetId = d.DistributedAssetId,
                        EmployeeId = d.EmployeeId,
                        EmployeeName = emp?.FullName ?? "",
                        EmployeePhone = emp?.Phone ?? "",
                        AssetName = $"{asset?.Brand} {asset?.Modell}",
                        CategoryName = category?.CategoryName ?? "",
                        AssignedDate = d.AssignedDate,
                        EndDate = d.EndDate
                    };
                })
                .OrderBy(x => x.EmployeeName)
                .ThenBy(x => x.AssignedDate)
                .ToList();

            return View(list);
        }

        // ==========================================================
        // CREATE (GET)
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.EmployeeList = await _employeeRepo.GetAllAsync();
            ViewBag.CategoryList = await _categoryRepo.GetAllAsync();
            return View(new DistributedAsset
            {
                AssignedDate = DateTime.Today
            });
        }

        // ==========================================================
        // CREATE (POST) - quantity--, basic validation
        // ==========================================================
        [HttpPost]
        public async Task<IActionResult> Create(DistributedAsset model)
        {
            if (!ModelState.IsValid ||
                model.EmployeeId == Guid.Empty ||
                model.CategoryId == Guid.Empty ||
                model.AssetId == Guid.Empty)
            {
                return Json(new { success = false, message = "Validation failed." });
            }

            // check asset exists
            var asset = await _assetRepo.GetByIdAsync(model.AssetId);
            if (asset == null)
                return Json(new { success = false, message = "Asset not found." });

            // stock check
            if (asset.Quantity <= 0)
                return Json(new { success = false, message = "No stock available for this asset." });

            // basic dupe check: same employee, same asset, no end date
            var existing = await _distributedRepo.GetAllAsync();
            bool alreadyAssigned = existing.Any(x =>
                x.EmployeeId == model.EmployeeId &&
                x.AssetId == model.AssetId &&
                x.EndDate == null);

            if (alreadyAssigned)
                return Json(new { success = false, message = "This asset is already assigned to this employee." });

            if (model.AssignedDate == default)
                model.AssignedDate = DateTime.Today;

            // deduct stock
            asset.Quantity -= 1;
            await _assetRepo.UpdateAsync(asset);

            // simple QR text (not used for PDF now, but required non-null)
            model.QRCodeValue = $"{model.AssetId}-{Guid.NewGuid()}";

            await _distributedRepo.AddAsync(model);

            return Json(new { success = true, message = "Asset assigned successfully!" });
        }

        // ==========================================================
        // EDIT (GET)
        // ==========================================================
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

        // ==========================================================
        // EDIT (POST) - also adjusts quantities if asset changed
        // ==========================================================
        [HttpPost]
        public async Task<IActionResult> Edit(
            Guid DistributedAssetId,
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

            // if asset changed, fix quantities
            if (oldAssetId != AssetId)
            {
                // return stock to old asset
                var oldAsset = await _assetRepo.GetByIdAsync(oldAssetId);
                if (oldAsset != null)
                {
                    oldAsset.Quantity += 1;
                    await _assetRepo.UpdateAsync(oldAsset);
                }

                // take stock from new asset
                var newAsset = await _assetRepo.GetByIdAsync(AssetId);
                if (newAsset == null)
                    return Json(new { success = false, message = "New asset not found." });

                if (newAsset.Quantity <= 0)
                    return Json(new { success = false, message = "Selected asset is out of stock." });

                newAsset.Quantity -= 1;
                await _assetRepo.UpdateAsync(newAsset);
            }

            return Json(new { success = true, message = "Updated successfully!" });
        }

        // ==========================================================
        // DELETE (POST) - quantity++
        // ==========================================================
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

        // ==========================================================
        // AJAX: LOAD ASSETS by CATEGORY (only active, in stock)
        // ==========================================================
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
    }
}
