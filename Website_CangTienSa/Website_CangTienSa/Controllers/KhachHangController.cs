using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Website_CangTienSa.Controllers
{
    public class KhachHangController : Controller
    {
        // GET: KhachHang
        public ActionResult Index_KhachHang()
        {
            ViewBag.Title = "Index_KhachHang";
            ViewBag.ActiveSidebar = "DangKyGuiHang";
            return View();
        }

        public ActionResult XemChiTiet()
        {
            ViewBag.Title = "XemChiTiet";
            ViewBag.ActiveSidebar = "XemChiTietVaLichSu";
            return View();
        }
    }
}