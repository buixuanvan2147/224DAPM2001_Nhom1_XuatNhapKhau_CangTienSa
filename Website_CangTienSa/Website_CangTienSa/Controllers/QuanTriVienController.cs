using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Website_CangTienSa.Models;
using System.Data.Entity;
using System.Linq.Dynamic;


namespace Website_CangTienSa.Controllers
{
    public class QuanTriVienController : Controller
    {
        private readonly XuatNhapHangTaiCangTienSaEntities db = new XuatNhapHangTaiCangTienSaEntities();

        // GET: QuanTriVien

        public ActionResult Index_QuanTriVien()
        {
            ViewBag.Title = "Index_QuanTriVien";
            ViewBag.ActiveSidebar = "TongQuan";

            int tongSoTKKhachHang = db.khachHangs.Count();
            ViewBag.TongSoTaiKhoanKhachHang = tongSoTKKhachHang;

            int tongSoTKNhanVien = db.nhanViens.Count();
            ViewBag.TongSoTaiKhoanNhanVien = tongSoTKNhanVien;

            int TongSoKhachHangChoDuyet = db.khachHangs
            .Count(kh => kh.trangThaiTaiKhoanKhachHang == "Chờ duyệt");
            ViewBag.TongSoKhachHangChoDuyet = TongSoKhachHangChoDuyet;

            int TongSoDonHangDangXuLy = db.donHangs
            .Count(kh => kh.trangThaiDonHang == "Đang xử lý");
            ViewBag.TongSoDonHangDangXuLy = TongSoDonHangDangXuLy;

            return View();
        }

        public ActionResult QuanLyTaiKhoan_QuanTriVien()
        {
            ViewBag.Title = "Quản Lý Tài Khoản";
            ViewBag.ActiveSidebar = "Quản Lý Tài Khoản";

            // Lấy danh sách khách hàng (không thay đổi)
            var allKhachHang = db.khachHangs.ToList();
            ViewBag.KhachHangs = allKhachHang;

            // Lấy danh sách vai trò nhân viên (không thay đổi)
            var allVaiTroNhanVien = db.vaiTroNhanViens.Select(vt => vt.tenLoaiNhanVien).Distinct().ToList();
            ViewBag.Roles = allVaiTroNhanVien;

            // Lấy danh sách nhân viên sử dụng NhanVienViewModel
            var allNhanVien = db.nhanViens
                .Include(nv => nv.vaiTroNhanVien)
                .Select(nv => new QTV_QLTK_NhanVienViewModel // Sử dụng QTV_QLTK_NhanVienViewModel tự tạo
                {
                    MaNhanVien = nv.maNhanVien,
                    AnhDaiDienNhanVienUrl = nv.anhDaiDienNhanVienUrl, // Đảm bảo tên thuộc tính khớp
                    TenDangNhap = nv.tenDangNhap,
                    MatKhau = nv.matKhau,
                    TenHienThi = nv.tenHienThi,
                    SdtNhanVien  = nv.sdtNhanVien,
                    DiaChi = nv.diaChi,
                    Email = nv.email,
                    TrangThaiTaiKhoanNhanVien = nv.trangThaiTaiKhoanNhanVien,
                    ThoiGianDangNhapGanNhat = nv.thoiGianDangNhapGanNhat,
                    TenLoaiNhanVien = nv.vaiTroNhanVien.tenLoaiNhanVien
                })
                .ToList();
            ViewBag.NhanViens = allNhanVien;

            // Lấy ánh xạ vai trò cho modal phân quyền
            // Tạo ánh xạ vai trò cho modal phân quyền dưới dạng Dictionary
            var roleMappingDict = db.vaiTroNhanViens.ToDictionary(
                v => v.tenLoaiNhanVien,
                v => v.maVaiTroNhanVien
            );
            ViewBag.RoleMapping = roleMappingDict;

            return View();
        }

