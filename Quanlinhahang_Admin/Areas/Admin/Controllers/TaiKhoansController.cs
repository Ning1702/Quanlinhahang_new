using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Quanlinhahang_Admin.Data;
using Quanlinhahang_Admin.Models;

namespace Quanlinhahang_Admin.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TaiKhoansController : Controller
    {
        private readonly QlnhContext _ctx;
        public TaiKhoansController(QlnhContext ctx) => _ctx = ctx;

        public async Task<IActionResult> Index()
        {
            var data = await _ctx.TaiKhoans.Where(t => t.VaiTro == "Staff").ToListAsync();
            return View(data);
        }

        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TaiKhoan model)
        {
            if (!ModelState.IsValid) return View(model);
            model.VaiTro = "Staff";
            _ctx.Add(model);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var tk = await _ctx.TaiKhoans.FindAsync(id);
            if (tk == null) return NotFound();
            return View(tk);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TaiKhoan model)
        {
            if (id != model.TaiKhoanID) return NotFound();
            var tk = await _ctx.TaiKhoans.FindAsync(id);
            if (tk == null) return NotFound();

            tk.TenDangNhap = model.TenDangNhap;
            tk.Email = model.Email;
            tk.MatKhauHash = model.MatKhauHash;
            tk.TrangThai = model.TrangThai;
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var tk = await _ctx.TaiKhoans.FindAsync(id);
            if (tk == null) return NotFound();
            _ctx.TaiKhoans.Remove(tk);
            await _ctx.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
