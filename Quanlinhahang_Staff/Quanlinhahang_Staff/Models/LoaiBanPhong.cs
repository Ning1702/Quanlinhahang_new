using System.ComponentModel.DataAnnotations.Schema;

namespace Quanlinhahang_Staff.Models
{
    [Table("LoaiBanPhong")]
    public class LoaiBanPhong
    {
        public int LoaiBanPhongID { get; set; }
        public string TenLoai { get; set; }
        public string? MoTa { get; set; }
        public decimal PhuThu { get; set; }

        public ICollection<BanPhong>? BanPhongs { get; set; }
    }
}
