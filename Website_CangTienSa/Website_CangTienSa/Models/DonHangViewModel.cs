using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Website_CangTienSa.Models
{
    public class DonHangViewModel
    {
        public List<donHang> DanhSachDonHang { get; set; }
        public donHang DonHangChiTiet { get; set; }
        public List<chiTietDonHang> ChiTietDonHang { get; set; }

        public string MaDonHang { get; set; }
        public string TenKhachHang { get; set; }
        public DateTime NgayTaoDon { get; set; }
        public float TongTien { get; set; }
        public string TrangThai { get; set; }  // "Đã thanh toán" hoặc "Chưa thanh toán"
        public DateTime? ThoiGianThanhToan { get; set; }
    }
    
}