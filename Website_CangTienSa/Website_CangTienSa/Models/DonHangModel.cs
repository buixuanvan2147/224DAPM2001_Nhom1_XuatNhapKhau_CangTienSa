using System;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace Website_CangTienSa.Models
{
    public class DonHangModel
    {
        public string MaDonHang { get; set; }
        public DateTime? NgayNhapCang { get; set; }
        public string TenNguoiGui { get; set; }
        public string MaKho { get; set; }
        public string TenKho { get; set; }
        public string MaContainer { get; set; }
        public string MaPhieuNhap { get; set; }
        public string MaHangHoa { get; set; }
        public float SoLuong { get; set; }
        public string TrangThai { get; set; }
        public DateTime? NgayNhapKho { get; set; } 
    public string NguoiXacNhan { get; set; }
    }
}