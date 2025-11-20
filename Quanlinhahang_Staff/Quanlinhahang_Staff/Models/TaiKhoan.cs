using System.ComponentModel.DataAnnotations.Schema;

namespace Quanlinhahang_Staff.Models
{
    [Table("TaiKhoan")]
    public class TaiKhoan
    {
        public int TaiKhoanID { get; set; }
        public string TenDangNhap { get; set; } = null!;
        public string MatKhauHash { get; set; } = null!;
        public string? Email { get; set; }
        public string? VaiTro { get; set; } // Admin / NhanVien
        public string? TrangThai { get; set; }
    }
}
