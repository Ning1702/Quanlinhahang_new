# Quanlinhahang — Admin Enhancements (11/07/2025)

## ✅ Yêu cầu đã thực hiện
- **Admin/Nhân viên**: thêm/sửa/xóa (`Areas/Admin/NhanViensController`, Views đầy đủ).
- **Admin/Khách hàng**: chỉ **xóa** (giữ `Index + Delete`).
- **Admin/Menu (Món ăn)**: thêm/sửa/xóa, **upload ảnh** vào `wwwroot/uploads`, lưu đường dẫn vào `MonAn.HinhAnhURL`.
- **Admin/Hóa đơn**: thêm trường **VAT (%)** và **Loại dịch vụ** (Tại chỗ/Mang về/Giao hàng), trang **Edit** để cập nhật.
- **Doanh số**: trang `Admin/ThongKe/Index` đã có **biểu đồ doanh số 12 tháng** (Chart.js).
- **Biểu đồ tròn**: cập nhật thành **tỷ lệ khách hàng đã có tài khoản** (`ThongKeController.CustomerAccountPercent`).

## 🔌 Kết nối CSDL
Đã sửa `appsettings*.json`:
```json
"ConnectionStrings": {
  "QLNH": "Server=DESKTOP-I6O0201\\MSSQLLocalDB;Database=Quanlinhahang;Trusted_Connection=True;MultipleActiveResultSets=True"
}
```

> Nếu bạn đổi server khác, sửa lại chuỗi trên cho phù hợp.

## 🗄️ Cập nhật CSDL (bắt buộc trước khi chạy)
Vì thêm 2 cột mới cho bảng `HoaDon`, hãy chạy script:
- `SqlScripts/2025-11-07_add_hoadon_vat_loaidv.sql` trong SQL Server (đúng DB `Quanlinhahang`).

Hoặc bạn có thể tạo Migration tương đương nếu dùng EF Core migrations.

## 🔎 Các file chính
- Controllers:
  - `Areas/Admin/Controllers/NhanViensController.cs`
  - `Areas/Admin/Controllers/HoaDonsController.cs` (thêm Edit VAT/Loại DV)
  - `Areas/Admin/Controllers/ThongKeController.cs` (thêm `CustomerAccountPercent`)
- Views:
  - `Areas/Admin/Views/NhanViens/{Index,Create,Edit}.cshtml`
  - `Areas/Admin/Views/MonAns/{Index,Create,Edit}.cshtml`
  - `Areas/Admin/Views/HoaDons/{Index,Edit}.cshtml`
  - `Areas/Admin/Views/ThongKe/Index.cshtml` (doughnut = *khách có tài khoản*)
- Model/Mapping:
  - `Models/HoaDon.cs` (thêm `VAT`, `LoaiDichVu`)
  - `Data/QlnhContext.ModelExtensions.cs` (map cột)

## 📦 Ảnh món ăn
- Lưu tại: `wwwroot/uploads`
- Nếu thư mục chưa có, app sẽ tự tạo.

## 🚀 Chạy
1. **Chạy script SQL** thêm cột cho `HoaDon`.
2. Mở solution, `dotnet restore`/build.
3. Chạy dự án. Khu vực Admin: `/Admin` (mặc định `Home/Index`).

---

*Mọi thay đổi đều cô lập, không phá vỡ route mặc định. Nếu cần thêm phân quyền đăng nhập cho Admin, mình sẽ bổ sung theo yêu cầu.*