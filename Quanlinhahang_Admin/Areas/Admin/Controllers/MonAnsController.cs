using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Quanlinhahang_Admin.Data;
using Quanlinhahang_Admin.Models;

namespace Quanlinhahang_Admin.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MonAnsController : Controller
    {
        private readonly QlnhContext _context;
        private readonly IWebHostEnvironment _env;

        public MonAnsController(QlnhContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ✅ GET: Admin/MonAns + lọc theo danh mục
        public async Task<IActionResult> Index(int? DanhMucID)
        {
            var query = _context.MonAns.Include(m => m.DanhMuc).AsQueryable();

            // Nếu có chọn danh mục → lọc theo danh mục
            if (DanhMucID.HasValue)
            {
                query = query.Where(m => m.DanhMucID == DanhMucID.Value);
                ViewBag.SelectedDanhMuc = DanhMucID.Value;
            }

            // Gửi danh sách danh mục xuống view
            ViewBag.DanhMucList = await _context.DanhMucMons.ToListAsync();

            return View(await query.ToListAsync());
        }

        private void LoadDropDowns(object? selectedDanhMucId = null)
        {
            ViewData["DanhMucID"] = new SelectList(
                _context.DanhMucMons,
                "DanhMucID",
                "TenDanhMuc",
                selectedDanhMucId
            );

            ViewBag.TrangThaiList = new List<SelectListItem>
            {
                new SelectListItem("Còn bán", "Còn bán"),
                new SelectListItem("Ngừng bán", "Ngừng bán")
            };
        }

        // GET: Admin/MonAns/Create
        public IActionResult Create()
        {
            LoadDropDowns();
            return View();
        }

        // POST: Admin/MonAns/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MonAn monAn, IFormFile? HinhAnh)
        {
            if (ModelState.IsValid)
            {
                if (HinhAnh != null && HinhAnh.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "images", "monan");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid() + Path.GetExtension(HinhAnh.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await HinhAnh.CopyToAsync(stream);
                    }

                    monAn.HinhAnhURL = "/images/monan/" + uniqueFileName;
                }

                _context.Add(monAn);
                await _context.SaveChangesAsync();
                TempData["msg"] = "Thêm món ăn thành công!";
                return RedirectToAction(nameof(Index));
            }

            LoadDropDowns(monAn.DanhMucID);
            return View(monAn);
        }

        // GET: Admin/MonAns/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var monAn = await _context.MonAns.FindAsync(id);
            if (monAn == null) return NotFound();

            LoadDropDowns(monAn.DanhMucID);
            return View(monAn);
        }

        // POST: Admin/MonAns/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MonAn monAn, IFormFile? HinhAnh)
        {
            if (id != monAn.MonAnID) return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _context.MonAns.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.MonAnID == id);
                if (existing == null) return NotFound();

                if (HinhAnh != null && HinhAnh.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "images", "monan");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid() + Path.GetExtension(HinhAnh.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await HinhAnh.CopyToAsync(stream);
                    }

                    // Xóa ảnh cũ
                    if (!string.IsNullOrEmpty(existing.HinhAnhURL))
                    {
                        string oldPath = Path.Combine(_env.WebRootPath, existing.HinhAnhURL.TrimStart('/'));
                        if (System.IO.File.Exists(oldPath))
                            System.IO.File.Delete(oldPath);
                    }

                    monAn.HinhAnhURL = "/images/monan/" + uniqueFileName;
                }
                else
                {
                    monAn.HinhAnhURL = existing.HinhAnhURL;
                }

                _context.Update(monAn);
                await _context.SaveChangesAsync();
                TempData["msg"] = "Cập nhật món ăn thành công!";
                return RedirectToAction(nameof(Index));
            }

            LoadDropDowns(monAn.DanhMucID);
            return View(monAn);
        }

        // POST: Admin/MonAns/Delete
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var monAn = await _context.MonAns.FindAsync(id);
            if (monAn == null)
            {
                TempData["msg"] = "Không tìm thấy món ăn!";
                return RedirectToAction(nameof(Index));
            }

            if (!string.IsNullOrEmpty(monAn.HinhAnhURL))
            {
                string oldPath = Path.Combine(_env.WebRootPath, monAn.HinhAnhURL.TrimStart('/'));
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);
            }

            _context.MonAns.Remove(monAn);
            await _context.SaveChangesAsync();
            TempData["msg"] = "Xóa món ăn thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