        public ActionResult QuanLyDonHang_QuanTriVien()
        {
            ViewBag.Title = "QuanLyDonHang_QuanTriVien";
            ViewBag.ActiveSidebar = "QuanLyDonHang";

            // Danh mục container để làm select option cho container
            // Đã thay đổi "maDanhMucContainer" thành "tenDanhMucContainer" cho cả valueField và textField
            ViewBag.dmContainers = new SelectList(db.danhMucContainers, "tenDanhMucContainer", "tenDanhMucContainer");

            // Danh sách container
            var allConTaiNer = db.containers
                .Include(ctn => ctn.danhMucContainer)
                .Select(ctn => new QTV_QLDH_ContainerViewModel
                {
                    MaContainer = ctn.maContainer,
                    TenMaDanhMucContainer = ctn.danhMucContainer.tenDanhMucContainer,
                    MaChiTietKho = ctn.maChiTietKho,
                    SoHieu = ctn.soHieu,
                    TrangThaiContainer = ctn.trangThaiContainer,
                    ViTriTrongKho = ctn.viTriTrongKho,
                    NgayMuaContainer = ctn.ngayMuaContainer,
                    TrongTai = ctn.trongTai
                })
                .ToList();
            ViewBag.ConTaiNers = allConTaiNer;

            // Danh sách phiếu nhập
            var allPhieuNhap = db.phieuNhaps.ToList();
            ViewBag.PhieuNhaps = allPhieuNhap;

            // Danh sách phiếu xuất
            var allPhieuXuat = db.phieuXuats.ToList();
            ViewBag.PhieuXuats = allPhieuXuat;

            // Trạng thái đơn hàng để làm select option cho đơn hàng
            // Đã thêm .Trim() để loại bỏ khoảng trắng thừa từ DB
            var ttdh = db.donHangs.Select(d => d.trangThaiDonHang.Trim()).Distinct().ToList();
            ViewBag.trangThaiDonHangs = ttdh;

            // Trạng thái thanh toán để làm select option cho đơn hàng
            // Đã thêm .Trim() để loại bỏ khoảng trắng thừa từ DB
            var tttt = db.donHangs.Select(d => d.trangThaiThanhToan.Trim()).Distinct().ToList();
            ViewBag.trangThaiThanhToans = tttt;

            // Danh sách đơn hàng
            var allDonHang = db.donHangs.ToList();
            ViewBag.DonHangs = allDonHang;

            return View();
        }
        public ActionResult ThongKeBaoCao_QuanTriVien()
        {
            ViewBag.Title = "ThongKeBaoCao_QuanTriVien";
            ViewBag.ActiveSidebar = "ThongKeBaoCao";

            double donHangDaHoanThanh = db.donHangs.Count(d => d.trangThaiDonHang == "Hoàn thành");
            double tongSoLuongDonHang = db.donHangs.Count();

            double tongTonKho = db.khoes.Sum(k => k.tonKho);
            double tongSucChuaToiDa = db.khoes.Sum(k => k.sucChuaToiDa);

            // Tính mốc thời gian 6 tháng trước
            // Sử dụng DateTime.Today để lấy ngày hiện tại (không tính giờ)
            // Hoặc DateTime.Now nếu bạn muốn tính chính xác đến giây hiện tại
            DateTime sixMonthsAgo = DateTime.Today.AddMonths(-6);

            var qtv_tongQuan = new QTV_ThongKeBaoCao_TongQuanViewModel
            {
                TongSoKhachHang = db.khachHangs.Count(),
                TongSoKhachHangMoi = db.khachHangs.Count(kh =>
                                    // Sử dụng DbFunctions.AddMonths cho EF6 hoặc EF.Functions.DateAdd cho EF Core
                                    // Ở đây, tôi sẽ dùng cách tạo mốc thời gian trước để tránh vấn đề dịch trực tiếp trong LINQ
                                    kh.ngayDangKy >= sixMonthsAgo &&
                                    kh.trangThaiTaiKhoanKhachHang == "Đã duyệt"), // Khách Hàng mới trong 6 tháng vừa qua
                TongSoNhanVien = db.nhanViens.Count(),
                TongSoDonHang = db.donHangs.Count(),
                TongSoDoanhThu6ThangQua = db.donHangs
                                        .Where(d => d.trangThaiDonHang == "Đã hoàn thành" &&
                                                    d.trangThaiThanhToan == "Đã thanh toán" &&
                                                    d.ngayNhapCang.HasValue && // Đảm bảo ngayNhapCang không null
                                                    d.ngayNhapCang.Value >= sixMonthsAgo) // Lọc 6 tháng gần nhất
                                        .Sum(d => (float?)d.tongTien) ?? 0,
                TongDonHangDangLuuKho = db.donHangs.Count(d =>
                                            d.ngayNhapCang != null && // Đã nhập kho
                                            d.ngayXuatCang == null && // Chưa xuất kho
                                            (d.trangThaiDonHang == "Đang xử lý" || d.trangThaiDonHang == "Đang yêu cầu")),
                TongSoContainerHienCo = db.containers.Count(c => c.trangThaiContainer == "Hoạt động"),
                TyLeDonHangHoanThanh = tongSoLuongDonHang == 0 ? 0 : (donHangDaHoanThanh / tongSoLuongDonHang) * 100,
                SoPhieuNhapMoi6ThangQua = db.donHangs.Count(p => p.ngayNhapCang.Value >= sixMonthsAgo),
                SoPhieuXuatMoi6ThangQua = db.phieuXuats.Count(p => p.ngayXuatKho >= sixMonthsAgo),
                CongXuatSuDungKho = tongSucChuaToiDa == 0 ? 0 : (tongTonKho / tongSucChuaToiDa) * 100
            };

            ViewBag.TongQuan = qtv_tongQuan;

            return View();
        }
        public ActionResult TrangCaNhan_QuanTriVien()
        {
            ViewBag.Title = "TrangCaNhan_QuanTriVien";
            ViewBag.ActiveSidebar = "TrangCaNhan";

            string tenDangNhap = User.Identity.Name;

            var nhanVienData = db.nhanViens
                .Where(nv => nv.tenDangNhap == tenDangNhap && nv.vaiTroNhanVien.tenLoaiNhanVien == "Quản trị viên")
                .FirstOrDefault();

            QuanTriVienViewModel quanTriVienViewModel = null;

            if (nhanVienData != null)
            {
                quanTriVienViewModel = new QuanTriVienViewModel
                {
                    MaNhanVien = nhanVienData.maNhanVien,
                    TenDangNhap = nhanVienData.tenDangNhap,
                    TenHienThi = nhanVienData.tenHienThi,
                    Email = nhanVienData.email,
                    SdtNhanVien = nhanVienData.sdtNhanVien,
                    DiaChi = nhanVienData.diaChi,
                    ThoiGianDangNhapGanNhat = nhanVienData.thoiGianDangNhapGanNhat,
                    AnhDaiDienNhanVienUrl = nhanVienData.anhDaiDienNhanVienUrl
                };

                // Kiểm tra nếu AnhDaiDienNhanVienUrl là null hoặc rỗng, thì gán giá trị mặc định
                if (string.IsNullOrEmpty(quanTriVienViewModel.AnhDaiDienNhanVienUrl))
                {
                    quanTriVienViewModel.AnhDaiDienNhanVienUrl = "~/Content/img/default_user_image.png";
                }
            }
            else
            {
                return Content("Không tìm thấy thông tin quản trị viên.");
            }

            return View(quanTriVienViewModel);
        }

        public ActionResult DangXuat_QuanTriVien()
        {
            FormsAuthentication.SignOut(); // Xóa cookie xác thực
            Session.Abandon(); // Hủy bỏ toàn bộ session
            return RedirectToAction("Index", "Home"); // Chuyển hướng về trang chủ
        }

    }
}