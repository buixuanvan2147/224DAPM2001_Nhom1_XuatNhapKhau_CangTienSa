using System;
using System.Collections.Generic;
using System.Linq;
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

        public ActionResult ChiTietDonHang(string maDonHang)
        {
            var model = _donHangDAO.GetChiTietDonHang(maDonHang);
            var containers = _donHangDAO.GetDanhSachContainerRong();

            ViewBag.ContainerRong = containers.Select(c => new SelectListItem
            {
                Value = c.MaContainer,
                Text = $"{c.MaContainer} - {c.TenKho} ({c.TenLoaiKho})"
            }).ToList();

            return View(model);
        }

        public ActionResult Index_NhanVienKhoBai()
        {
            var donHangList = _donHangDAO.GetDonHangDangVanChuyen();
            return View(donHangList);
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
        [HttpPost]
        public ActionResult GanDonHangVaoContainer(string maDonHang, string maContainer)
        {
            try
            {
                // Trim và validate độ dài
                maDonHang = maDonHang?.Trim() ?? "";
                maContainer = maContainer?.Trim() ?? "";

                if (string.IsNullOrEmpty(maDonHang) || string.IsNullOrEmpty(maContainer))
                {
                    TempData["ErrorMessage"] = "Mã đơn hàng và container không được để trống";
                    return RedirectToAction("ChiTietDonHang", new { maDonHang });
                }

                if (maDonHang.Length > 10 || maContainer.Length > 10)
                {
                    TempData["ErrorMessage"] = "Mã đơn hàng hoặc container không được vượt quá 10 ký tự";
                    return RedirectToAction("ChiTietDonHang", new { maDonHang });
                }

                bool success = _donHangDAO.GanDonHangVaoContainer(maDonHang, maContainer);

                if (success)
                    TempData["SuccessMessage"] = "✅ Thêm vào container thành công";
                else
                    TempData["ErrorMessage"] = "❌ Thêm vào container thất bại. Container có thể không tồn tại hoặc đã được sử dụng";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"❌ Lỗi hệ thống: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Lỗi khi thêm vào container: {ex}");
            }

            return RedirectToAction("ChiTietDonHang", new { maDonHang });
        }
    }
}