using System;
using System.Collections.Generic;

namespace Quanlinhahang_Admin.Models;

public partial class TaiKhoan
{
    public int TaiKhoanID { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhauHash { get; set; } = null!;

    public string? Email { get; set; }

    public string VaiTro { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();
}
