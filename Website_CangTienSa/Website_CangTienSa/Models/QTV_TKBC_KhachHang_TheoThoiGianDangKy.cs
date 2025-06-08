using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website_CangTienSa.Models
{
    public class QTV_TKBC_KhachHang_TheoThoiGianDangKy
    {
        public string ThoiGianDangKy { get; set; } // Ví dụ: "Tháng 1-6/2024", "Tháng 7-12/2024"
        public int SoLuongKhachHang { get; set; }
        public string MaThoiGian { get; set; } // Dùng để truyền tham số khi click "Xem chi tiết" (ví dụ: "2024-H1", "2024-H2")
    }
}