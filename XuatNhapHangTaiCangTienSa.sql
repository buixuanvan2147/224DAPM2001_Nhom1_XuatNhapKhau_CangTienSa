CREATE DATABASE  XuatNhapHangTaiCangTienSa
GO
USE XuatNhapHangTaiCangTienSa
GO

--1
CREATE TABLE khachHang (
    maKhachHang CHAR(10) NOT NULL PRIMARY KEY, -- Khóa chính
    tenDangNhap VARCHAR(100) NOT NULL,
    matKhau VARCHAR(50) NOT NULL,
    tenKhachHang NVARCHAR(100) NOT NULL,
    tenCongTy NVARCHAR(255) NULL,
    maSoThueCongTy CHAR(14) NULL,
    ngayDangKy DATE NOT NULL DEFAULT GETDATE(),
    cccd CHAR(12) NULL,
    diaChiLienLac NVARCHAR(100) NOT NULL,
    sdtKhachHang CHAR(10) NOT NULL,
    email NVARCHAR(50) NOT NULL,
	trangThaiTaiKhoanKhachHang NVarChar(100) NOT NULL
);
GO

--2
CREATE TABLE vaiTroNhanVien (
	maVaiTroNhanVien CHAR(10) NOT NULL PRIMARY KEY, -- Khóa chính
	tenLoaiNhanVien NVARCHAR(100) NOT NULL,
	moTa NVARCHAR(255) NULL,
	);
GO

--3
CREATE TABLE nhanVien (
	maNhanVien CHAR(10) NOT NULL PRIMARY KEY, -- Khóa chính
	maLoaiNhanVien CHAR(10) NOT NULL FOREIGN KEY REFERENCES vaiTroNhanVien(maVaiTroNhanVien),
	tenDangNhap VARCHAR(50) NOT NULL,
	matKhau VARCHAR(50) NOT NULL,
	tenHienThi NVARCHAR(100) NULL,
	sdtNhanVien VARCHAR(11) NULL,
	diaChi NVARCHAR(100) NULL,
	trangThaiTaiKhoanNhanVien NVARCHAR(100) NOT NULL,
	thoiGianDangNhapGanNhat DATETIME NOT NULL
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
	tenDanhMucHangHoa NVARCHAR(100) NOT NULL,
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
	tenLoaiKho NVARCHAR(100) NOT NULL,
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
	tenDanhMucContainer NVARCHAR(100) NOT NULL,
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
	soHieu CHAR(8) NOT NULL,
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
