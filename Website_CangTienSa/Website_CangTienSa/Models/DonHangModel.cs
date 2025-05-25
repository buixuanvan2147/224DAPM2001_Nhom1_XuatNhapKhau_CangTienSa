using System;
using System.ComponentModel.DataAnnotations;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace Website_CangTienSa.Models
{
    public class DonHangModel
    {
        [Key]
        public string LoaiHangHoa { get; set; }
        public int ThoiGianLuuTru { get; set; }
        public DateTime NgayNhapHang { get; set; }
        public string TrangThaiPhanLoai { get; set; }
        public string TrangThaiDonHang { get; set; }


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
        public string TrangThai { get; set; }
        public DateTime? NgayNhapKho { get; set; }
        public string NguoiXacNhan { get; set; }
    }


}