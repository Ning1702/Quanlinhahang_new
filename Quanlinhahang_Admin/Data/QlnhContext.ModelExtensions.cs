using Microsoft.EntityFrameworkCore;
using Quanlinhahang_Admin.Models;

namespace Quanlinhahang_Admin.Data
{
    public partial class QlnhContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HoaDon>(entity =>
            {
                entity.Property(e => e.VAT).HasColumnType("decimal(5,2)");
                entity.Property(e => e.LoaiDichVu).HasMaxLength(50);
            });
        }
    }
}