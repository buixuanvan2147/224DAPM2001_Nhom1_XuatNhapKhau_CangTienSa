using System.ComponentModel.DataAnnotations;

namespace Website_CangTienSa.Models
{
    public class ContainerModel
    {
        [Key]
        public string MaContainer { get; set; }
        public string ViTriTrongKho { get; set; }
        public string KichThuoc { get; set; }
        public string TenKho { get; set; }
        public string TenLoaiKho { get; set; }
        public string MaDonHang { get; set; } // Lưu mã đơn hàng từ phieuNhap
        public string MoTa { get; set; } // Lưu mô tả từ chiTietPhieuNhap
    }
}