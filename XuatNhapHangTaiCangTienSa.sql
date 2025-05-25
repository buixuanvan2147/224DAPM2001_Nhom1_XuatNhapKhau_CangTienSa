USE master;
ALTER DATABASE XuatNhapHangTaiCangTienSa SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO
DROP DATABASE XuatNhapHangTaiCangTienSa;

CREATE DATABASE  XuatNhapHangTaiCangTienSa
GO

USE XuatNhapHangTaiCangTienSa
GO

CREATE TABLE khachHang (
    maKhachHang CHAR(10) NOT NULL PRIMARY KEY, -- Khóa chính
    tenDangNhap VARCHAR(100) NOT NULL, -- UNIQUE
    matKhau VARCHAR(50) NOT NULL,
    tenKhachHang NVARCHAR(100) NOT NULL,
    tenCongTy NVARCHAR(255) NULL,
    maSoThueCongTy CHAR(14) NULL,
    ngayDangKy DATE NOT NULL DEFAULT GETDATE(),
    cccd CHAR(12) NULL, -- UNIQUE
    diaChiLienLac NVARCHAR(100) NOT NULL,
    sdtKhachHang CHAR(10) NOT NULL, -- UNIQUE
    email NVARCHAR(50) NOT NULL, -- UNIQUE
	trangThaiTaiKhoanKhachHang NVarChar(100) NOT NULL,--Chờ duyệt, Đang khóa, Đã duyệt
	anhDaiDienKhachHangUrl VARCHAR(255) NULL
);
GO

--2
CREATE TABLE vaiTroNhanVien (
	maVaiTroNhanVien CHAR(10) NOT NULL PRIMARY KEY, -- Khóa chính
	tenLoaiNhanVien NVARCHAR(100) NOT NULL,-- Quản trị viên, Nhân viên nhập kho, Nhân viên xuất kho, Nhân viên kho bãi, Nhân viên hải quan, Nhân viên kế toán
	moTa NVARCHAR(255) NULL,
	);
GO

--3
CREATE TABLE nhanVien (
	maNhanVien CHAR(10) NOT NULL PRIMARY KEY, -- Khóa chính
	maLoaiNhanVien CHAR(10) NOT NULL FOREIGN KEY REFERENCES vaiTroNhanVien(maVaiTroNhanVien),
	tenDangNhap VARCHAR(50) NOT NULL, -- UNIQUE
	matKhau VARCHAR(50) NOT NULL,
	tenHienThi NVARCHAR(100) NULL,
	sdtNhanVien VARCHAR(11) NULL, -- UNIQUE
	diaChi NVARCHAR(100) NULL, -- UNIQUE
	email NVARCHAR(50) NOT NULL, -- UNIQUE
	trangThaiTaiKhoanNhanVien NVARCHAR(100) NOT NULL,--Mở, Khóa
	thoiGianDangNhapGanNhat DATETIME NOT NULL,
	anhDaiDienNhanVienUrl VARCHAR(255) NULL
);
GO

--4
CREATE TABLE donHang (
	maDonHang CHAR(10) NOT NULL PRIMARY KEY, -- Khóa chính
	maKhachHang CHAR(10) NOT NULL FOREIGN KEY REFERENCES khachHang(maKhachHang),
	maNhanVien CHAR(10) NOT NULL FOREIGN KEY REFERENCES nhanVien(maNhanVien),
	tenNguoiGui NVARCHAR(100) NOT NULL,
	tenNguoiNhan NVARCHAR(100) NOT NULL,
	cangDichDen NVARCHAR(255) NOT NULL,
	ngayTaoDonHang DATETIME NOT NULL,
	thoiGianLuuTru INT NOT NULL,
	ngayXuatCang DATETIME NULL,
	ngayNhapCang DATETIME NULL,
	trangThaiDonHang NVARCHAR(100) NOT NULL,
	trangThaiThanhToan NVARCHAR(100) NOT NULL,
	tongTien FLOAT NULL,
	moTa NVARCHAR(255) NULL
);
GO

--5
CREATE TABLE danhMucHangHoa (
	maDanhMucHangHoa CHAR(10) NOT NULL PRIMARY KEY, -- Khóa chính
	tenDanhMucHangHoa NVARCHAR(100) NOT NULL, -- UNIQUE
	moTa NVARCHAR(255) NULL
);
GO

--6
CREATE TABLE hangHoa (
	maHangHoa CHAR(10) NOT NULL PRIMARY KEY, -- Khóa chính
	maDanhMucHangHoa CHAR(10) NOT NULL FOREIGN KEY REFERENCES danhMucHangHoa(maDanhMucHangHoa),
	tenHangHoa NVARCHAR(500) NOT NULL,
	chiPhiLuuKhoToiThieu FLOAT NOT NULL,
	donViTinh NVARCHAR(50) NOT NULL,
	moTa NVARCHAR(255) NULL
);
GO

