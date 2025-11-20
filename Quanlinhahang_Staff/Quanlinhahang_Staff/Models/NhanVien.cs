using System.ComponentModel.DataAnnotations.Schema;

namespace Quanlinhahang_Staff.Models
{
    [Table("NhanVien")]
    public class NhanVien
    {
        public int NhanVienID { get; set; }
        public int? TaiKhoanID { get; set; }
        public string HoTen { get; set; } = null!;
        public string? SoDienThoai { get; set; }
        public string? ChucVu { get; set; }
        public DateTime? NgayVaoLam { get; set; }
        public string? TrangThai { get; set; }

        public TaiKhoan? TaiKhoan { get; set; }
    }
}
