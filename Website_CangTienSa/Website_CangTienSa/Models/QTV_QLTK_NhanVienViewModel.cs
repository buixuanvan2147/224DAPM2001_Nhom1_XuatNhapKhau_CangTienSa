using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website_CangTienSa.Models
{
    public class QTV_QLTK_NhanVienViewModel
    {
        public string MaNhanVien { get; set; }
        public string AnhDaiDienNhanVienUrl { get; set; } // Lưu ý: bạn đang đặt tên là 'NhanVienUrl' trong controller
        public string TenDangNhap { get; set; }
        public string MatKhau { get; set; }
        public string TenHienThi { get; set; }
        public string SdtNhanVien { get; set; }
        public string DiaChi { get; set; }
        public string Email { get; set; }
        public string TrangThaiTaiKhoanNhanVien { get; set; }
        public DateTime? ThoiGianDangNhapGanNhat { get; set; } // Sử dụng DateTime? để chấp nhận giá trị null
        public string TenLoaiNhanVien { get; set; }
    }
}