--7
CREATE TABLE chiTietDonHang (
	maChiTietDonHang CHAR(10) NOT NULL PRIMARY KEY, -- Khóa chính
	maDonHang CHAR(10) NOT NULL FOREIGN KEY REFERENCES donHang(maDonHang),
	maHangHoa CHAR(10) NOT NULL FOREIGN KEY REFERENCES hangHoa(maHangHoa),
	soLuong FLOAT NOT NULL,
	donViTinh NVARCHAR(50) NOT NULL,
	chatLuong NVARCHAR(255) NULL,
	donGia FLOAT NOT NULL,
	tienLuuKho FLOAT NULL,
	moTa NVARCHAR(500) NULL
);
GO

--8
CREATE TABLE phieuXuat (
	maPhieuXuat CHAR(10) NOT NULL PRIMARY KEY, -- Khóa chính
	maDonHang CHAR(10) NOT NULL FOREIGN KEY REFERENCES donHang(maDonHang),
	trangThaiXuatHang NVARCHAR(100) NOT NULL,
	ngayXuatKho DATETIME NOT NULL,
	moTa NVARCHAR(255) NULL
);
GO

--9
CREATE TABLE loaiKho (
	maLoaiKho CHAR(10) NOT NULL PRIMARY KEY, -- Khóa chính
	tenLoaiKho NVARCHAR(100) NOT NULL, -- UNIQUE
	soLuongKho INT NULL,
	moTa NVARCHAR(255) NULL
);
GO

--10
CREATE TABLE kho (
	maKho CHAR(10) NOT NULL PRIMARY KEY, -- Khóa chính
	maLoaiKho CHAR(10) NOT NULL FOREIGN KEY REFERENCES loaiKho(maLoaiKho),
	tenKho NVARCHAR(100) NOT NULL,
	diaChiKho NVARCHAR(100) NOT NULL,
	trangThaiKho NVARCHAR(100) NOT NULL,
	sucChuaToiDa FLOAT NOT NULL,
	tonKho FLOAT NOT NULL,
	moTa NVARCHAR(255) NULL
);
GO

--11
CREATE TABLE chiTietKho (
	maChiTietKho CHAR(10) NOT NULL PRIMARY KEY, -- Khóa chính
	maKho CHAR(10) NOT NULL FOREIGN KEY REFERENCES kho(maKho),
	sucChuaToiDa FLOAT NOT NULL,
	tonKho FLOAT NOT NULL,
	trangThaiChiTietKho NVARCHAR(100) NOT NULL,
	ngayCapNhatCuoi DATETIME NOT NULL,
	moTa NVARCHAR(255) NULL
);
GO

--12
CREATE TABLE danhMucContainer (
	maDanhMucContainer CHAR(10) NOT NULL PRIMARY KEY, -- Khóa chính
	tenDanhMucContainer NVARCHAR(100) NOT NULL, -- UNIQUE
	sucChuaToiDa FLOAT NOT NULL,
	trongTaiToiDa FLOAT NOT NULL,
	chieuDai FLOAT NOT NULL,
	chieuRong FLOAT NOT NULL,
	chieuCao FLOAT NOT NULL,
	moTa NVARCHAR(255) NULL
);
GO

--13
CREATE TABLE container (
	maContainer CHAR(10) NOT NULL PRIMARY KEY, -- Khóa chính
	maDanhMucContainer CHAR(10) NOT NULL FOREIGN KEY REFERENCES danhMucContainer(maDanhMucContainer),
	maChiTietKho CHAR(10) NOT NULL FOREIGN KEY REFERENCES chiTietKho(maChiTietKho),
	soHieu CHAR(8) NOT NULL, -- UNIQUE
	trangThaiContainer NVARCHAR(100) NOT NULL,
	viTriTrongKho VARCHAR(255) NOT NULL,
	ngayMuaContainer DATE NULL,
	trongTai FLOAT NOT NULL
);
GO

--14
CREATE TABLE phieuNhap (
	maPhieuNhap CHAR(10) NOT NULL PRIMARY KEY, -- Khóa chính
	maDonHang CHAR(10) NOT NULL FOREIGN KEY REFERENCES donHang(maDonHang),
	moTa NVARCHAR(255) NULL
);
GO

--15
CREATE TABLE chiTietPhieuNhap (
	maChiTietPhieuNhap CHAR(10) NOT NULL PRIMARY KEY, -- Khóa chính
	maPhieuNhap CHAR(10) NOT NULL FOREIGN KEY REFERENCES phieuNhap(maPhieuNhap),
	maContainer CHAR(10) NOT NULL FOREIGN KEY REFERENCES container(maContainer),
	moTa NVARCHAR(255) NULL
);
GO

