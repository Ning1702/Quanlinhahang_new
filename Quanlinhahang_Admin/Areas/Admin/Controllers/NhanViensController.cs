using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quanlinhahang_Admin.Data;
using Quanlinhahang_Admin.Models;

namespace Quanlinhahang_Admin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class NhanViensController : Controller
    {
        private readonly QlnhContext _context;

        public NhanViensController(QlnhContext context)
        {
            _context = context;
        }

        // ===========================
        // DANH SÁCH + TÌM KIẾM
        // ===========================
        public IActionResult Index(string searchType, string keyword)
        {
            ViewBag.SearchType = searchType;
            ViewBag.Keyword = keyword;

            var query = _context.NhanViens
                .Include(x => x.TaiKhoan)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                if (searchType == "id" && int.TryParse(keyword, out int id))
                {
                    query = query.Where(x => x.NhanVienID == id);
                }
                else if (searchType == "phone")
                {
                    query = query.Where(x => x.SoDienThoai.Contains(keyword));
                }
            }

            return View(query.ToList());
        }

        // ===========================
        // EDIT GET
        // ===========================
        public IActionResult Edit(int id, string searchType, string keyword)
        {
            var nv = _context.NhanViens
                .Include(x => x.TaiKhoan)
                .FirstOrDefault(x => x.NhanVienID == id);

            if (nv == null) return NotFound();

            ViewBag.SearchType = searchType;
            ViewBag.Keyword = keyword;

            return View(nv);
        }

        // ===========================
        // EDIT POST
        // ===========================
        [HttpPost]
        public IActionResult Edit(
            NhanVien nv,
            string TenDangNhap,
            string MatKhau,
            string searchType,
            string keyword)
        {
            var existing = _context.NhanViens
                .Include(x => x.TaiKhoan)
                .FirstOrDefault(x => x.NhanVienID == nv.NhanVienID);

            if (existing == null)
                return NotFound();

            // VALIDATE SDT
            if (!IsValidPhone(nv.SoDienThoai))
                ModelState.AddModelError("SoDienThoai", "Số điện thoại không hợp lệ!");

            if (_context.NhanViens.Any(x =>
                    x.SoDienThoai == nv.SoDienThoai &&
                    x.NhanVienID != nv.NhanVienID))
                ModelState.AddModelError("SoDienThoai", "Số điện thoại đã tồn tại!");

            // VALIDATE TÊN ĐĂNG NHẬP
            if (_context.TaiKhoans.Any(x =>
                    x.TenDangNhap == TenDangNhap &&
                    x.TaiKhoanID != existing.TaiKhoanID))
            {
                ModelState.AddModelError("TaiKhoan.TenDangNhap", "Tên đăng nhập đã tồn tại!");
            }

            // Nếu lỗi → giữ lại input + trả view
            if (!ModelState.IsValid)
            {
                ViewBag.SearchType = searchType;
                ViewBag.Keyword = keyword;

                if (existing.TaiKhoan == null)
                    existing.TaiKhoan = new TaiKhoan();

                existing.TaiKhoan.TenDangNhap = TenDangNhap;
                existing.TaiKhoan.MatKhauHash = MatKhau;

                return View(existing);
            }

            // UPDATE NHÂN VIÊN
            existing.HoTen = nv.HoTen;
            existing.SoDienThoai = nv.SoDienThoai;
            existing.ChucVu = nv.ChucVu;
            existing.NgayVaoLam = nv.NgayVaoLam;
            existing.TrangThai = nv.TrangThai;

            // UPDATE TÀI KHOẢN
            if (existing.TaiKhoan == null)
            {
                TaiKhoan tk = new TaiKhoan
                {
                    TenDangNhap = TenDangNhap,
                    MatKhauHash = MatKhau,
                    VaiTro = "NhanVien"
                };

                _context.TaiKhoans.Add(tk);
                _context.SaveChanges();

                existing.TaiKhoanID = tk.TaiKhoanID;
                existing.TaiKhoan = tk;
            }
            else
            {
                existing.TaiKhoan.TenDangNhap = TenDangNhap;
                existing.TaiKhoan.MatKhauHash = MatKhau;
            }

            _context.SaveChanges();

            // QUAY LẠI ĐÚNG TRANG
            if (!string.IsNullOrEmpty(keyword))
                return RedirectToAction("Index", new { searchType, keyword });

            return RedirectToAction("Index");
        }


        // ===========================
        // DELETE
        // ===========================
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var nv = _context.NhanViens.FirstOrDefault(x => x.NhanVienID == id);
            if (nv == null) return NotFound();

            _context.NhanViens.Remove(nv);
            _context.SaveChanges();

            TempData["success"] = "Xóa nhân viên thành công!";
            return RedirectToAction("Index");
        }

        // ===========================
        // VALIDATE SDT
        // ===========================
        private bool IsValidPhone(string phone)
        {
            return phone != null &&
                   phone.Length == 10 &&
                   phone.StartsWith("0") &&
                   phone.All(char.IsDigit);
        }
    }
}
