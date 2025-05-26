using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_CangTienSa.Models;
using System.Data.Entity;
using System.Web.Configuration;

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
            var phieuNhap = db.phieuNhaps.FirstOrDefault(pn => pn.maPhieuNhap == id);

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

        public ActionResult QTV_QLDH_XoaPhieuNhap(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound("Không tìm thấy mã phiếu nhập.");
            }

            var phieuNhap = db.phieuNhaps
                    .Include(pn => pn.donHang) // Nếu có mối quan hệ và bạn muốn hiển thị thông tin đơn hàng
                    .FirstOrDefault(pn => pn.maPhieuNhap == id);

            if (phieuNhap == null)
            {
                return HttpNotFound($"Không tìm thấy phiếu nhập với mã: {id}");
            }

            return View(phieuNhap); // Truyền đối tượng phiếu nhập sang View để hiển thị thông tin cần xác nhận
        }
        [HttpPost, ActionName("QTV_QLDH_XoaPhieuNhap")]
        [ValidateAntiForgeryToken] // Nên thêm để chống tấn công CSRF
        public ActionResult XacNhanXoaPhieuNhap(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound("Không tìm thấy mã phiếu nhập để xóa.");
            }

            try
            {
                // Kiểm tra các ràng buộc khóa ngoại (Foreign Key Constraints)
                // Trong trường hợp của bạn, PhieuNhap có ChiTietPhieuNhap phụ thuộc vào nó.
                // Do đó, bạn phải xóa ChiTietPhieuNhap trước khi xóa PhieuNhap.

                // 1. Tìm và xóa các ChiTietPhieuNhap liên quan
                var chiTietPhieuNhapsCanXoa = db.chiTietPhieuNhaps
                        .Where(ctpn => ctpn.maPhieuNhap == id)
                        .ToList();
                if (chiTietPhieuNhapsCanXoa.Any())
                {
                    db.chiTietPhieuNhaps.RemoveRange(chiTietPhieuNhapsCanXoa);
                }

                // 2. Sau khi đã xóa các ChiTietPhieuNhap, tiến hành xóa PhieuNhap
                var phieuNhapCanXoa = db.phieuNhaps.Find(id);
                if (phieuNhapCanXoa == null)
                {
                    return HttpNotFound($"Không tìm thấy phiếu nhập với mã: {id} để xóa.");
                }

                db.phieuNhaps.Remove(phieuNhapCanXoa);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Phiếu nhập đã được xóa thành công!";
                return RedirectToAction("QuanLyDonHang_QuanTriVien", "QuanTriVien"); 
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
            {
                // Xử lý lỗi nếu có ràng buộc toàn vẹn dữ liệu khác
                // Ví dụ: Phiếu nhập đang có liên kết với dữ liệu khác mà không thể xóa
                ModelState.AddModelError("", "Không thể xóa phiếu nhập này vì có dữ liệu liên quan khác.");
                System.Diagnostics.Debug.WriteLine("Lỗi khi xóa phiếu nhập: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner Exception: " + ex.InnerException.Message);
                }
                var phieuNhap = db.phieuNhaps.Find(id); // Tải lại để hiển thị lại View
                return View("QTV_QLDH_XoaPhieuNhap", phieuNhap); // Hiển thị lại trang xác nhận với lỗi
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("", "Đã xảy ra lỗi trong quá trình xóa: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("General Error: " + ex.Message);
                var phieuNhap = db.phieuNhaps.Find(id);
                return View("QTV_QLDH_XoaPhieuNhap", phieuNhap);
            }
        }

        // Model dùng cho chi tiết phiếu nhập
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