--1 Insert dữ liệu cho bảng khachHang
INSERT INTO khachHang (maKhachHang, tenDangNhap, matKhau, tenKhachHang, tenCongTy, maSoThueCongTy, ngayDangKy, cccd, diaChiLienLac, sdtKhachHang, email, trangThaiTaiKhoanKhachHang, anhDaiDienKhachHangUrl)
VALUES 
('KH00000001', 'khachhang1', 'password123', N'Nguyễn Văn An', N'Công ty TNHH Alpha', '01012345678901', '2024-01-01', '123456789001', N'123 Đường Lê Lợi, Đà Nẵng', '0905123456', 'an.nguyen@email.com', N'Đã duyệt', NULL),
('KH00000002', 'khachhang2', 'password456', N'Trần Thị Bình', N'Công ty CP Beta', '01012345678902', '2024-02-01', '123456789002', N'456 Đường Nguyễn Huệ, Đà Nẵng', '0905123457', 'binh.tran@email.com', N'Đã duyệt', NULL),
('KH00000003', 'khachhang3', 'password789', N'Lê Văn Cường', NULL, NULL, '2024-03-01', '123456789003', N'789 Đường Trần Phú, Đà Nẵng', '0905123458', 'cuong.le@email.com', N'Chờ duyệt', NULL),
('KH00000004', 'khachhang4', 'pass123456', N'Phạm Thị Dung', N'Công ty TNHH Gamma', '01012345678903', '2024-04-01', '123456789004', N'101 Đường Hùng Vương, Đà Nẵng', '0905123459', 'dung.pham@email.com', N'Đã duyệt', NULL),
('KH00000005', 'khachhang5', 'pass789123', N'Hoàng Văn Em', NULL, NULL, '2024-05-01', '123456789005', N'202 Đường Phạm Văn Đồng, Đà Nẵng', '0905123460', 'em.hoang@email.com', N'Đã duyệt', NULL),
('KH00000006', 'khachhang6', 'password321', N'Ngô Thị Phượng', N'Công ty CP Delta', '01012345678904', '2024-06-01', '123456789006', N'303 Đường Nguyễn Văn Linh, Đà Nẵng', '0905123461', 'phuong.ngo@email.com', N'Đang bị khóa', NULL),
('KH00000007', 'khachhang7', 'pass654321', N'Vũ Văn Giang', NULL, NULL, '2024-07-01', '123456789007', N'404 Đường Lê Duẩn, Đà Nẵng', '0905123462', 'giang.vu@email.com', N'Đã duyệt', NULL),
('KH00000008', 'khachhang8', 'password987', N'Đỗ Thị Hà', N'Công ty TNHH Epsilon', '01012345678905', '2024-08-01', '123456789008', N'505 Đường Nguyễn Tất Thành, Đà Nẵng', '0905123463', 'ha.do@email.com', N'Chờ duyệt', NULL),
('KH00000009', 'khachhang9', 'pass123789', N'Bùi Văn Hùng', NULL, NULL, '2024-09-01', '123456789009', N'606 Đường Tôn Đức Thắng, Đà Nẵng', '0905123464', 'hung.bui@email.com', N'Đã duyệt', NULL),
('KH00000010', 'khachhang10', 'password147', N'Nguyễn Thị In', N'Công ty CP Zeta', '01012345678906', '2024-10-01', '123456789010', N'707 Đường Võ Nguyên Giáp, Đà Nẵng', '0905123465', 'in.nguyen@email.com', N'Đã duyệt', NULL);

--2 Insert dữ liệu cho bảng vaiTroNhanVien
INSERT INTO vaiTroNhanVien (maVaiTroNhanVien, tenLoaiNhanVien, moTa)
VALUES 
('VTNV000001', N'Quản trị viên', N'Quản lý hệ thống và phân quyền'),
('VTNV000002', N'Nhân viên nhập kho', N'Xử lý nhập hàng vào kho'),
('VTNV000003', N'Nhân viên xuất kho', N'Xử lý xuất hàng ra khỏi kho'),
('VTNV000004', N'Nhân viên kho bãi', N'Quản lý kho và kiểm tra hàng hóa'),
('VTNV000005', N'Nhân viên hải quan', N'Xử lý thủ tục hải quan'),
('VTNV000006', N'Nhân viên kế toán', N'Quản lý tài chính và thanh toán');

