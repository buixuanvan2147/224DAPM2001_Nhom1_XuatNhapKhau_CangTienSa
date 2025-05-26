using System.ComponentModel.DataAnnotations;
using System;

public class DonHangModel
{
    [Key]
    public string LoaiHangHoa { get; set; }
    public int ThoiGianLuuTru { get; set; }
    public DateTime NgayNhapHang { get; set; }
    public string TrangThaiPhanLoai { get; set; }
    public string TrangThaiDonHang { get; set; } // Đã có sẵn để lưu trạng thái đơn hàng

    public string MaDonHang { get; set; }
    public DateTime? NgayNhapCang { get; set; }
    public string TenNguoiGui { get; set; }
    public string MaKho { get; set; }
    public string TenKho { get; set; }
    public string MaContainer { get; set; }
    public string MaPhieuNhap { get; set; }
    public string MaHangHoa { get; set; }
    public float SoLuong { get; set; }
    public string TenSanPham { get; set; }
    public string DonViTinh { get; set; }
    public string TrangThai { get; set; } // Lưu trạng thái thanh toán
    public DateTime? NgayNhapKho { get; set; }
    public string NguoiXacNhan { get; set; }
    public string MoTa { get; set; }
}