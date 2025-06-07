using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Website_CangTienSa.Models
{
    public class HangHoaInput
    {
        public string maDanhMucHangHoa { get; set; }
        public string maHangHoa { get; set; }
        public string LoaiHang { get; set; }
        public string DonViTinhKhoiLuong { get; set; }
        public float? KhoiLuong { get; set; }
        public float? SoLuong { get; set; }
        public string ThoiGianLuuKho { get; set; }
        public int? ThoiGianLuuKhoKhac { get; set; }
    }
}