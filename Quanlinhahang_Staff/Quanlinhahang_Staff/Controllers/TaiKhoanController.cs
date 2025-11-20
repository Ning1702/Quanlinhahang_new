using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Quanlinhahang_Staff.Data;
using Quanlinhahang_Staff.Models;
using Quanlinhahang_Staff.Models.ViewModels;

namespace Quanlinhahang_Staff.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public TaiKhoanController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return Redirect("https://localhost:5001/Account/Login");

            var info = (
                from nv in _context.NhanViens
                join tk in _context.TaiKhoans on nv.TaiKhoanID equals tk.TaiKhoanID
                where tk.TaiKhoanID == userId
                select new TaiKhoanStaffVM
                {
                    NhanVienID = nv.NhanVienID,
                    HoTen = nv.HoTen,
                    SoDienThoai = nv.SoDienThoai,
                    ChucVu = nv.ChucVu,
                    NgayVaoLam = nv.NgayVaoLam,
                    TrangThaiNV = nv.TrangThai,

                    TaiKhoanID = tk.TaiKhoanID,
                    TenDangNhap = tk.TenDangNhap,
                    Email = tk.Email,
                    VaiTro = tk.VaiTro,
                    TrangThaiTK = tk.TrangThai,
                    MatKhauHash = tk.MatKhauHash
                }
            ).FirstOrDefault();

            if (info == null)
                return NotFound();

            return View(info);
        }

        [HttpPost]
        public IActionResult CapNhat(TaiKhoanStaffVM model)
        {
            var nv = _context.NhanViens.FirstOrDefault(x => x.NhanVienID == model.NhanVienID);
            if (nv != null)
            {
                nv.HoTen = model.HoTen;
                nv.SoDienThoai = model.SoDienThoai;
                nv.ChucVu = model.ChucVu;
            }

            var tk = _context.TaiKhoans.FirstOrDefault(x => x.TaiKhoanID == model.TaiKhoanID);
            if (tk != null)
            {
                tk.Email = model.Email;
                tk.TenDangNhap = model.TenDangNhap;

                // cập nhật mật khẩu
                if (!string.IsNullOrEmpty(model.MatKhauHash))
                {
                    tk.MatKhauHash = model.MatKhauHash;
                }
            }

            _context.SaveChanges();

            TempData["update"] = true;
            return RedirectToAction("Index");
        }
    }
}
