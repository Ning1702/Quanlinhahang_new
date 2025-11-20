using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quanlinhahang_Admin.Models
{
    public partial class HoaDon
    {
        public int HoaDonID { get; set; }

        public int DatBanID { get; set; }

        public int? TaiKhoanID { get; set; }

        public DateTime NgayLap { get; set; }

        public decimal TongTien { get; set; }

        public decimal GiamGia { get; set; }

        public int DiemCong { get; set; }

        public int DiemSuDung { get; set; }

        public string? HinhThucThanhToan { get; set; }

        // ⚙️ Cột thật trong DB
        public string TrangThai { get; set; } = "Chưa xác nhận";

        // ✅ Thuộc tính ảo, không lưu DB, chỉ để hiển thị
        [NotMapped]
        public string TrangThaiThanhToan
        {
            get
            {
                if (TrangThai.Contains("thanh toán", StringComparison.OrdinalIgnoreCase))
                    return "Đã thanh toán";
                return "Chưa thanh toán";
            }
            set { /* bỏ qua - không ghi xuống DB */ }
        }

        public decimal? VAT { get; set; }
        public string? LoaiDichVu { get; set; }

        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
        public virtual DatBan DatBan { get; set; } = null!;
        public virtual TaiKhoan? TaiKhoan { get; set; }
    }
}
