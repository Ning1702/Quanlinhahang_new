using System;
using System.Collections.Generic;

namespace Quanlinhahang_Admin.Models;

public partial class KhungGio
{
    public int KhungGioID { get; set; }

    public string TenKhungGio { get; set; } = null!;

    public TimeOnly GioBatDau { get; set; }

    public TimeOnly GioKetThuc { get; set; }

    public virtual ICollection<DatBan> DatBans { get; set; } = new List<DatBan>();
}
