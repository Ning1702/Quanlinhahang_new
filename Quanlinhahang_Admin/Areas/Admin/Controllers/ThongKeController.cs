using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Quanlinhahang_Admin.Data;

namespace Quanlinhahang_Admin.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ThongKeController : Controller
    {
        private readonly QlnhContext _ctx;
        public ThongKeController(QlnhContext ctx) => _ctx = ctx;

        public IActionResult Index() => View();

        // ============= 1) Doanh thu 12 tháng gần nhất =============
        // Trả về [{ label: "MM/yyyy", value: number }, ...]
        [HttpGet]
        public async Task<IActionResult> RevenueLast12Months()
        {
            var today = DateTime.Today;
            var start = new DateTime(today.Year, today.Month, 1).AddMonths(-11);

            var raw = await _ctx.HoaDons
                .Where(h => h.NgayLap >= start)
                .GroupBy(h => new { h.NgayLap.Year, h.NgayLap.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Total = g.Sum(x => x.TongTien) })
                .ToListAsync();

            var result = Enumerable.Range(0, 12)
                .Select(i =>
                {
                    var d = start.AddMonths(i);
                    var hit = raw.FirstOrDefault(r => r.Year == d.Year && r.Month == d.Month);
                    return new
                    {
                        label = d.ToString("MM/yyyy"),
                        value = hit?.Total ?? 0m
                    };
                })
                .ToList();

            return Json(result);
        }

        // ============= 2) Doanh thu theo quý trong năm hiện tại =============
        // Trả về { year, q1, q2, q3, q4, yearTotal }
        [HttpGet]
        public async Task<IActionResult> RevenueQuarterAndYear(int? year)
        {
            int y = year ?? DateTime.Today.Year;

            var raw = await _ctx.HoaDons
                .Where(h => h.NgayLap.Year == y)
                .Select(h => new { h.NgayLap.Month, h.TongTien })
                .ToListAsync();

            decimal q1 = raw.Where(x => x.Month >= 1 && x.Month <= 3).Sum(x => x.TongTien);
            decimal q2 = raw.Where(x => x.Month >= 4 && x.Month <= 6).Sum(x => x.TongTien);
            decimal q3 = raw.Where(x => x.Month >= 7 && x.Month <= 9).Sum(x => x.TongTien);
            decimal q4 = raw.Where(x => x.Month >= 10 && x.Month <= 12).Sum(x => x.TongTien);
            decimal yearTotal = q1 + q2 + q3 + q4;

            return Json(new { year = y, q1, q2, q3, q4, yearTotal });
        }

        // ============= 3) Tỷ lệ KH có tài khoản (join theo Email) =============
        [HttpGet]
        public async Task<IActionResult> CustomerAccountPercent()
        {
            var total = await _ctx.KhachHangs.CountAsync();

            // Quy ước nhanh: có tài khoản nếu Email trùng với Email của bảng TaiKhoan
            var haveAccount = await _ctx.KhachHangs
                .Where(k => k.Email != null && _ctx.TaiKhoans.Any(t => t.Email == k.Email))
                .Select(k => k.KhachHangID)
                .Distinct()
                .CountAsync();

            var percent = total == 0 ? 0 : (haveAccount * 100.0 / total);
            return Json(new { totalKH = total, coTaiKhoan = haveAccount, percent = Math.Round(percent, 2) });
        }
    }
}
