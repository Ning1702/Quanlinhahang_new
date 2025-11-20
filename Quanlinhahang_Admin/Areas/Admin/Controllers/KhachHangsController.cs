using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quanlinhahang_Admin.Data;
using Quanlinhahang_Admin.Models;
using System.Linq;

namespace Quanlinhahang_Admin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class KhachHangsController : Controller
    {
        private readonly QlnhContext _context;

        public KhachHangsController(QlnhContext context)
        {
            _context = context;
        }

        // ===============================
        // HIỂN THỊ DANH SÁCH KHÁCH HÀNG + TÌM KIẾM SĐT
        // ===============================
        public IActionResult Index(string? searchPhone)
        {
            var query = _context.KhachHangs
                .Include(k => k.HangThanhVien)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchPhone))
            {
                query = query.Where(k => k.SoDienThoai.Contains(searchPhone));
                ViewBag.SearchPhone = searchPhone;

                if (!query.Any())
                {
                    TempData["msg"] = "Không tìm thấy khách hàng có số điện thoại này!";
                }
            }

            var khachhangs = query.ToList();
            return View(khachhangs);
        }

        // ===============================
        // TẠO KHÁCH HÀNG MỚI
        // ===============================
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.HangThanhVienList = _context.HangThanhViens.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(KhachHang kh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.KhachHangs.Add(kh);
                    _context.SaveChanges();
                    TempData["msg"] = "✅ Thêm khách hàng thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["msg"] = "❌ Lỗi: " + ex.Message;
                }
            }
            ViewBag.HangThanhVienList = _context.HangThanhViens.ToList();
            return View(kh);
        }

        // ===============================
        // CHỈNH SỬA KHÁCH HÀNG
        // ===============================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var kh = _context.KhachHangs
                .Include(k => k.HangThanhVien)
                .FirstOrDefault(k => k.KhachHangID == id);

            if (kh == null)
                return NotFound();

            ViewBag.HangThanhVienList = _context.HangThanhViens.ToList();
            return View(kh);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, KhachHang kh)
        {
            if (id != kh.KhachHangID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(kh);
                    _context.SaveChanges();
                    TempData["msg"] = "✅ Cập nhật khách hàng thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["msg"] = "❌ Lỗi khi cập nhật: " + ex.Message;
                }
            }

            ViewBag.HangThanhVienList = _context.HangThanhViens.ToList();
            return View(kh);
        }

        // ===============================
        // XÓA KHÁCH HÀNG (AJAX)
        // ===============================
        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            var kh = _context.KhachHangs.Find(id);
            if (kh == null) return NotFound();

            try
            {
                _context.KhachHangs.Remove(kh);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Không thể xóa khách hàng: " + ex.Message);
            }
        }
    }
}
