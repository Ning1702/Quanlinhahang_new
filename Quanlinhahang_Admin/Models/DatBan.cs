using System;
using System.Collections.Generic;

namespace Quanlinhahang_Admin.Models;

public partial class DatBan
{
    public int DatBanID { get; set; }

    public int KhachHangID { get; set; }

    public int? BanPhongID { get; set; }

    public int KhungGioID { get; set; }

    public DateOnly NgayDen { get; set; }

    public int SoNguoi { get; set; }

    public decimal? TongTienDuKien { get; set; }

    public string? YeuCauDacBiet { get; set; }

    public string TrangThai { get; set; } = null!;

    public DateTime NgayTao { get; set; }

    public virtual BanPhong? BanPhong { get; set; }

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();

    public virtual KhachHang KhachHang { get; set; } = null!;

    public virtual KhungGio KhungGio { get; set; } = null!;
}
