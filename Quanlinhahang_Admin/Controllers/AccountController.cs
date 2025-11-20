using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Quanlinhahang_Admin.Data;
using System.Linq;

namespace Quanlinhahang_Admin.Controllers
{
    public class AccountController : Controller
    {
        private readonly QlnhContext _context;

        public AccountController(QlnhContext context)
        {
            _context = context;
        }

        // ======================== LOGIN GET ========================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ======================== LOGIN POST ========================
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _context.TaiKhoans
                .FirstOrDefault(t => t.TenDangNhap == username && t.MatKhauHash == password);

            if (user == null)
            {
                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
                return View();
            }

            // Ghi Session UserId
            HttpContext.Session.SetInt32("UserId", user.TaiKhoanID);

            string role = (user.VaiTro ?? "").Trim();

            // Tạo claims để login bằng Cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.TenDangNhap),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                });

            // ================= PHÂN QUYỀN SAU ĐĂNG NHẬP =================

            if (role == "Admin")
            {
                return RedirectToAction("Index", "Home");
            }

            if (role == "Staff")
            {
                // ⚠️ Hãy thay đúng PORT của Staff project
                string staffUrl = $"https://localhost:7163/Auth/FromAdmin?userId={user.TaiKhoanID}";
                return Redirect(staffUrl);
            }

            // Role không hợp lệ
            return RedirectToAction("Login");
        }


        // ======================== LOGOUT ========================
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();

            return RedirectToAction("Login", "Account");
        }
    }
}
