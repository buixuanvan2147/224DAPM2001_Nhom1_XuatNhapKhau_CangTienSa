using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Website_CangTienSa.Models;
using System.Data.Entity; // Đảm bảo có using này để hỗ trợ Include()

namespace Website_CangTienSa.Controllers
{
    public class NhanVienXuatKhoController : Controller
    {
        private readonly XuatNhapHangTaiCangTienSaEntities db = new XuatNhapHangTaiCangTienSaEntities();

        // GET: Trang danh sách
        public ActionResult Index_NhanVienXuatKho()
        {
            var donHangs = db.donHangs
                .Include(d => d.khachHang)
                .ToList();
            return View(donHangs);
        }

        // GET: Chi tiết đơn hàng
        public ActionResult ChiTietDonHang(string id)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            var donHang = db.donHangs
                .Include(d => d.khachHang)
                .FirstOrDefault(d => d.maDonHang == id);

            if (donHang == null)
                return HttpNotFound();

            var chiTiet = db.chiTietDonHangs
                .Where(c => c.maDonHang == id)
                .ToList();

            var viewModel = new DonHangChiTietViewModel
            {
                DonHang = donHang,
                ChiTietDonHang = chiTiet
            };

            return View(viewModel);
        }


    }
}
