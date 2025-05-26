//using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_CangTienSa.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Web.Security;


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
                    TenKhachHang = d.khachHang.tenKhachHang,
                    TenCongTy = d.khachHang.tenCongTy,
                    NgayTaoDon = d.ngayTaoDonHang,
                    TongTien = (float)d.tongTien,
                    TrangThai = d.trangThaiThanhToan,
                    ThoiGianThanhToan = d.ngayTaoDonHang
                }).ToList();

            return View(listDonHang);
        }

        public ActionResult DangXuat()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
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
                    donHang.trangThaiDonHang = "Đang vận chuyển";
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


        [HttpPost]
        public ActionResult XuatBienLai(string maDonHang)
        {
            var donHang = db.donHangs
                .Where(d => d.maDonHang == maDonHang)
                .Select(d => new
                {
                    MaDonHang = d.maDonHang,
                    TenKhachHang = d.khachHang.tenKhachHang,
                    TenCongTy = d.khachHang.tenCongTy,
                    TongTien = d.tongTien,
                    ThoiGianThanhToan = d.ngayTaoDonHang
                })
                .FirstOrDefault();

            if (donHang == null)
            {
                return new HttpStatusCodeResult(404, "Đơn hàng không tồn tại.");
            }

            // Tạo PDF bằng iTextSharp
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                // Tiêu đề
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                Paragraph title = new Paragraph("BIÊN LAI THANH TOÁN", titleFont);
                title.Alignment = Element.ALIGN_CENTER;
                document.Add(title);
                document.Add(new Paragraph("\n"));

                // Thông tin đơn hàng
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                document.Add(new Paragraph($"Mã Đơn Hàng: {donHang.MaDonHang}", normalFont));
                document.Add(new Paragraph($"Tên Khách Hàng: {donHang.TenKhachHang}", normalFont));
                document.Add(new Paragraph($"Tên Công Ty: {donHang.TenCongTy}", normalFont));
                document.Add(new Paragraph($"Tổng Tiền: {string.Format("{0:N0} VNĐ", donHang.TongTien)}", normalFont));
                document.Add(new Paragraph($"Ngày Thanh Toán: {donHang.ThoiGianThanhToan.ToString("dd/MM/yyyy")}", normalFont));
                document.Add(new Paragraph("\nCảm ơn quý khách đã sử dụng dịch vụ của chúng tôi!", normalFont));

                document.Close();

                byte[] bytes = ms.ToArray();
                return File(bytes, "application/pdf", $"BienLai_{donHang.MaDonHang}.pdf");
            }
        }





    }
}
