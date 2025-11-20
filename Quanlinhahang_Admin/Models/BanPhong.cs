using System;
using System.Collections.Generic;

namespace Quanlinhahang_Admin.Models;

public partial class BanPhong
{
    public int BanPhongID { get; set; }

    public int LoaiBanPhongID { get; set; }

    public string TenBanPhong { get; set; } = null!;

    public int SucChua { get; set; }

    public string TrangThai { get; set; } = null!;

    public virtual ICollection<DatBan> DatBans { get; set; } = new List<DatBan>();

    public virtual LoaiBanPhong LoaiBanPhong { get; set; } = null!;
}
