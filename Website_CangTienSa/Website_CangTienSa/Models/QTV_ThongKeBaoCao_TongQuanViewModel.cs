using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Website_CangTienSa.Models
{
    public class QTV_ThongKeBaoCao_TongQuanViewModel
    {
        public int TongSoKhachHang { get; set; }
        public int TongSoKhachHangMoi { get; set; } //khách Hàng mới đã được duyệt trong 6 tháng vừa qua
        public int TongSoNhanVien { get; set; }
        public int TongSoDonHang { get; set; }
        public float TongSoDoanhThu6ThangQua { get; set; } //Doanh thu 6 tháng vừa qua
        public int TongDonHangDangLuuKho { get; set; }
        public int TongSoContainerHienCo { get; set; }
        public double TyLeDonHangHoanThanh { get; set; }
        public int DonHangChoXacNhan { get; set; } // Trạng thái đang yêu cầu
        public int SoPhieuNhapMoi6ThangQua { get; set; }
        public int SoPhieuXuatMoi6ThangQua { get; set; }
        public double CongXuatSuDungKho { get; set; } 
    }
}