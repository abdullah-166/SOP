using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FeroTech.Web.Models;
using FeroTech.Infrastructure.Data; // <-- ADDED
using System.Linq;
using Microsoft.AspNetCore.Authorization; // <-- ADDED

namespace FeroTech.Web.Controllers;
[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context; // <-- ADDED

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context) // <-- UPDATED
    {
        _logger = logger;
        _context = context; // <-- ADDED
    }

    public IActionResult Index()
    {
        // --- ADDED THIS LOGIC ---
        // Get the count from the database
        int employeeCount = _context.Employees.Count();

        // Pass the count to the view
        ViewData["EmployeeCount"] = employeeCount;
        // --- END ---
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}