--3 Insert dữ liệu cho bảng nhanVien
INSERT INTO nhanVien (maNhanVien, maLoaiNhanVien, tenDangNhap, matKhau, tenHienThi, sdtNhanVien, diaChi, email, trangThaiTaiKhoanNhanVien, thoiGianDangNhapGanNhat, anhDaiDienNhanVienUrl)
VALUES 
('NV00000001', 'VTNV000001', 'admin1', 'admin123', N'Nguyễn Văn Admin', '0912345678', N'123 Đường Admin, Đà Nẵng', 'admin1.le@email.com', N'Đang hoạt động', '2025-05-01 08:00:00', NULL),
('NV00000002', 'VTNV000002', 'nhapkho1', 'nhapkho123', N'Trần Thị Nhập', '0912345679', N'456 Đường Nhập Kho, Đà Nẵng', 'nhap1.le@email.com', N'Đang hoạt động', '2025-05-02 09:00:00', NULL),
('NV00000003', 'VTNV000003', 'xuatkho1', 'xuatkho123', N'Lê Văn Xuất', '0912345680', N'789 Đường Xuất Kho, Đà Nẵng', 'xuat1.le@email.com', N'Đang hoạt động', '2025-05-03 10:00:00', NULL),
('NV00000004', 'VTNV000004', 'khobai1', 'khobai123', N'Phạm Thị Kho', '0912345681', N'101 Đường Kho Bãi, Đà Nẵng', 'khobai1.le@email.com', N'Đang hoạt động', '2025-05-04 11:00:00', NULL),
('NV00000005', 'VTNV000005', 'haiquan1', 'haiquan123', N'Hoàng Văn Hải', '0912345682', N'202 Đường Hải Quan, Đà Nẵng', 'haiquan1.le@email.com', N'Đang hoạt động', '2025-05-05 12:00:00', NULL),
('NV00000006', 'VTNV000006', 'ketoan1', 'ketoan123', N'Ngô Thị Kế', '0912345683', N'303 Đường Kế Toán, Đà Nẵng', 'ketoan1.le@email.com', N'Đang hoạt động', '2025-05-06 13:00:00', NULL),
('NV00000007', 'VTNV000001', 'admin2', 'admin456', N'Vũ Văn Admin', '0912345684', N'404 Đường Admin, Đà Nẵng', 'admin2.le@email.com', N'Đang hoạt động', '2025-05-07 14:00:00', NULL),
('NV00000008', 'VTNV000002', 'nhapkho2', 'nhapkho456', N'Đỗ Thị Nhập', '0912345685', N'505 Đường Nhập Kho, Đà Nẵng', 'nhapkho2.le@email.com', N'Đang hoạt động', '2025-05-08 15:00:00', NULL),
('NV00000009', 'VTNV000003', 'xuatkho2', 'xuatkho456', N'Bùi Văn Xuất', '0912345686', N'606 Đường Xuất Kho, Đà Nẵng', 'xuatkho2.le@email.com', N'Đang hoạt động', '2025-05-09 16:00:00', NULL),
('NV00000010', 'VTNV000004', 'khobai2', 'khobai456', N'Nguyễn Thị Kho', '0912345687', N'707 Đường Kho Bãi, Đà Nẵng', 'khobai2.le@email.com', N'Đang hoạt động', '2025-05-10 17:00:00', NULL);

--4 Insert dữ liệu cho bảng donHang
INSERT INTO donHang (maDonHang, maKhachHang, maNhanVien, tenNguoiGui, tenNguoiNhan, cangDichDen, ngayTaoDonHang, thoiGianLuuTru, ngayXuatCang, ngayNhapCang, trangThaiDonHang, trangThaiThanhToan, tongTien, moTa)
VALUES 
('DH00000001', 'KH00000001', 'NV00000001', N'Nguyễn Văn An', N'Trần Văn Bình', N'Cảng Singapore', '2024-01-10 08:00:00', 30, '2024-02-10 08:00:00', '2024-02-05 08:00:00', N'Hoàn thành', N'Đã thanh toán', 5000000, N'Đơn hàng xuất khẩu'),
('DH00000002', 'KH00000002', 'NV00000002', N'Trần Thị Bình', N'Lê Văn Cường', N'Cảng Shanghai', '2024-02-10 09:00:00', 20, NULL, '2024-03-01 09:00:00', N'Đang xử lý', N'Chưa thanh toán', 3000000, N'Đơn hàng nhập khẩu'),
('DH00000003', 'KH00000003', 'NV00000003', N'Lê Văn Cường', N'Phạm Thị Dung', N'Cảng Tokyo', '2024-03-10 10:00:00', 15, '2024-04-10 10:00:00', NULL, N'Đang vận chuyển', N'Đã thanh toán', 4000000, N'Đơn hàng xuất khẩu'),
('DH00000004', 'KH00000004', 'NV00000004', N'Phạm Thị Dung', N'Hoàng Văn Em', N'Cảng Busan', '2024-04-10 11:00:00', 25, NULL, '2024-05-01 11:00:00', N'Đang xử lý', N'Chưa thanh toán', 6000000, N'Đơn hàng nhập khẩu'),
('DH00000005', 'KH00000005', 'NV00000005', N'Hoàng Văn Em', N'Ngô Thị Phượng', N'Cảng Hong Kong', '2024-05-10 12:00:00', 10, '2024-06-10 12:00:00', NULL, N'Đang xử lý', N'Đã thanh toán', 2000000, N'Đơn hàng xuất khẩu'),
('DH00000006', 'KH00000006', 'NV00000006', N'Ngô Thị Phượng', N'Vũ Văn Giang', N'Cảng Kaohsiung', '2024-06-10 13:00:00', 30, NULL, '2024-07-01 13:00:00', N'Đang xử lý', N'Chưa thanh toán', 7000000, N'Đơn hàng nhập khẩu'),
('DH00000007', 'KH00000007', 'NV00000007', N'Vũ Văn Giang', N'Đỗ Thị Hà', N'Cảng Los Angeles', '2024-07-10 14:00:00', 20, '2024-08-10 14:00:00', NULL, N'Đang xử lý', N'Đã thanh toán', 8000000, N'Đơn hàng xuất khẩu'),
('DH00000008', 'KH00000008', 'NV00000008', N'Đỗ Thị Hà', N'Bùi Văn Hùng', N'Cảng Rotterdam', '2024-08-10 15:00:00', 15, NULL, '2024-09-01 15:00:00', N'Đang xử lý', N'Chưa thanh toán', 9000000, N'Đơn hàng nhập khẩu'),
('DH00000009', 'KH00000009', 'NV00000009', N'Bùi Văn Hùng', N'Nguyễn Thị In', N'Cảng Hamburg', '2024-09-10 16:00:00', 25, '2024-10-10 16:00:00', NULL, N'Đang vận chuyển', N'Đã thanh toán', 10000000, N'Đơn hàng xuất khẩu'),
('DH00000010', 'KH00000010', 'NV00000010', N'Nguyễn Thị In', N'Nguyễn Văn An', N'Cảng Dubai', '2024-10-10 17:00:00', 30, NULL, '2024-11-01 17:00:00', N'Đang xử lý', N'Chưa thanh toán', 11000000, N'Đơn hàng nhập khẩu');

