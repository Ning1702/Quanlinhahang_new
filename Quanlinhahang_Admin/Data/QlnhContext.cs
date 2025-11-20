using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Quanlinhahang_Admin.Models;

namespace Quanlinhahang_Admin.Data;

public partial class QlnhContext : DbContext
{
    public QlnhContext(DbContextOptions<QlnhContext> options)
        : base(options)
    {
    }

    public virtual DbSet<BanPhong> BanPhongs { get; set; }

    public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }

    public virtual DbSet<DanhMucMon> DanhMucMons { get; set; }

    public virtual DbSet<DatBan> DatBans { get; set; }

    public virtual DbSet<HangThanhVien> HangThanhViens { get; set; } = default!;

    public virtual DbSet<HoaDon> HoaDons { get; set; }

    public virtual DbSet<KhachHang> KhachHangs { get; set; }

    public virtual DbSet<KhungGio> KhungGios { get; set; }

    public virtual DbSet<LoaiBanPhong> LoaiBanPhongs { get; set; }

    public virtual DbSet<MonAn> MonAns { get; set; }

    public virtual DbSet<NhanVien> NhanViens { get; set; }

    public virtual DbSet<TaiKhoan> TaiKhoans { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BanPhong>(entity =>
        {
            entity.HasKey(e => e.BanPhongID).HasName("PK__BanPhong__B2D0E957D32D2C07");

            entity.ToTable("BanPhong");

            entity.Property(e => e.TenBanPhong).HasMaxLength(50);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("Trống");

            entity.HasOne(d => d.LoaiBanPhong).WithMany(p => p.BanPhongs)
                .HasForeignKey(d => d.LoaiBanPhongID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BanPhong_LoaiBanPhong");
        });

        modelBuilder.Entity<ChiTietHoaDon>(entity =>
        {
            entity.HasKey(e => new { e.HoaDonID, e.MonAnID });

            entity.ToTable("ChiTietHoaDon");

            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SoLuong).HasDefaultValue(1);
            entity.Property(e => e.ThanhTien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.HoaDon).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.HoaDonID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTHD_HoaDon");

            entity.HasOne(d => d.MonAn).WithMany(p => p.ChiTietHoaDons)
                .HasForeignKey(d => d.MonAnID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CTHD_MonAn");
        });

        modelBuilder.Entity<DanhMucMon>(entity =>
        {
            entity.HasKey(e => e.DanhMucID).HasName("PK__DanhMucM__1C53BA7BB2F5116B");

            entity.ToTable("DanhMucMon");

            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.TenDanhMuc).HasMaxLength(100);
        });

        modelBuilder.Entity<DatBan>(entity =>
        {
            entity.HasKey(e => e.DatBanID).HasName("PK__DatBan__6A75F719E0C5F0EE");

            entity.ToTable("DatBan");

            entity.Property(e => e.NgayTao).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.TongTienDuKien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("Chờ xác nhận");

            entity.HasOne(d => d.BanPhong).WithMany(p => p.DatBans)
                .HasForeignKey(d => d.BanPhongID)
                .HasConstraintName("FK_DatBan_BanPhong");

            entity.HasOne(d => d.KhachHang).WithMany(p => p.DatBans)
                .HasForeignKey(d => d.KhachHangID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DatBan_KhachHang");

            entity.HasOne(d => d.KhungGio).WithMany(p => p.DatBans)
                .HasForeignKey(d => d.KhungGioID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DatBan_KhungGio");
        });

        modelBuilder.Entity<HangThanhVien>(entity =>
        {
            entity.HasKey(e => e.HangThanhVienID).HasName("PK__HangThan__16F81D7A2375E340");

            entity.ToTable("HangThanhVien");

            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.TenHang).HasMaxLength(50);
        });

        // ✅ SỬA LẠI PHẦN HÓA ĐƠN CHUẨN EF
        modelBuilder.Entity<HoaDon>(entity =>
        {
            entity.HasKey(e => e.HoaDonID).HasName("PK__HoaDon__6956CE69650B65D2");

            entity.ToTable("HoaDon");

            entity.Property(e => e.GiamGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.HinhThucThanhToan).HasMaxLength(50);
            entity.Property(e => e.NgayLap).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.TongTien).HasColumnType("decimal(18, 2)");

            // ⚙️ Chuyển về dùng cột thật 'TrangThai' thay vì 'TrangThaiThanhToan'
            entity.Property(e => e.TrangThai)
                .HasMaxLength(30)
                .HasDefaultValue("Chưa thanh toán");

            // ⚙️ Bỏ qua property ảo để EF không truy vấn cột không tồn tại
            entity.Ignore(e => e.TrangThaiThanhToan);

            entity.HasOne(d => d.DatBan).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.DatBanID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HoaDon_DatBan");

            entity.HasOne(d => d.TaiKhoan).WithMany(p => p.HoaDons)
                .HasForeignKey(d => d.TaiKhoanID)
                .HasConstraintName("FK_HoaDon_TaiKhoan");
        });

        modelBuilder.Entity<KhachHang>(entity =>
        {
            entity.HasKey(e => e.KhachHangID).HasName("PK__KhachHan__880F211B702D204B");

            entity.ToTable("KhachHang");

            entity.Property(e => e.DiaChi).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.NgayTao).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("Hoạt động");

            entity.HasOne(d => d.HangThanhVien).WithMany(p => p.KhachHangs)
                .HasForeignKey(d => d.HangThanhVienID)
                .HasConstraintName("FK_KhachHang_HangThanhVien");
        });

        modelBuilder.Entity<KhungGio>(entity =>
        {
            entity.HasKey(e => e.KhungGioID).HasName("PK__KhungGio__CC9AB36A01916E99");

            entity.ToTable("KhungGio");

            entity.Property(e => e.TenKhungGio).HasMaxLength(50);
        });

        modelBuilder.Entity<LoaiBanPhong>(entity =>
        {
            entity.HasKey(e => e.LoaiBanPhongID).HasName("PK__LoaiBanP__BA742BBF5987BDBA");

            entity.ToTable("LoaiBanPhong");

            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.PhuThu).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TenLoai).HasMaxLength(100);
        });

        modelBuilder.Entity<MonAn>(entity =>
        {
            entity.HasKey(e => e.MonAnID).HasName("PK__MonAn__272259EF896ED27D");

            entity.ToTable("MonAn");

            entity.Property(e => e.DonGia).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.HinhAnhURL).HasMaxLength(255);
            entity.Property(e => e.LoaiMon).HasMaxLength(50);
            entity.Property(e => e.TenMon).HasMaxLength(150);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("Còn bán");

            entity.HasOne(d => d.DanhMuc).WithMany(p => p.MonAns)
                .HasForeignKey(d => d.DanhMucID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MonAn_DanhMucMon");
        });

        modelBuilder.Entity<NhanVien>(entity =>
        {
            entity.HasKey(e => e.NhanVienID).HasName("PK__NhanVien__E27FD7EA763952EE");

            entity.ToTable("NhanVien");

            entity.Property(e => e.ChucVu).HasMaxLength(50);
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("Đang làm");

            entity.HasOne(d => d.TaiKhoan).WithMany(p => p.NhanViens)
                .HasForeignKey(d => d.TaiKhoanID)
                .HasConstraintName("FK_NhanVien_TaiKhoan");
        });

        modelBuilder.Entity<TaiKhoan>(entity =>
        {
            entity.HasKey(e => e.TaiKhoanID).HasName("PK__TaiKhoan__9A124B6561480AFC");

            entity.ToTable("TaiKhoan");

            entity.HasIndex(e => e.TenDangNhap, "UQ__TaiKhoan__55F68FC0AD08CD7E").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.MatKhauHash).HasMaxLength(255);
            entity.Property(e => e.TenDangNhap).HasMaxLength(50);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(20)
                .HasDefaultValue("Hoạt động");
            entity.Property(e => e.VaiTro).HasMaxLength(20);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
