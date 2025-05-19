using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_CangTienSa.Models;

namespace Website_CangTienSa.Controllers
{
    public class NhanVienKeToanController : Controller
    {
        private readonly XuatNhapHangTaiCangTienSaEntities db = new XuatNhapHangTaiCangTienSaEntities();

        public ActionResult Index_NhanVienKeToan()
        {
            var listDonHang = db.donHangs
                .Select(d => new DonHangViewModel
                {
                    MaDonHang = d.maDonHang,
                    TenKhachHang = d.khachHang.tenCongTy,
                    NgayTaoDon = d.ngayTaoDonHang,
                    TongTien = (float)d.tongTien,
                    TrangThai = d.trangThaiThanhToan,
                    ThoiGianThanhToan = d.ngayTaoDonHang
                }).ToList();

            return View(listDonHang);
        }

        [HttpPost]
        public JsonResult XacNhanThanhToan(string maDonHang)
        {
            try
            {
                var donHang = db.donHangs.FirstOrDefault(d => d.maDonHang == maDonHang);

                if (donHang != null && donHang.trangThaiThanhToan == "Chưa thanh toán")
                {
                    donHang.trangThaiThanhToan = "Đã thanh toán";
                 //   donHang.ngayThanhToan = DateTime.Now;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Cập nhật trạng thái thành công." });
                }

                return Json(new { success = false, message = "Đơn hàng không tồn tại hoặc đã được thanh toán." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message });
            }
        }
    }
}