--5 Insert dữ liệu cho bảng danhMucHangHoa
INSERT INTO danhMucHangHoa (maDanhMucHangHoa, tenDanhMucHangHoa, moTa)
VALUES 
('DMHH000001', N'Hàng điện tử', N'Hàng hóa thuộc ngành điện tử'),
('DMHH000002', N'Hàng thực phẩm', N'Hàng hóa thực phẩm chế biến'),
('DMHH000003', N'Hàng may mặc', N'Sản phẩm dệt may'),
('DMHH000004', N'Hàng công nghiệp', N'Sản phẩm công nghiệp nặng');

--6 Insert dữ liệu cho bảng hangHoa
INSERT INTO hangHoa (maHangHoa, maDanhMucHangHoa, tenHangHoa, chiPhiLuuKhoToiThieu, donViTinh, moTa)
VALUES 
('HH00000001', 'DMHH000001', N'Tivi Samsung 55 inch', 500000, N'Cái', N'Tivi LED cao cấp'),
('HH00000002', 'DMHH000001', N'Điện thoại iPhone 14', 300000, N'Cái', N'Điện thoại thông minh'),
('HH00000003', 'DMHH000002', N'Gạo ST25', 100000, N'Tấn', N'Gạo chất lượng cao'),
('HH00000004', 'DMHH000002', N'Cá đông lạnh', 200000, N'Tấn', N'Cá đông lạnh xuất khẩu'),
('HH00000005', 'DMHH000003', N'Áo thun cotton', 50000, N'Cái', N'Áo thun thời trang'),
('HH00000006', 'DMHH000003', N'Quần jeans', 70000, N'Cái', N'Quần jeans cao cấp'),
('HH00000007', 'DMHH000004', N'Thép cuộn', 1000000, N'Tấn', N'Thép cuộn công nghiệp'),
('HH00000008', 'DMHH000004', N'Xi măng', 800000, N'Tấn', N'Xi măng xây dựng'),
('HH00000009', 'DMHH000001', N'Máy tính Dell', 400000, N'Cái', N'Laptop doanh nghiệp'),
('HH00000010', 'DMHH000002', N'Nước mắm Phú Quốc', 150000, N'Thùng', N'Nước mắm truyền thống');

