using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace Website_CangTienSa.Models
{
    public class DonHangChiTietViewModel
    {
        public donHang DonHang { get; set; }
        public List<chiTietDonHang> ChiTietDonHang { get; set; }
    }

}