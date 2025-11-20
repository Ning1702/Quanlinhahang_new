using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Quanlinhahang_Staff.Controllers
{
    public class AuthController : Controller
    {
        // Project Staff KHÔNG có login riêng
        // Chỉ nhận userId từ Admin và set Session Staff

        [HttpGet]
        public IActionResult FromAdmin(int userId)
        {
            // Lưu session cho STAFF
            HttpContext.Session.SetInt32("UserId", userId);

            // Chuyển vào trang Staff (Invoices)
            return RedirectToAction("Index", "Invoices");
        }
    }
}
