using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quanlinhahang_Staff.Data;
using Quanlinhahang_Staff.Models;
using Quanlinhahang_Staff.Models.ViewModels;
using System.Text.Json;

namespace Quanlinhahang_Staff.Controllers
{
    public class InvoicesController : BaseController
    {
        private readonly AppDbContext _db;

        public InvoicesController(AppDbContext db)
        {
            _db = db;
        }

        // ==============================
        // LIST HÓA ĐƠN
        // ==============================
        public async Task<IActionResult> Index([FromQuery] InvoiceFilterVM f, [FromQuery] int status = 0)
        {
            ViewBag.Status = status;
            ViewBag.Filter = f;

            var query = _db.HoaDons
                .Include(h => h.TrangThai)
                .Include(h => h.DatBan).ThenInclude(db => db.KhachHang)
                .Include(h => h.BanPhong).ThenInclude(bp => bp.LoaiBanPhong)
                .Include(h => h.ChiTietHoaDons)
                .AsQueryable();

            // Filter
            if (status > 0)
                query = query.Where(h => h.TrangThaiID == status);

            if (!string.IsNullOrWhiteSpace(f.Search))
            {
                string s = f.Search.Trim().ToLower();
                query = query.Where(h =>
                    (h.DatBan.KhachHang.HoTen.ToLower().Contains(s)) ||
                    (h.DatBan.KhachHang.SoDienThoai ?? "").Contains(s));
            }

            if (f.From.HasValue)
                query = query.Where(h => h.NgayLap.Date >= f.From.Value.Date);

            if (f.To.HasValue)
                query = query.Where(h => h.NgayLap.Date <= f.To.Value.Date);

            // SELECT → ViewModel
            var projectedData = query.Select(h => new
            {
                HoaDon = h,
                SubTotal = h.ChiTietHoaDons.Sum(ct => ct.ThanhTien)
            })
            .Select(x => new InvoiceRowVM
            {
                HoaDonID = x.HoaDon.HoaDonID,
                NgayLap = x.HoaDon.NgayLap,
                KhachHang = x.HoaDon.DatBan.KhachHang.HoTen,
                SoDienThoai = x.HoaDon.DatBan.KhachHang.SoDienThoai,

                BanPhong = x.HoaDon.BanPhong != null
                    ? x.HoaDon.BanPhong.TenBanPhong
                    : "",

                LoaiBanPhong = (x.HoaDon.BanPhong != null && x.HoaDon.BanPhong.LoaiBanPhong != null)
                    ? x.HoaDon.BanPhong.LoaiBanPhong.TenLoai
                    : "",

                ThanhTien =
                    (x.SubTotal * (1 + (x.HoaDon.VAT.HasValue ? x.HoaDon.VAT.Value : 0.10m)))
                    - x.HoaDon.GiamGia
                    - x.HoaDon.DiemSuDung,

                TrangThaiID = x.HoaDon.TrangThaiID,
                TrangThaiTen = x.HoaDon.TrangThai.TenTrangThai
            });


            var list = await projectedData
                .OrderByDescending(x => x.NgayLap)
                .Take(500)
                .ToListAsync();

            return View(list);
        }

        // ==============================
        // CHUYỂN SANG ĐANG PHỤC VỤ
        // ==============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartServing(int id, int status)
        {
            var hd = await _db.HoaDons
                .Include(h => h.TrangThai)
                .FirstOrDefaultAsync(h => h.HoaDonID == id);

            if (hd == null) return NotFound();

            if (hd.TrangThaiID == 2)
            {
                hd.TrangThaiID = 3;
                await _db.SaveChangesAsync();
                TempData["msg"] = "✅ Đã chuyển sang trạng thái phục vụ.";
            }
            else TempData["msg"] = "⚠️ Không thể phục vụ hóa đơn này.";

            return RedirectToAction(nameof(Index), new { status });
        }

        // ==============================
        // XÁC NHẬN HÓA ĐƠN
        // ==============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmInvoice(int id, int status)
        {
            var hd = await _db.HoaDons
                .Include(h => h.TrangThai)
                .FirstOrDefaultAsync(h => h.HoaDonID == id);

            if (hd == null) return NotFound();

            if (hd.TrangThaiID == 1)
            {
                hd.TrangThaiID = 2;
                await _db.SaveChangesAsync();
                TempData["msg"] = "✅ Hóa đơn đã được xác nhận.";
            }
            else TempData["msg"] = "⚠️ Không thể xác nhận hóa đơn này.";

            return RedirectToAction(nameof(Index), new { status });
        }

