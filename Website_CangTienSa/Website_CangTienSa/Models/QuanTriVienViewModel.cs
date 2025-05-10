using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;    
namespace Website_CangTienSa.Models
{
    public class QuanTriVienViewModel
    {
        public string MaNhanVien { get; set; }
        public string TenDangNhap { get; set; }
        public string TenHienThi { get; set; }
        public string Email { get; set; }
        public string SdtNhanVien { get; set; }
        public string DiaChi { get; set; }
        public DateTime? ThoiGianDangNhapGanNhat { get; set; }
        public string AnhDaiDienNhanVienUrl { get; set; }
    }
}