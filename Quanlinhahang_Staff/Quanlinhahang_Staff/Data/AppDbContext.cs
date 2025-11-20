using Microsoft.EntityFrameworkCore;
using Quanlinhahang_Staff.Models;

namespace Quanlinhahang_Staff.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TaiKhoan> TaiKhoans { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<DatBan> DatBans { get; set; }
        public DbSet<BanPhong> BanPhongs { get; set; }
        public DbSet<LoaiBanPhong> LoaiBanPhongs { get; set; }
        public DbSet<KhungGio> KhungGios { get; set; }
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<TrangThaiHoaDon> TrangThaiHoaDons { get; set; }
        public DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        public DbSet<MonAn> MonAns { get; set; }
        public DbSet<NhanVien> NhanViens { get; set; }


        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            // CHI TIẾT HÓA ĐƠN
            mb.Entity<ChiTietHoaDon>()
              .HasKey(ct => new { ct.HoaDonID, ct.MonAnID });

            mb.Entity<ChiTietHoaDon>()
              .HasOne(ct => ct.HoaDon)
              .WithMany(h => h.ChiTietHoaDons)
              .HasForeignKey(ct => ct.HoaDonID)
              .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<ChiTietHoaDon>()
              .HasOne(ct => ct.MonAn)
              .WithMany()
              .HasForeignKey(ct => ct.MonAnID)
              .OnDelete(DeleteBehavior.Restrict);

            // HÓA ĐƠN – ĐẶT BÀN
            mb.Entity<HoaDon>()
              .HasOne(h => h.DatBan)
              .WithMany(db => db.HoaDons)
              .HasForeignKey(h => h.DatBanID)
              .OnDelete(DeleteBehavior.Restrict);

            // HÓA ĐƠN – BÀN PHÒNG
            mb.Entity<HoaDon>()
              .HasOne(h => h.BanPhong)
              .WithMany(bp => bp.HoaDons)
              .HasForeignKey(h => h.BanPhongID)
              .OnDelete(DeleteBehavior.Restrict);

            // HÓA ĐƠN – TRẠNG THÁI
            mb.Entity<HoaDon>()
              .HasOne(h => h.TrangThai)
              .WithMany(t => t.HoaDons)
              .HasForeignKey(h => h.TrangThaiID)
              .OnDelete(DeleteBehavior.Restrict);

            // ĐẶT BÀN – KHÁCH HÀNG
            mb.Entity<DatBan>()
              .HasOne(db => db.KhachHang)
              .WithMany()
              .HasForeignKey(db => db.KhachHangID)
              .OnDelete(DeleteBehavior.Restrict);

            // ĐẶT BÀN – BÀN PHÒNG
            mb.Entity<DatBan>()
              .HasOne(db => db.BanPhong)
              .WithMany(bp => bp.DatBans)
              .HasForeignKey(db => db.BanPhongID)
              .OnDelete(DeleteBehavior.Restrict);

            // BÀN PHÒNG – LOẠI BÀN PHÒNG
            mb.Entity<BanPhong>()
                .HasOne(bp => bp.LoaiBanPhong)
                .WithMany(lp => lp.BanPhongs)
                .HasForeignKey(bp => bp.LoaiBanPhongID)
                .OnDelete(DeleteBehavior.Restrict);

            // ĐẶT BÀN – KHUNG GIỜ
            mb.Entity<DatBan>()
              .HasOne(db => db.KhungGio)
              .WithMany()
              .HasForeignKey(db => db.KhungGioID)
              .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
