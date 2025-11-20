using System;
using System.Collections.Generic;

namespace Quanlinhahang_Admin.Models;

public partial class KhachHang
{
    public int KhachHangID { get; set; }

    public string HoTen { get; set; } = null!;

    public string? Email { get; set; }

    public string SoDienThoai { get; set; } = null!;

    public string? DiaChi { get; set; }

    public int DiemTichLuy { get; set; }

    public int? HangThanhVienID { get; set; }

    public DateTime NgayTao { get; set; }

    public string TrangThai { get; set; } = null!;

    public virtual ICollection<DatBan> DatBans { get; set; } = new List<DatBan>();

    public virtual HangThanhVien? HangThanhVien { get; set; }
}
