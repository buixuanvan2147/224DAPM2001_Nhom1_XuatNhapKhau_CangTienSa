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
            return View();
        }

        public ActionResult ThongKeBaoCao_QuanTriVien()
        {
            ViewBag.Title = "ThongKeBaoCao_QuanTriVien";
            ViewBag.ActiveSidebar = "ThongKeBaoCao";
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