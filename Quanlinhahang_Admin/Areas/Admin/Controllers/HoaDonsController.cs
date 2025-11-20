using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Quanlinhahang_Admin.Data;
using Quanlinhahang_Admin.Models;

namespace Quanlinhahang_Admin.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HoaDonsController : Controller
    {
        private readonly QlnhContext _ctx;
        public HoaDonsController(QlnhContext ctx) => _ctx = ctx;

        public async Task<IActionResult> Index(string? status)
        {
            var list = await _ctx.HoaDons
                .Include(h => h.DatBan)
                .ThenInclude(db => db.KhachHang)
                .OrderByDescending(h => h.NgayLap)
                .ToListAsync();

            // Filter trạng thái thanh toán ảo
            if (!string.IsNullOrEmpty(status))
            {
                list = list
                    .Where(h => h.TrangThaiThanhToan.Equals(status, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ViewBag.Status = status;
            return View(list);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var hd = await _ctx.HoaDons.FindAsync(id);
            if (hd == null) return NotFound();
            return View(hd);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HoaDonID,VAT,LoaiDichVu")] HoaDon model)
        {
            if (id != model.HoaDonID) return BadRequest();
            var hd = await _ctx.HoaDons.FindAsync(id);
            if (hd == null) return NotFound();

            hd.VAT = model.VAT;
            hd.LoaiDichVu = model.LoaiDichVu;
            await _ctx.SaveChangesAsync();

            TempData["msg"] = "Cập nhật hóa đơn thành công!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkPaid(int id)
        {
            var hd = await _ctx.HoaDons.FindAsync(id);
            if (hd == null) return NotFound();

            // Đổi cột thật (TrangThai) thay vì cột ảo
            hd.TrangThai = "Đã thanh toán";
            await _ctx.SaveChangesAsync();

            TempData["msg"] = $"Hóa đơn #{id} đã được đánh dấu ĐÃ THANH TOÁN.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkUnpaid(int id)
        {
            var hd = await _ctx.HoaDons.FindAsync(id);
            if (hd == null) return NotFound();

            hd.TrangThai = "Chưa thanh toán";
            await _ctx.SaveChangesAsync();

            TempData["msg"] = $"Hóa đơn #{id} đã được đánh dấu CHƯA THANH TOÁN.";
            return RedirectToAction(nameof(Index));
        }
    }
}
