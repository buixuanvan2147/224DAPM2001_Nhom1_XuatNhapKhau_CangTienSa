using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Website_CangTienSa.Models
{
    public class KhachHangViewModel
    {
        public int maKhachHang { get; set; }
        public string tenKhachHang { get; set; }
        public string tenCongTy { get; set; }
        public string maSoThueCongTy { get; set; }
        public string email { get; set; }
        public string sdtKhachHang { get; set; }
        public string diaChiLienLac { get; set; }
        public string tenDangNhap { get; set; }
        public string matKhau { get; set; }
    }
}