--7 Insert dữ liệu cho bảng chiTietDonHang
INSERT INTO chiTietDonHang (maChiTietDonHang, maDonHang, maHangHoa, soLuong, donViTinh, chatLuong, donGia, tienLuuKho, moTa)
VALUES 
('CTDH000001', 'DH00000001', 'HH00000001', 10, N'Cái', N'Cao cấp', 10000000, 5000000, N'Tivi chất lượng cao'),
('CTDH000002', 'DH00000002', 'HH00000003', 5, N'Tấn', N'Tốt', 2000000, 500000, N'Gạo xuất khẩu'),
('CTDH000003', 'DH00000003', 'HH00000005', 100, N'Cái', N'Trung bình', 100000, 5000000, N'Áo thun thời trang'),
('CTDH000004', 'DH00000004', 'HH00000007', 10, N'Tấn', N'Cao cấp', 15000000, 10000000, N'Thép công nghiệp'),
('CTDH000005', 'DH00000005', 'HH00000002', 20, N'Cái', N'Cao cấp', 20000000, 6000000, N'Điện thoại cao cấp'),
('CTDH000006', 'DH00000006', 'HH00000004', 8, N'Tấn', N'Tốt', 3000000, 1600000, N'Cá đông lạnh'),
('CTDH000007', 'DH00000007', 'HH00000006', 50, N'Cái', N'Trung bình', 200000, 3500000, N'Quần jeans thời trang'),
('CTDH000008', 'DH00000008', 'HH00000008', 15, N'Tấn', N'Cao cấp', 1200000, 12000000, N'Xi măng xây dựng'),
('CTDH000009', 'DH00000009', 'HH00000009', 10, N'Cái', N'Cao cấp', 15000000, 4000000, N'Laptop doanh nghiệp'),
('CTDH000010', 'DH00000010', 'HH00000010', 20, N'Thùng', N'Tốt', 500000, 3000000, N'Nước mắm truyền thống');

--8 Insert dữ liệu cho bảng phieuXuat
INSERT INTO phieuXuat (maPhieuXuat, maDonHang, trangThaiXuatHang, ngayXuatKho, moTa)
VALUES 
('PX00000001', 'DH00000001', N'Hoàn thành', '2024-02-10 08:00:00', N'Xuất hàng đúng hạn'),
('PX00000002', 'DH00000003', N'Hoàn thành', '2024-04-10 10:00:00', N'Xuất hàng đúng hạn'),
('PX00000003', 'DH00000005', N'Hoàn thành', '2024-06-10 12:00:00', N'Xuất hàng đúng hạn'),
('PX00000004', 'DH00000007', N'Hoàn thành', '2024-08-10 14:00:00', N'Xuất hàng đúng hạn'),
('PX00000005', 'DH00000009', N'Hoàn thành', '2024-10-10 16:00:00', N'Xuất hàng đúng hạn'),
('PX00000006', 'DH00000001', N'Hoàn thành', '2024-02-11 08:00:00', N'Xuất hàng bổ sung'),
('PX00000007', 'DH00000003', N'Hoàn thành', '2024-04-11 10:00:00', N'Xuất hàng bổ sung'),
('PX00000008', 'DH00000005', N'Hoàn thành', '2024-06-11 12:00:00', N'Xuất hàng bổ sung'),
('PX00000009', 'DH00000007', N'Hoàn thành', '2024-08-11 14:00:00', N'Xuất hàng bổ sung'),
('PX00000010', 'DH00000009', N'Hoàn thành', '2024-10-11 16:00:00', N'Xuất hàng bổ sung');

--9 Insert dữ liệu cho bảng loaiKho
INSERT INTO loaiKho (maLoaiKho, tenLoaiKho, soLuongKho, moTa)
VALUES 
('LK00000001', N'Kho lạnh', 5, N'Kho bảo quản hàng đông lạnh'),
('LK00000002', N'Kho thường', 10, N'Kho bảo quản hàng hóa thông thường'),
('LK00000003', N'Kho container', 3, N'Kho chứa container');

--10 Insert dữ liệu cho bảng kho
INSERT INTO kho (maKho, maLoaiKho, tenKho, diaChiKho, trangThaiKho, sucChuaToiDa, tonKho, moTa)
VALUES 
('KHO0000001', 'LK00000001', N'Kho lạnh 1', N'123 Đường Kho Lạnh, Đà Nẵng', N'Hoạt động', 1000, 500, N'Kho lạnh hiện đại'),
('KHO0000002', 'LK00000001', N'Kho lạnh 2', N'456 Đường Kho Lạnh, Đà Nẵng', N'Hoạt động', 800, 400, N'Kho lạnh tiêu chuẩn'),
('KHO0000003', 'LK00000002', N'Kho thường 1', N'789 Đường Kho Thường, Đà Nẵng', N'Hoạt động', 2000, 1000, N'Kho thường rộng rãi'),
('KHO0000004', 'LK00000002', N'Kho thường 2', N'101 Đường Kho Thường, Đà Nẵng', N'Hoạt động', 1500, 750, N'Kho thường tiêu chuẩn'),
('KHO0000005', 'LK00000003', N'Kho container 1', N'202 Đường Kho Container, Đà Nẵng', N'Hoạt động', 500, 200, N'Kho chứa container lớn'),
('KHO0000006', 'LK00000003', N'Kho container 2', N'303 Đường Kho Container, Đà Nẵng', N'Hoạt động', 400, 150, N'Kho chứa container tiêu chuẩn'),
('KHO0000007', 'LK00000001', N'Kho lạnh 3', N'404 Đường Kho Lạnh, Đà Nẵng', N'Hoạt động', 600, 300, N'Kho lạnh nhỏ'),
('KHO0000008', 'LK00000002', N'Kho thường 3', N'505 Đường Kho Thường, Đà Nẵng', N'Hoạt động', 1200, 600, N'Kho thường hiện đại'),
('KHO0000009', 'LK00000002', N'Kho thường 4', N'606 Đường Kho Thường, Đà Nẵng', N'Hoạt động', 1800, 900, N'Kho thường lớn'),
('KHO0000010', 'LK00000003', N'Kho container 3', N'707 Đường Kho Container, Đà Nẵng', N'Hoạt động', 300, 100, N'Kho chứa container nhỏ');

