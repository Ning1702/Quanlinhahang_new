using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quanlinhahang_Admin.Models
{
    [Table("MonAn")]
    public class MonAn
    {
        [Key]
        public int MonAnID { get; set; }

        [Required(ErrorMessage = "Tên món không được để trống")]
        [Display(Name = "Tên món ăn")]
        public string TenMon { get; set; } = string.Empty;

        [Display(Name = "Mô tả")]
        public string? MoTa { get; set; }

        // ✅ Thuộc tính chính lưu trong DB
        [Display(Name = "Đơn giá (VNĐ)")]
        public int DonGia { get; set; }

        // ✅ Thuộc tính phụ cho phép nhập số lẻ (không ánh xạ DB)
        [NotMapped]
        [Display(Name = "Giá nhập (có thể số lẻ)")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải là số hợp lệ")]
        public decimal DonGiaInput
        {
            get => DonGia;
            set => DonGia = (int)Math.Round(value); // làm tròn
            // 👉 nếu muốn cắt phần lẻ thì thay bằng: DonGia = (int)value;
        }

        [Display(Name = "Loại món")]
        public string? LoaiMon { get; set; }

        [Display(Name = "Ảnh món ăn")]
        public string? HinhAnhURL { get; set; }

        [Display(Name = "Trạng thái")]
        public string TrangThai { get; set; } = "Còn bán";

        [ForeignKey("DanhMuc")]
        [Display(Name = "Danh mục")]
        public int DanhMucID { get; set; }

        public virtual DanhMucMon? DanhMuc { get; set; }

        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
    }
}
