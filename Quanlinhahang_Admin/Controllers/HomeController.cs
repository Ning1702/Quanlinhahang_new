using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Quanlinhahang_Admin.Models;

namespace Quanlinhahang_Admin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // ✅ Khi vào "/", tự động chuyển sang khu vực Admin
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Home", new { area = "Admin" });
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
