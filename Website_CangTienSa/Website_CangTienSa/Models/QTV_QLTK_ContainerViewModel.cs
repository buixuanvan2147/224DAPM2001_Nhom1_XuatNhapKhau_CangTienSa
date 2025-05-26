using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website_CangTienSa.Models
{
    public class QTV_QLTK_ContainerViewModel
    {
        public string MaContainer { get; set; }
        public string TenMaDanhMucContainer { get; set; }
        public string MaChiTietKho { get; set; }
        public string SoHieu { get; set; }
        public string TrangThaiContainer { get; set; }
        public string ViTriTrongKho { get; set; }
        public DateTime? NgayMuaContainer { get; set; }
        public double TrongTai { get; set; }
    }
}