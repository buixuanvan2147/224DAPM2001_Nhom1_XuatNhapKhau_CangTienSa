using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text.RegularExpressions;
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
                ViewBag.MaSoThue = khachHang.maSoThueCongTy;

                // Lấy danh sách danh mục hàng hóa từ bảng danhMucHangHoa
                var danhMucHangHoaList = db.danhMucHangHoas
                    .Select(d => new SelectListItem
                    {
                        Value = d.maDanhMucHangHoa,
                        Text = d.tenDanhMucHangHoa
                    }).ToList();
                ViewBag.DanhMucHangHoaList = danhMucHangHoaList;

                // Lấy số lượng mã hàng hóa khác nhau từ bảng hangHoa
                var soLuongHangHoa = db.hangHoas
                    .Select(h => h.maHangHoa)
                    .Distinct()
                    .Count();
                ViewBag.SoLuongHangHoa = soLuongHangHoa;

                // Không gửi toàn bộ danh sách hàng hóa ngay từ đầu
                // Dropdown maHangHoa sẽ được điền động qua AJAX
                ViewBag.HangHoaList = new List<SelectListItem>(); // Gửi danh sách rỗng để tránh lỗi

                // Lấy danh sách loại đơn hàng từ cột moTa của bảng donHang
                var loaiDonHangList = db.donHangs
                    .Select(d => d.moTa)
                    .Distinct()
                    .Select(m => new SelectListItem
                    {
                        Value = m,
                        Text = m
                    }).ToList();
                ViewBag.LoaiDonHangList = loaiDonHangList;

                return View(); // Chuyển đến view Index_KhachHang
            }
            else
            {
                return RedirectToAction("DangNhap", "Home"); // Nếu không có thông tin, chuyển hướng về trang đăng nhập
            }
        }

        // AJAX: Lấy danh sách hàng hóa theo danh mục
        public JsonResult GetHangHoaByDanhMuc(string maDanhMucHangHoa)
        {
            var hangHoaList = db.hangHoas
                .Where(h => h.maDanhMucHangHoa == maDanhMucHangHoa)
                .Select(h => new SelectListItem
                {
                    Value = h.maHangHoa,
                    Text = h.tenHangHoa
                })
                .ToList();
            return Json(hangHoaList, JsonRequestBehavior.AllowGet);
        }

        // Tạo đơn hàng
        [HttpPost]
        public ActionResult TaoDonHang(string TenNguoiNhan, string SDTNguoiNhan, string CangDichDen,
            string SoLuongDanhMucHang, List<HangHoaInput> DanhSachHangHoa, string LoaiDonHang, string MoTa)
        {
            var khachHang = Session["KhachHang"] as khachHang;
            if (khachHang == null)
            {
                return RedirectToAction("DangNhap", "Home");
            }

            // Validation
            if (string.IsNullOrEmpty(CangDichDen) || string.IsNullOrEmpty(TenNguoiNhan) ||
                string.IsNullOrEmpty(SDTNguoiNhan) || string.IsNullOrEmpty(SoLuongDanhMucHang) ||
                string.IsNullOrEmpty(LoaiDonHang) || DanhSachHangHoa == null || !DanhSachHangHoa.Any())
            {
                TempData["Error"] = "Vui lòng điền đầy đủ các trường bắt buộc.";
                return RedirectToAction("Index_KhachHang");
            }

            if (!Regex.IsMatch(SDTNguoiNhan, @"^\d{10}$"))
            {
                TempData["Error"] = "Số điện thoại người nhận phải có đúng 10 chữ số.";
                return RedirectToAction("Index_KhachHang");
            }

            int soLuongDanhMuc;
            if (!int.TryParse(SoLuongDanhMucHang, out soLuongDanhMuc) || soLuongDanhMuc <= 0)
            {
                TempData["Error"] = "Số lượng danh mục hàng không hợp lệ.";
                return RedirectToAction("Index_KhachHang");
            }

            // Kiểm tra từng mục trong DanhSachHangHoa
            foreach (var item in DanhSachHangHoa)
            {
                if (string.IsNullOrEmpty(item.maDanhMucHangHoa) || string.IsNullOrEmpty(item.maHangHoa) ||
                    string.IsNullOrEmpty(item.LoaiHang) || string.IsNullOrEmpty(item.DonViTinhKhoiLuong) ||
                    string.IsNullOrEmpty(item.ThoiGianLuuKho))
                {
                    TempData["Error"] = "Vui lòng điền đầy đủ thông tin cho tất cả các tập hợp hàng hóa.";
                    return RedirectToAction("Index_KhachHang");
                }

                int thoiGianLuuTru;
                if (item.ThoiGianLuuKho == "other")
                {
                    if (!item.ThoiGianLuuKhoKhac.HasValue || item.ThoiGianLuuKhoKhac <= 0)
                    {
                        TempData["Error"] = "Vui lòng nhập số ngày lưu kho hợp lệ.";
                        return RedirectToAction("Index_KhachHang");
                    }
                    thoiGianLuuTru = item.ThoiGianLuuKhoKhac.Value;
                }
                else
                {
                    if (!int.TryParse(item.ThoiGianLuuKho, out thoiGianLuuTru) || thoiGianLuuTru <= 0)
                    {
                        TempData["Error"] = "Thời gian lưu kho không hợp lệ.";
                        return RedirectToAction("Index_KhachHang");
                    }
                }

                float soLuongThucTe = item.DonViTinhKhoiLuong == "Tấn" ? (item.KhoiLuong ?? 0) : (item.SoLuong ?? 0);
                if (soLuongThucTe <= 0)
                {
                    TempData["Error"] = "Vui lòng nhập khối lượng hoặc số lượng hợp lệ.";
                    return RedirectToAction("Index_KhachHang");
                }

                var danhMucHangHoa = db.danhMucHangHoas.FirstOrDefault(d => d.maDanhMucHangHoa == item.maDanhMucHangHoa);
                if (danhMucHangHoa == null)
                {
                    TempData["Error"] = "Danh mục hàng hóa không hợp lệ.";
                    return RedirectToAction("Index_KhachHang");
                }

                var hangHoa = db.hangHoas.FirstOrDefault(h => h.maHangHoa == item.maHangHoa);
                if (hangHoa == null)
                {
                    TempData["Error"] = "Hàng hóa không hợp lệ.";
                    return RedirectToAction("Index_KhachHang");
                }
            }

            // --- Tạo mã đơn hàng tự động tăng ---
            string lastMaDonHang = db.donHangs
                .OrderByDescending(d => d.maDonHang)
                .Select(d => d.maDonHang)
                .FirstOrDefault();

            int nextDonHangNumber = 1;
            if (!string.IsNullOrEmpty(lastMaDonHang) && lastMaDonHang.Length > 2)
            {
                if (int.TryParse(lastMaDonHang.Substring(2), out int lastNumber))
                {
                    nextDonHangNumber = lastNumber + 1;
                }
            }
            string newMaDonHang = "DH" + nextDonHangNumber.ToString("D8");

            var donHang = new donHang
            {
                maDonHang = newMaDonHang,
                maKhachHang = khachHang.maKhachHang,
                maNhanVien = "NV00000001",
                tenNguoiGui = khachHang.tenKhachHang,
                tenNguoiNhan = TenNguoiNhan,
                cangDichDen = CangDichDen,
                ngayTaoDonHang = DateTime.Now,
                thoiGianLuuTru = DanhSachHangHoa.First().ThoiGianLuuKho == "other"
                    ? DanhSachHangHoa.First().ThoiGianLuuKhoKhac.Value
                    : int.Parse(DanhSachHangHoa.First().ThoiGianLuuKho),
                trangThaiDonHang = "Đang yêu cầu",
                trangThaiThanhToan = "Chưa thanh toán",
                tongTien = 0,
                moTa = LoaiDonHang
            };

            try
            {
                db.donHangs.Add(donHang);
                db.SaveChanges();

                // Tạo chi tiết đơn hàng cho từng tập hợp hàng hóa
                int nextChiTietNumber = 1;
                string lastMaChiTiet = db.chiTietDonHangs
                    .OrderByDescending(c => c.maChiTietDonHang)
                    .Select(c => c.maChiTietDonHang)
                    .FirstOrDefault();

                if (!string.IsNullOrEmpty(lastMaChiTiet) && lastMaChiTiet.Length > 4)
                {
                    if (int.TryParse(lastMaChiTiet.Substring(4), out int lastNumber))
                    {
                        nextChiTietNumber = lastNumber + 1;
                    }
                }

                foreach (var item in DanhSachHangHoa)
                {
                    var danhMucHangHoa = db.danhMucHangHoas.FirstOrDefault(d => d.maDanhMucHangHoa == item.maDanhMucHangHoa);
                    var hangHoa = db.hangHoas.FirstOrDefault(h => h.maHangHoa == item.maHangHoa);
                    int thoiGianLuuTru = item.ThoiGianLuuKho == "other"
                        ? item.ThoiGianLuuKhoKhac.Value
                        : int.Parse(item.ThoiGianLuuKho);
                    float soLuongThucTe = item.DonViTinhKhoiLuong == "Tấn" ? (item.KhoiLuong ?? 0) : (item.SoLuong ?? 0);

                    string newMaChiTietDonHang = "CTDH" + nextChiTietNumber.ToString("D6");
                    var chiTietDonHang = new chiTietDonHang
                    {
                        maChiTietDonHang = newMaChiTietDonHang,
                        maDonHang = donHang.maDonHang,
                        maHangHoa = item.maHangHoa,
                        soLuong = soLuongThucTe,
                        donViTinh = item.DonViTinhKhoiLuong,
                        chatLuong = item.LoaiHang,
                        donGia = 0,
                        tienLuuKho = 0,
                        moTa = $"Danh mục: {danhMucHangHoa.tenDanhMucHangHoa}, Hàng hóa: {hangHoa.tenHangHoa}, Loại đơn hàng: {LoaiDonHang}{(string.IsNullOrEmpty(MoTa) ? "" : $", Mô tả: {MoTa}")}"
                    };

                    db.chiTietDonHangs.Add(chiTietDonHang);
                    nextChiTietNumber++;
                }

                db.SaveChanges();

                TempData["Success"] = "Đơn hàng đã được tạo thành công!";
                return RedirectToAction("Index_KhachHang");
            }
            catch (DbEntityValidationException ex)
            {
                string errorMessages = "";
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        errorMessages += $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}\n";
                    }
                }
                TempData["Error"] = "Lỗi dữ liệu: " + errorMessages;
                return RedirectToAction("Index_KhachHang");
            }
        }

        // GET: XemChiTiet
        public ActionResult XemChiTiet()
        {
            ViewBag.Title = "XemChiTiet";
            ViewBag.ActiveSidebar = "XemChiTietVaLichSu";

            var khachHang = Session["KhachHang"] as khachHang;
            if (khachHang == null)
            {
                return RedirectToAction("DangNhap", "Home");
            }

            var donHangs = db.donHangs.Where(d => d.maKhachHang == khachHang.maKhachHang).ToList();

            var daChuyenThanhCong = donHangs
                .Where(d => d.trangThaiDonHang == "Hoàn thành" && d.trangThaiThanhToan == "Đã thanh toán")
                .ToList();

            var daDuyet = donHangs
                .Where(d => (d.trangThaiDonHang == "Đang vận chuyển" || d.trangThaiDonHang == "Đang xử lý") &&
                            (d.trangThaiThanhToan == "Chưa thanh toán" || d.trangThaiThanhToan == "Đã thanh toán"))
                .ToList();

            var chuaDuyet = donHangs
                .Where(d => d.trangThaiDonHang == "Đang yêu cầu" && d.trangThaiThanhToan == "Chưa thanh toán")
                .ToList();

            ViewBag.DaChuyenThanhCong = daChuyenThanhCong;
            ViewBag.DaDuyet = daDuyet;
            ViewBag.ChuaDuyet = chuaDuyet;

            return View();
        }

        //Xem chi tiết đơn hàng
        public ActionResult XemChiTietDonHang(string maDonHang)
        {
            if (string.IsNullOrEmpty(maDonHang))
                return RedirectToAction("Index"); // Hoặc trang danh sách đơn hàng

            // Lấy đơn hàng theo mã
            var donHang = db.donHangs.FirstOrDefault(d => d.maDonHang == maDonHang);
            if (donHang == null)
                return HttpNotFound();

            // Lấy chi tiết đơn hàng
            var chiTiet = db.chiTietDonHangs.Where(c => c.maDonHang == maDonHang).ToList();

            ViewBag.ChiTiet = chiTiet;
            return View(donHang);
        }

        [HttpPost]
        public ActionResult HuyDonHang(string maDonHang)
        {
            try
            {
                // Kiểm tra khách hàng đã đăng nhập
                var khachHang = Session["KhachHang"] as khachHang;
                if (khachHang == null)
                {
                    TempData["Error"] = "Vui lòng đăng nhập để hủy đơn hàng.";
                    return RedirectToAction("DangNhap", "Home");
                }

                // Kiểm tra mã đơn hàng
                if (string.IsNullOrEmpty(maDonHang))
                {
                    TempData["Error"] = "Mã đơn hàng không hợp lệ.";
                    return RedirectToAction("XemChiTiet");
                }

                // Tìm đơn hàng
                var donHang = db.donHangs.FirstOrDefault(dh => dh.maDonHang == maDonHang && dh.maKhachHang == khachHang.maKhachHang);
                if (donHang == null)
                {
                    TempData["Error"] = "Không tìm thấy đơn hàng hoặc bạn không có quyền hủy đơn hàng này.";
                    return RedirectToAction("XemChiTiet");
                }

                // Kiểm tra trạng thái đơn hàng
                if (donHang.trangThaiDonHang != "Đang yêu cầu")
                {
                    TempData["Error"] = "Chỉ có thể hủy đơn hàng đang yêu cầu.";
                    return RedirectToAction("XemChiTiet");
                }

                // Sử dụng transaction để đảm bảo toàn vẹn
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // Xóa chi tiết đơn hàng
                        var chiTietDonHangs = db.chiTietDonHangs.Where(ct => ct.maDonHang == maDonHang).ToList();
                        if (chiTietDonHangs.Any())
                        {
                            db.chiTietDonHangs.RemoveRange(chiTietDonHangs);
                        }

                        // Xóa đơn hàng
                        db.donHangs.Remove(donHang);

                        // Lưu thay đổi
                        db.SaveChanges();
                        transaction.Commit();

                        TempData["Success"] = $"Đã hủy đơn hàng {maDonHang} thành công.";
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        TempData["Error"] = $"Lỗi khi hủy đơn hàng: {ex.Message}";
                        return RedirectToAction("XemChiTiet");
                    }
                }

                return RedirectToAction("XemChiTiet");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi hủy đơn hàng: {ex.Message}";
                return RedirectToAction("XemChiTiet");
            }
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