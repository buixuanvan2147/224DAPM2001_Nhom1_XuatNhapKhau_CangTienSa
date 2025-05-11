using System.Collections.Generic;
using System;

namespace Website_CangTienSa.Models
{
    public class PhieuNhapModel
    {
        public string MaPhieuNhap { get; set; }
        public DateTime? NgayNhapCang { get; set; }
        public string TenNguoiGui { get; set; }
        public string TenKho { get; set; }
        public List<HangHoaModel> Items { get; set; }

        public PhieuNhapModel()
        {
            Items = new List<HangHoaModel>();
        }
    }
}