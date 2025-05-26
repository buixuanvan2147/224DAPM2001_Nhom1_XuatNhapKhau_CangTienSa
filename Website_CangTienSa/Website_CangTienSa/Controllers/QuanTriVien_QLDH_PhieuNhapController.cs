using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_CangTienSa.Models;

namespace Website_CangTienSa.Controllers
{
    public class QuanTriVien_QLDH_PhieuNhapController : Controller
    {
        private readonly XuatNhapHangTaiCangTienSaEntities db = new XuatNhapHangTaiCangTienSaEntities();
        // GET: QuanTriVien_QLDH_PhieuNhap/QTV_QLDH_ChiTietPhieuNhap/PN001
        public ActionResult QTV_QLDH_ChiTietPhieuNhap(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound("Không tìm thấy mã phiếu nhập.");
            }

            // 1. Lấy thông tin Phiếu Nhập chính
            var phieuNhap = db.phieuNhaps
                                     .FirstOrDefault(pn => pn.maPhieuNhap == id);

            if (phieuNhap == null)
            {
                return HttpNotFound($"Không tìm thấy phiếu nhập với mã: {id}");
            }

            // 2. Lấy thông tin Đơn Hàng liên quan ( eager loading với Include nếu dùng Entity Framework)
            // Nếu không dùng Include, bạn phải tải riêng DonHang:
            var donHang = db.donHangs
                                    .FirstOrDefault(dh => dh.maDonHang == phieuNhap.maDonHang);

            // 3. Lấy danh sách Chi Tiết Phiếu Nhập liên quan
            // Sử dụng LINQ Join hoặc Include để lấy các thông tin liên quan từ ChiTietPhieuNhap, Container, DanhMucContainer, ChiTietKho
            var chiTietPhieuNhaps = (from ctpn in db.chiTietPhieuNhaps
                                     join c in db.containers on ctpn.maContainer equals c.maContainer
                                     join dmc in db.danhMucContainers on c.maDanhMucContainer equals dmc.maDanhMucContainer
                                     join ctk in db.chiTietKhoes on c.maChiTietKho equals ctk.maChiTietKho
                                     where ctpn.maPhieuNhap == id
                                     select new ChiTietPhieuNhapViewModel // Tạo một ViewModel để chứa tất cả thông tin bạn muốn hiển thị
                                     {
                                         MaChiTietPhieuNhap = ctpn.maChiTietPhieuNhap,
                                         MoTaChiTietPN = ctpn.moTa, // Mô tả riêng của chi tiết phiếu nhập
                                         MaContainer = c.maContainer,
                                         SoHieuContainer = c.soHieu,
                                         TenDanhMucContainer = dmc.tenDanhMucContainer,
                                         TrongTaiContainer = c.trongTai,
                                         TrangThaiContainer = c.trangThaiContainer,
                                         ViTriTrongKho = c.viTriTrongKho,
                                         NgayMuaContainer = c.ngayMuaContainer,
                                         SucChuaToiDaDanhMuc = dmc.sucChuaToiDa,
                                         TrongTaiToiDaDanhMuc = dmc.trongTaiToiDa,
                                         ChieuDaiDanhMuc = dmc.chieuDai,
                                         ChieuRongDanhMuc = dmc.chieuRong,
                                         ChieuCaoDanhMuc = dmc.chieuCao,
                                         MaChiTietKho = c.maChiTietKho,
                                         TrangThaiChiTietKho = ctk.trangThaiChiTietKho,
                                         NgayCapNhatCuoiCTK = ctk.ngayCapNhatCuoi
                                         // Thêm các trường khác bạn muốn hiển thị từ Container, DanhMucContainer, ChiTietKho
                                     }).ToList();

            // Đưa dữ liệu vào ViewBag hoặc tạo một ViewModel tổng hợp
            ViewBag.PhieuNhap = phieuNhap;
            ViewBag.DonHang = donHang;
            ViewBag.ChiTietPhieuNhaps = chiTietPhieuNhaps;

            return View();
        }

        // Đảm bảo bạn có class ViewModel này trong thư mục Models của bạn
        // Hoặc bạn có thể định nghĩa nó ngay trong Controller nếu chỉ dùng nội bộ
        public class ChiTietPhieuNhapViewModel
        {
            public string MaChiTietPhieuNhap { get; set; }
            public string MoTaChiTietPN { get; set; }
            public string MaContainer { get; set; }
            public string SoHieuContainer { get; set; }
            public string TenDanhMucContainer { get; set; }
            public double TrongTaiContainer { get; set; }
            public string TrangThaiContainer { get; set; }
            public string ViTriTrongKho { get; set; }
            public DateTime? NgayMuaContainer { get; set; }

            // Thông tin từ DanhMucContainer
            public double SucChuaToiDaDanhMuc { get; set; }
            public double TrongTaiToiDaDanhMuc { get; set; }
            public double ChieuDaiDanhMuc { get; set; }
            public double ChieuRongDanhMuc { get; set; }
            public double ChieuCaoDanhMuc { get; set; }

            // Thông tin từ ChiTietKho
            public string MaChiTietKho { get; set; }
            public string TrangThaiChiTietKho { get; set; }
            public DateTime NgayCapNhatCuoiCTK { get; set; }
        }
    }
}