--11 Insert dữ liệu cho bảng chiTietKho
INSERT INTO chiTietKho (maChiTietKho, maKho, sucChuaToiDa, tonKho, trangThaiChiTietKho, ngayCapNhatCuoi, moTa)
VALUES 
('CTK0000001', 'KHO0000001', 1000, 500, N'Hoạt động', '2025-05-01 08:00:00', N'Chi tiết kho lạnh 1'),
('CTK0000002', 'KHO0000002', 800, 400, N'Hoạt động', '2025-05-02 09:00:00', N'Chi tiết kho lạnh 2'),
('CTK0000003', 'KHO0000003', 2000, 1000, N'Hoạt động', '2025-05-03 10:00:00', N'Chi tiết kho thường 1'),
('CTK0000004', 'KHO0000004', 1500, 750, N'Hoạt động', '2025-05-04 11:00:00', N'Chi tiết kho thường 2'),
('CTK0000005', 'KHO0000005', 500, 200, N'Hoạt động', '2025-05-05 12:00:00', N'Chi tiết kho container 1'),
('CTK0000006', 'KHO0000006', 400, 150, N'Hoạt động', '2025-05-06 13:00:00', N'Chi tiết kho container 2'),
('CTK0000007', 'KHO0000007', 600, 300, N'Hoạt động', '2025-05-07 14:00:00', N'Chi tiết kho lạnh 3'),
('CTK0000008', 'KHO0000008', 1200, 600, N'Hoạt động', '2025-05-08 15:00:00', N'Chi tiết kho thường 3'),
('CTK0000009', 'KHO0000009', 1800, 900, N'Hoạt động', '2025-05-09 16:00:00', N'Chi tiết kho thường 4'),
('CTK0000010', 'KHO0000010', 300, 100, N'Hoạt động', '2025-05-10 17:00:00', N'Chi tiết kho container 3');

--12 Insert dữ liệu cho bảng danhMucContainer
INSERT INTO danhMucContainer (maDanhMucContainer, tenDanhMucContainer, sucChuaToiDa, trongTaiToiDa, chieuDai, chieuRong, chieuCao, moTa)
VALUES 
('DMC0000001', N'Container 20 feet', 33.2, 24000, 6.058, 2.438, 2.591, N'Container tiêu chuẩn 20 feet'),
('DMC0000002', N'Container 40 feet', 67.7, 30480, 12.192, 2.438, 2.591, N'Container tiêu chuẩn 40 feet'),
('DMC0000003', N'Container 40 feet cao', 76.4, 30480, 12.192, 2.438, 2.896, N'Container cao 40 feet');

--13 Insert dữ liệu cho bảng container
INSERT INTO container (maContainer, maDanhMucContainer, maChiTietKho, soHieu, trangThaiContainer, viTriTrongKho, ngayMuaContainer, trongTai)
VALUES 
('CONT000001', 'DMC0000001', 'CTK0000001', 'ABCD1234', N'Hoạt động', 'A1-001', '2023-01-01', 24000),
('CONT000002', 'DMC0000002', 'CTK0000002', 'EFGH5678', N'Hoạt động', 'A2-002', '2023-02-01', 30480),
('CONT000003', 'DMC0000003', 'CTK0000003', 'IJKL9012', N'Hoạt động', 'B1-003', '2023-03-01', 30480),
('CONT000004', 'DMC0000001', 'CTK0000004', 'MNOP3456', N'Hoạt động', 'B2-004', '2023-04-01', 24000),
('CONT000005', 'DMC0000002', 'CTK0000005', 'QRST7890', N'Hoạt động', 'C1-005', '2023-05-01', 30480),
('CONT000006', 'DMC0000003', 'CTK0000006', 'UVWX1234', N'Hoạt động', 'C2-006', '2023-06-01', 30480),
('CONT000007', 'DMC0000001', 'CTK0000007', 'YZAB5678', N'Hoạt động', 'D1-007', '2023-07-01', 24000),
('CONT000008', 'DMC0000002', 'CTK0000008', 'CDEF9012', N'Hoạt động', 'D2-008', '2023-08-01', 30480),
('CONT000009', 'DMC0000003', 'CTK0000009', 'GHIJ3456', N'Hoạt động', 'E1-009', '2023-09-01', 30480),
('CONT000010', 'DMC0000001', 'CTK0000010', 'KLMN7890', N'Hoạt động', 'E2-010', '2023-10-01', 24000);

