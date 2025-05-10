using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Website_CangTienSa.Models;

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
            return View();
        }

        public ActionResult QuanLyTaiKhoan_QuanTriVien()
        {
            ViewBag.Title = "QuanLyTaiKhoan_QuanTriVien";
            ViewBag.ActiveSidebar = "QuanLyTaiKhoan";
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