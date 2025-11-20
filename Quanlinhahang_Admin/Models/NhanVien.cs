using System;
using System.Collections.Generic;

namespace Quanlinhahang_Admin.Models;

public partial class NhanVien
{
    public int NhanVienID { get; set; }

    public int? TaiKhoanID { get; set; }

    public string HoTen { get; set; } = null!;

    public string? SoDienThoai { get; set; }

    public string? ChucVu { get; set; }

    public DateOnly? NgayVaoLam { get; set; }

    public string TrangThai { get; set; } = null!;

    public virtual TaiKhoan? TaiKhoan { get; set; }
}
