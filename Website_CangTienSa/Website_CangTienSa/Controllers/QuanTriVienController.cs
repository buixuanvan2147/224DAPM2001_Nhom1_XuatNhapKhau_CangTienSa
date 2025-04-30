using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Website_CangTienSa.Controllers
{
    public class QuanTriVienController : Controller
    {
        // GET: QuanTriVien

        public ActionResult TongQuan_QuanTriVien()
        {
            ViewBag.Title = "TongQuan_QuanTriVien";
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
            return View();
        }
    }
}