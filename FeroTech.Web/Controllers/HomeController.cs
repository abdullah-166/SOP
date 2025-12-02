using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FeroTech.Web.Models;
using FeroTech.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FeroTech.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["EmployeeCount"] = await _context.Employees.CountAsync();

            ViewData["LastEmployees"] = await _context.Employees
                .OrderByDescending(e => e.EmployeeId)
                .Take(10)
                .ToListAsync();

            ViewData["Categories"] = await _context.Categories
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetPendingAssets(Guid categoryId)
        {
            var pending = await _context.Assets
                .Where(a => a.CategoryId == categoryId && a.Quantity > 0)
                .Select(a => new
                {
                    brand = a.Brand,
                    modell = a.Modell,
                    quantity = a.Quantity
                })
                .ToListAsync();

            return Json(pending);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}