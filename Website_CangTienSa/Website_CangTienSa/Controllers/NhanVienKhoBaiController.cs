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
            var donHangDangVanChuyen = _donHangDAO.GetDonHangDangVanChuyen();
            var donHangXuatKho = _donHangDAO.GetDonHangXuatKhau();

            var viewModel = new Tuple<List<DonHangModel>, List<DonHangModel>>(donHangDangVanChuyen, donHangXuatKho);
            return View(viewModel);
        }
        public ActionResult ChiTietDonHangXuatKho(string maDonHang)
        {
            var model = _donHangDAO.GetChiTietDonHang(maDonHang);
            return View(model);
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

        [HttpPost]
        public ActionResult ConfirmTransport(string maDonHang)
        {
            try
            {
                // Validate input
                if (string.IsNullOrEmpty(maDonHang))
                {
                    TempData["ErrorMessage"] = "Mã đơn hàng không hợp lệ";
                    return RedirectToAction("Index_NhanVienKhoBai");
                }

                // Cập nhật trạng thái đơn hàng và ngày xuất cảng
                bool success = _donHangDAO.UpdateTrangThaiVaNgayXuatCang(maDonHang, "Hoàn thành", DateTime.Now);

                if (success)
                {
                    TempData["SuccessMessage"] = "Đã xác nhận đơn hàng thành công";
                }
                else
                {
                    TempData["ErrorMessage"] = "Xác nhận đơn hàng thất bại";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi hệ thống: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Lỗi khi xác nhận đơn hàng: {ex}");
            }

            return RedirectToAction("Index_NhanVienKhoBai");
        }
        public ActionResult Index_DonHangXuatKho()
        {
            var donHangList = _donHangDAO.GetDonHangXuatKhau();
            return View(donHangList);
        }
        public ActionResult LichSuChonContainer()
        {
            var containers = _donHangDAO.GetLichSuChonContainer();
            return View(containers);
        }

        [HttpPost]
        public ActionResult KhoiPhucContainer(string maContainer)
        {
            try
            {
                maContainer = maContainer?.Trim() ?? "";
                if (string.IsNullOrEmpty(maContainer))
                {
                    TempData["ErrorMessage"] = "Mã container không hợp lệ";
                    return RedirectToAction("LichSuChonContainer");
                }

                if (maContainer.Length > 10)
                {
                    TempData["ErrorMessage"] = "Mã container không được vượt quá 10 ký tự";
                    return RedirectToAction("LichSuChonContainer");
                }

                bool success = _donHangDAO.KhoiPhucContainer(maContainer);
                if (success)
                {
                    TempData["SuccessMessage"] = "✅ Khôi phục container thành công";
                }
                else
                {
                    TempData["ErrorMessage"] = "❌ Khôi phục container thất bại. Container có thể không tồn tại hoặc không ở trạng thái 'Đang sử dụng'";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"❌ Lỗi hệ thống: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Lỗi khi khôi phục container: {ex}");
            }

            return RedirectToAction("LichSuChonContainer");
        }
    }
}