--14 Insert dữ liệu cho bảng phieuNhap
INSERT INTO phieuNhap (maPhieuNhap, maDonHang, moTa)
VALUES 
('PN00000001', 'DH00000002', N'Nhập hàng từ cảng Shanghai'),
('PN00000002', 'DH00000004', N'Nhập hàng từ cảng Busan'),
('PN00000003', 'DH00000006', N'Nhập hàng từ cảng Kaohsiung'),
('PN00000004', 'DH00000008', N'Nhập hàng từ cảng Rotterdam'),
('PN00000005', 'DH00000010', N'Nhập hàng từ cảng Dubai'),
('PN00000006', 'DH00000002', N'Nhập hàng bổ sung từ cảng Shanghai'),
('PN00000007', 'DH00000004', N'Nhập hàng bổ sung từ cảng Busan'),
('PN00000008', 'DH00000006', N'Nhập hàng bổ sung từ cảng Kaohsiung'),
('PN00000009', 'DH00000008', N'Nhập hàng bổ sung từ cảng Rotterdam'),
('PN00000010', 'DH00000010', N'Nhập hàng bổ sung từ cảng Dubai');

--15 Insert dữ liệu cho bảng chiTietPhieuNhap
INSERT INTO chiTietPhieuNhap (maChiTietPhieuNhap, maPhieuNhap, maContainer, moTa)
VALUES 
('CTPN000001', 'PN00000001', 'CONT000001', N'Container chứa gạo'),
('CTPN000002', 'PN00000002', 'CONT000002', N'Container chứa thép'),
('CTPN000003', 'PN00000003', 'CONT000003', N'Container chứa cá đông lạnh'),
('CTPN000004', 'PN00000004', 'CONT000004', N'Container chứa xi măng'),
('CTPN000005', 'PN00000005', 'CONT000005', N'Container chứa nước mắm'),
('CTPN000006', 'PN00000006', 'CONT000006', N'Container chứa gạo bổ sung'),
('CTPN000007', 'PN00000007', 'CONT000007', N'Container chứa thép bổ sung'),
('CTPN000008', 'PN00000008', 'CONT000008', N'Container chứa cá đông lạnh bổ sung'),
('CTPN000009', 'PN00000009', 'CONT000009', N'Container chứa xi măng bổ sung'),
('CTPN000010', 'PN00000010', 'CONT000010', N'Container chứa nước mắm bổ sung');
GO
CREATE TRIGGER CapNhatGiaTienChiTietDonHang
ON chiTietDonHang
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Cập nhật donGia theo danh mục hàng hóa
    UPDATE ctdh
    SET donGia = 
        CASE hh.maDanhMucHangHoa
            WHEN 'DMHH000001' THEN 90000   -- Hàng điện tử
            WHEN 'DMHH000002' THEN 60000   -- Hàng thực phẩm
            WHEN 'DMHH000003' THEN 30000   -- Hàng may mặc
            WHEN 'DMHH000004' THEN 70000   -- Hàng công nghiệp
            ELSE 10000                     -- Mặc định
        END
    FROM chiTietDonHang ctdh
    INNER JOIN inserted i ON ctdh.maChiTietDonHang = i.maChiTietDonHang
    INNER JOIN hangHoa hh ON ctdh.maHangHoa = hh.maHangHoa;

    -- Cập nhật tienLuuKho theo quy tắc
    UPDATE ctdh
    SET tienLuuKho =
        CASE 
            WHEN i.soLuong <= 10 THEN hh.chiPhiLuuKhoToiThieu
            ELSE hh.chiPhiLuuKhoToiThieu + hh.chiPhiLuuKhoToiThieu * FLOOR((i.soLuong - 10) / 20)
        END
    FROM chiTietDonHang ctdh
    INNER JOIN inserted i ON ctdh.maChiTietDonHang = i.maChiTietDonHang
    INNER JOIN hangHoa hh ON ctdh.maHangHoa = hh.maHangHoa;

    -- Cập nhật tongTien trong donHang
    UPDATE dh
    SET tongTien = (
        SELECT SUM(ct.soLuong * ct.donGia + ISNULL(ct.tienLuuKho, 0))
        FROM chiTietDonHang ct
        WHERE ct.maDonHang = dh.maDonHang
    )
    FROM donHang dh
    WHERE dh.maDonHang IN (
        SELECT maDonHang FROM inserted
    );
END;

