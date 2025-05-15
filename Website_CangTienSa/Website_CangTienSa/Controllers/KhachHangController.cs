using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Website_CangTienSa.Models;

namespace Website_CangTienSa.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly XuatNhapHangTaiCangTienSaEntities db = new XuatNhapHangTaiCangTienSaEntities();

        // GET: KhachHang
        public ActionResult Index_KhachHang()
        {
            // Lấy thông tin khách hàng từ session
            var khachHang = Session["KhachHang"] as khachHang;

            if (khachHang != null)
            {
                // Truyền thông tin khách hàng vào View
                ViewBag.TenKhachHang = khachHang.tenKhachHang;
                ViewBag.TenCongTy = khachHang.tenCongTy;
                ViewBag.MaSoThue = khachHang.maSoThueCongTy; // Mã số thuế công ty

                return View(); // Chuyển đến view DangKyGuiHang
            }
            else
            {
                return RedirectToAction("DangNhap", "Home"); // Nếu không có thông tin, chuyển hướng về trang đăng nhập
            }
        }

        // GET: XemChiTiet
        public ActionResult XemChiTiet()
        {
            ViewBag.Title = "XemChiTiet";
            ViewBag.ActiveSidebar = "XemChiTietVaLichSu";
            return View();
        }

        // GET: Thông tin cá nhân
        public ActionResult ThongTinCaNhan()
        {
            var khachHang = Session["KhachHang"] as khachHang;

            if (khachHang != null)
            {
                return View(khachHang);
            }
            else
            {
                return RedirectToAction("DangNhap", "Home");
            }
        }

        // POST: Cập nhật email
        [HttpPost]
        public ActionResult UpdateEmail(string email)
        {
            var khachHang = Session["KhachHang"] as khachHang;
            if (khachHang != null)
            {
                // Cập nhật email trong database
                var dbKhachHang = db.khachHangs.FirstOrDefault(k => k.tenDangNhap == khachHang.tenDangNhap);
                if (dbKhachHang != null)
                {
                    dbKhachHang.email = email;
                    db.SaveChanges(); // Lưu vào cơ sở dữ liệu

                    // Cập nhật lại session với dữ liệu mới
                    khachHang.email = email;
                    Session["KhachHang"] = khachHang;
                }
            }
            return RedirectToAction("ThongTinCaNhan");
        }

        // POST: Cập nhật số điện thoại
        [HttpPost]
        public ActionResult UpdatePhone(string phone)
        {
            var khachHang = Session["KhachHang"] as khachHang;
            if (khachHang != null)
            {
                // Cập nhật số điện thoại trong database
                var dbKhachHang = db.khachHangs.FirstOrDefault(k => k.tenDangNhap == khachHang.tenDangNhap);
                if (dbKhachHang != null)
                {
                    dbKhachHang.sdtKhachHang = phone;
                    db.SaveChanges(); // Lưu vào cơ sở dữ liệu

                    // Cập nhật lại session với dữ liệu mới
                    khachHang.sdtKhachHang = phone;
                    Session["KhachHang"] = khachHang;
                }
            }
            return RedirectToAction("ThongTinCaNhan");
        }

        // POST: Cập nhật địa chỉ
        [HttpPost]
        public ActionResult UpdateAddress(string address)
        {
            var khachHang = Session["KhachHang"] as khachHang;
            if (khachHang != null)
            {
                // Cập nhật địa chỉ trong database
                var dbKhachHang = db.khachHangs.FirstOrDefault(k => k.tenDangNhap == khachHang.tenDangNhap);
                if (dbKhachHang != null)
                {
                    dbKhachHang.diaChiLienLac = address;
                    db.SaveChanges(); // Lưu vào cơ sở dữ liệu

                    // Cập nhật lại session với dữ liệu mới
                    khachHang.diaChiLienLac = address;
                    Session["KhachHang"] = khachHang;
                }
            }
            return RedirectToAction("ThongTinCaNhan");
        }


        public ActionResult DangXuat()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}