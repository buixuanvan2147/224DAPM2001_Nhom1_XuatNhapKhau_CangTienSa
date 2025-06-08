using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website_CangTienSa.Models
{
    public class QTV_TKBC_KhachHang_TongKhachHangTheoTrangThai
    {
        public string TrangThai { get; set; } // Có các trạng thái : "Đã duyệt", "Chờ duyệt", "Đang bị khóa", " Đã từ chối"
        public int SoLuong { get; set; }     // Số lượng khách hàng ở trạng thái đó
    }
}