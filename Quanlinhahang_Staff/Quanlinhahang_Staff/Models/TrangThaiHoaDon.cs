using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quanlinhahang_Staff.Models
{
    [Table("TrangThaiHoaDon")]
    public class TrangThaiHoaDon
    {
        [Key]
        public int TrangThaiID { get; set; }

        [Required]
        [MaxLength(50)]
        public string TenTrangThai { get; set; } = "";

        public List<HoaDon> HoaDons { get; set; } = new();
    }
}
