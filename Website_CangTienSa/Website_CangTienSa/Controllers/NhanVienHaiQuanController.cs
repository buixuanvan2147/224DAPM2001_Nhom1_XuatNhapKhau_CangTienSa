using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Website_CangTienSa.Models;

namespace Website_CangTienSa.Controllers
{
    public class NhanVienHaiQuanController : Controller
    {
        // GET: NhanVienHaiQuan
        private readonly XuatNhapHangTaiCangTienSaEntities db = new XuatNhapHangTaiCangTienSaEntities();
        public ActionResult Index_NhanVienHaiQuan()
        {
            var listDH = db.donHangs
                .ToList() // Lấy ra toàn bộ từ DB trước
                .Select(d => new DonHangModel
                {
                    MaDonHang = d.maDonHang,
                    LoaiHangHoa= string.Join(", ",
                d.chiTietDonHangs
                .Select(ct => ct.hangHoa.danhMucHangHoa.tenDanhMucHangHoa)
                .Distinct()
                .ToList()
            ),
                    ThoiGianLuuTru = d.thoiGianLuuTru ,
                    NgayNhapHang = d.ngayNhapCang ?? DateTime.MinValue, // tránh lỗi null
                    TrangThaiPhanLoai = d.moTa ?? "Chưa phân loại",
                    TrangThaiDonHang = d.trangThaiDonHang ?? "Chưa gửi"
                }).ToList();


            return View(listDH);
        }
        public ActionResult DangXuat()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult DuyetDonHang(string maDonHang)
        {
            var donHang = db.donHangs.FirstOrDefault(d => d.maDonHang == maDonHang);

            if (donHang == null)
            {
                return HttpNotFound("Không tìm thấy đơn hàng.");
            }

            // Chỉ cập nhật nếu không phải "Đang vận chuyển"
            if (donHang.trangThaiDonHang != "Đang vận chuyển")
            {
                donHang.trangThaiDonHang = "Đang xử lý";
                db.SaveChanges();
                return Json(new { success = true, message = "Đơn hàng đã được duyệt thành Đang xử lý." });
            }

            return Json(new { success = false, message = "Đơn hàng đang vận chuyển không thể duyệt." });
        }

        public JsonResult LayChiTietDonHang(string maDonHang)
        {
            var chiTiet = db.chiTietDonHangs
                            .Where(c => c.maDonHang == maDonHang)
                            .Select(c => new {
                                c.maChiTietDonHang,
                                c.maDonHang,
                                c.maHangHoa,
                                c.soLuong,
                                c.donViTinh,
                                c.chatLuong,
                                c.donGia,
                                c.tienLuuKho,
                                c.moTa
                            }).ToList();

            return Json(chiTiet, JsonRequestBehavior.AllowGet);
        }

    }
}