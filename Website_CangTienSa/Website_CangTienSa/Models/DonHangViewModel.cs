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
    }
    
}