-- Thêm cột VAT và Loại dịch vụ cho bảng HoaDon
ALTER TABLE dbo.HoaDon
    ADD VAT DECIMAL(5,2) NULL CONSTRAINT DF_HoaDon_VAT DEFAULT(0);

ALTER TABLE dbo.HoaDon
    ADD LoaiDichVu NVARCHAR(50) NULL;