        // ==============================
        // HỦY HÓA ĐƠN
        // ==============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> HuyHoaDon(int id, int status)
        {
            var hd = await _db.HoaDons
                .Include(h => h.TrangThai)
                .FirstOrDefaultAsync(h => h.HoaDonID == id);

            if (hd == null) return NotFound();

            if (hd.TrangThaiID != 4)
            {
                hd.TrangThaiID = 5;
                await _db.SaveChangesAsync();
                TempData["msg"] = "🗑 Hóa đơn đã bị hủy.";
            }
            else TempData["msg"] = "⚠️ Không thể hủy hóa đơn đã thanh toán.";

            return RedirectToAction(nameof(Index), new { status });
        }
        // ==============================
        // THANH TOÁN HÓA ĐƠN
        // ==============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThanhToan(int id, int status)
        {
            var hd = await _db.HoaDons
                .Include(h => h.ChiTietHoaDons)
                .Include(h => h.TrangThai)
                .FirstOrDefaultAsync(h => h.HoaDonID == id);

            if (hd == null) return NotFound();

            if (hd.TrangThaiID == 3)
            {
                await UpdateTongTienAsync(hd);
                hd.TrangThaiID = 4;
                await _db.SaveChangesAsync();
                TempData["msg"] = "💰 Đã thanh toán.";
            }
            else TempData["msg"] = "⚠️ Chỉ thanh toán hóa đơn đang phục vụ.";

            return RedirectToAction(nameof(Index), new { status });
        }

