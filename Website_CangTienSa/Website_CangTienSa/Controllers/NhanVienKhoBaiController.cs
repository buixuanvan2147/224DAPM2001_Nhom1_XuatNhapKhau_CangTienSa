using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Website_CangTienSa.DAO;
using Website_CangTienSa.Models;

namespace Website_CangTienSa.Controllers
{
    public class NhanVienKhoBaiController : Controller
    {
        private readonly DonHangDAO _donHangDAO;

        public NhanVienKhoBaiController()
        {
            _donHangDAO = new DonHangDAO();
        }

        public ActionResult Index_NhanVienKhoBai()
        {
            var donHangList = _donHangDAO.GetDonHangDangVanChuyen();
            return View(donHangList);
        }

        public ActionResult ChiTietDonHang(string maDonHang)
        {
            var model = _donHangDAO.GetChiTietDonHang(maDonHang);
            return View(model);
        }

        [HttpPost]
        public JsonResult ConfirmTransport(string maPhieuXuat)
        {
            string nguoiXacNhan = User.Identity.Name;
            bool success = _donHangDAO.ConfirmTransport(maPhieuXuat, nguoiXacNhan);
            return Json(new
            {
                success,
                message = success ? "✅ Đã xác nhận nhập kho." : "❌ Không tìm thấy đơn hàng."
            });
        }
    }
}


    