        // ==============================
        // TẠO HÓA ĐƠN
        // ==============================
        public async Task<IActionResult> Create(int? datBanId)
        {
            if (datBanId == null) return BadRequest("Thiếu DatBanID");

            var datBan = await _db.DatBans.FindAsync(datBanId);
            if (datBan == null) return NotFound();

            var hd = new HoaDon
            {
                DatBanID = datBan.DatBanID,
                NgayLap = DateTime.Now,
                TongTien = 0,
                TrangThaiID = 1
            };

            _db.HoaDons.Add(hd);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Edit), new { id = hd.HoaDonID });
        }

        // ==============================
        // EDIT (KHÔNG LƯU DB MÓN ĂN)
        // ==============================
        public async Task<IActionResult> Edit(int id, int status = 0)
        {
            ViewBag.Status = status;

            var hd = await _db.HoaDons
                .Include(h => h.ChiTietHoaDons).ThenInclude(ct => ct.MonAn)
                .Include(h => h.DatBan)
                .Include(h => h.TrangThai)
                .FirstOrDefaultAsync(h => h.HoaDonID == id);

            if (hd == null) return NotFound();

            var vm = new InvoiceEditVM
            {
                HoaDonID = hd.HoaDonID,
                DatBanID = hd.DatBanID,
                BanPhongID = hd.BanPhongID,
                GiamGia = hd.GiamGia,
                DiemSuDung = hd.DiemSuDung,
                HinhThucThanhToan = hd.HinhThucThanhToan,
                TrangThai = hd.TrangThai.TenTrangThai,

                Items = hd.ChiTietHoaDons.Select(ct => new InvoiceEditVM.ItemLine
                {
                    MonAnID = ct.MonAnID,
                    TenMon = ct.MonAn.TenMon,
                    SoLuong = ct.SoLuong,
                    DonGia = ct.DonGia
                }).ToList()
            };

            ViewBag.MonAn = await _db.MonAns.Where(m => m.TrangThai == "Còn bán").ToListAsync();
            ViewBag.BanPhongs = await _db.BanPhongs.ToListAsync();
            ViewBag.TrangThai = hd.TrangThai.TenTrangThai;
            ViewBag.DaThanhToan = (hd.TrangThaiID == 4);

            return View(vm);
        }

        
        // =======================================
        // LẤY DANH SÁCH BÀN PHÒNG
        // =======================================
        [HttpGet]
        public async Task<IActionResult> GetBanPhong()
        {
            var data = await _db.BanPhongs
                .Select(b => new
                {
                    banPhongID = b.BanPhongID,
                    tenBanPhong = b.TenBanPhong,
                    soChoNgoi = b.SucChua,   // tên đúng của database
                    trangThai = b.TrangThai       // "Trống", "Đã đặt", "Đang phục vụ"
                })
                .ToListAsync();

            return Json(data);
        }



        // ==============================
        // SAVE (NHẬN MÓN TỪ CLIENT — ItemsJson)
        // ==============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(InvoiceEditVM vm, int status, string ItemsJson)
        {
            var hd = await _db.HoaDons
                .Include(h => h.ChiTietHoaDons)
                .FirstOrDefaultAsync(h => h.HoaDonID == vm.HoaDonID);

            if (hd == null) return NotFound();

            // Cập nhật thông tin chung
            hd.BanPhongID = vm.BanPhongID;
            hd.GiamGia = vm.GiamGia;
            hd.DiemSuDung = vm.DiemSuDung;
            hd.HinhThucThanhToan = vm.HinhThucThanhToan;

            // Parse JSON món ăn
            if (!string.IsNullOrEmpty(ItemsJson))
            {
                var items = JsonSerializer.Deserialize<List<InvoiceEditVM.ItemLine>>(ItemsJson);

                // Xóa cũ
                _db.ChiTietHoaDons.RemoveRange(hd.ChiTietHoaDons);

                // Thêm mới
                foreach (var item in items)
                {
                    _db.ChiTietHoaDons.Add(new ChiTietHoaDon
                    {
                        HoaDonID = hd.HoaDonID,
                        MonAnID = item.MonAnID,
                        SoLuong = item.SoLuong,
                        DonGia = item.DonGia,
                        ThanhTien = item.SoLuong * item.DonGia
                    });
                }
            }

            // Cập nhật tổng tiền
            await UpdateTongTienAsync(hd);
            await _db.SaveChangesAsync();

            TempData["msg"] = "Đã lưu hóa đơn.";

            return RedirectToAction(nameof(Edit), new { id = vm.HoaDonID, status });
        }



        // ==============================
        // UPDATE TỔNG TIỀN
        // ==============================
        private Task UpdateTongTienAsync(HoaDon hd)
        {
            var sub = hd.ChiTietHoaDons.Sum(x => x.ThanhTien);
            var vat = sub * 0.1m;
            var final = sub + vat - hd.GiamGia - hd.DiemSuDung;

            if (final < 0) final = 0;

            hd.TongTien = final;
            return Task.CompletedTask;
        }

        // ==============================
        // DETAILS
        // ==============================
        public async Task<IActionResult> Details(int id, int status = 0)
        {
            var hd = await _db.HoaDons
                .Include(h => h.DatBan).ThenInclude(db => db.BanPhong).ThenInclude(bp => bp.LoaiBanPhong)
                .Include(h => h.ChiTietHoaDons).ThenInclude(ct => ct.MonAn)
                .Include(h => h.TrangThai)
                .FirstOrDefaultAsync(h => h.HoaDonID == id);

            if (hd == null) return NotFound();

            ViewBag.Status = status;

            return View(hd);
        }

        // ==============================
        // PRINT
        // ==============================
        public async Task<IActionResult> Print(int id)
        {
            var hd = await _db.HoaDons
                .Include(h => h.DatBan)
                .Include(h => h.ChiTietHoaDons).ThenInclude(ct => ct.MonAn)
                .Include(h => h.TrangThai)
                .FirstOrDefaultAsync(h => h.HoaDonID == id);

            if (hd == null) return NotFound();

            return View(hd);
        }

        // ==============================
        // DELETE
        // ==============================
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var hd = await _db.HoaDons
                .Include(h => h.DatBan)
                .Include(h => h.TrangThai)
                .FirstOrDefaultAsync(h => h.HoaDonID == id);

            if (hd == null) return NotFound();

            return View(hd);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hd = await _db.HoaDons.FindAsync(id);
            if (hd != null)
            {
                _db.HoaDons.Remove(hd);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // ==============================
        // GO MANAGE — quay lại tab đúng + highlight dòng
        // ==============================
        public IActionResult GoManage(int id)
        {
            var hd = _db.HoaDons.FirstOrDefault(x => x.HoaDonID == id);
            if (hd == null) return NotFound();

            return RedirectToAction("Index", new
            {
                status = hd.TrangThaiID,
                highlight = id
            });
        }
    }
}
