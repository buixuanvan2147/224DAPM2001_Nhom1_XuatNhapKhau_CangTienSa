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
        [HttpPost]
        public ActionResult TaoDonHang(string maDanhMucHangHoa, string ThoiGianLuuKho, int? ThoiGianLuuKhoKhac,
        string TenNguoiNhan, string SDTNguoiNhan, string CangDichDen, string LoaiHang,
        string DonViTinhKhoiLuong, float? KhoiLuong, float? SoLuong,
        string LoaiDonHang,string MoTa, string MaHangHoa)
            {
            var khachHang = Session["KhachHang"] as khachHang;
            if (khachHang == null)
            {
                return RedirectToAction("DangNhap", "Home");
            }

            // Validation
            if (string.IsNullOrEmpty(maDanhMucHangHoa) || string.IsNullOrEmpty(CangDichDen) ||
                string.IsNullOrEmpty(TenNguoiNhan) || string.IsNullOrEmpty(SDTNguoiNhan) ||
                string.IsNullOrEmpty(ThoiGianLuuKho) || string.IsNullOrEmpty(LoaiHang) ||
                string.IsNullOrEmpty(DonViTinhKhoiLuong) || string.IsNullOrEmpty(LoaiDonHang) ||
                string.IsNullOrEmpty(MaHangHoa))
            {
                TempData["Error"] = "Vui lòng điền đầy đủ các trường bắt buộc.";
                return RedirectToAction("Index_KhachHang");
            }

            if (!Regex.IsMatch(SDTNguoiNhan, @"^\d{10}$"))
            {
                TempData["Error"] = "Số điện thoại người nhận phải có đúng 10 chữ số.";
                return RedirectToAction("Index_KhachHang");
            }

            int thoiGianLuuTru;
            if (ThoiGianLuuKho == "other")
            {
                if (!ThoiGianLuuKhoKhac.HasValue || ThoiGianLuuKhoKhac <= 0)
                {
                    TempData["Error"] = "Vui lòng nhập số ngày lưu kho hợp lệ.";
                    return RedirectToAction("Index_KhachHang");
                }
                thoiGianLuuTru = ThoiGianLuuKhoKhac.Value;
            }
            else
            {
                if (!int.TryParse(ThoiGianLuuKho, out thoiGianLuuTru) || thoiGianLuuTru <= 0)
                {
                    TempData["Error"] = "Thời gian lưu kho không hợp lệ.";
                    return RedirectToAction("Index_KhachHang");
                }
            }

            float soLuongThucTe = DonViTinhKhoiLuong == "Tấn" ? (KhoiLuong ?? 0) : (SoLuong ?? 0); // Đổi từ 'ton' thành 'Tấn'
            if (soLuongThucTe <= 0)
            {
                TempData["Error"] = "Vui lòng nhập khối lượng hoặc số lượng hợp lệ.";
                return RedirectToAction("Index_KhachHang");
            }

            var danhMucHangHoa = db.danhMucHangHoas.FirstOrDefault(d => d.maDanhMucHangHoa == maDanhMucHangHoa);
            if (danhMucHangHoa == null)
            {
                TempData["Error"] = "Danh mục hàng hóa không hợp lệ.";
                return RedirectToAction("Index_KhachHang");
            }

            var hangHoa = db.hangHoas.FirstOrDefault(h => h.maHangHoa == MaHangHoa);
            if (hangHoa == null)
            {
                TempData["Error"] = "Hàng hóa không hợp lệ.";
                return RedirectToAction("Index_KhachHang");
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
                thoiGianLuuTru = thoiGianLuuTru,
                trangThaiDonHang = "Đang xử lý",
                trangThaiThanhToan = "Chưa thanh toán",
                tongTien = 0,
                moTa = LoaiDonHang
            };

            try
            {
                db.donHangs.Add(donHang);
                db.SaveChanges();

                // Tạo mã chi tiết đơn hàng tự động tăng
                string lastMaChiTiet = db.chiTietDonHangs
                    .OrderByDescending(c => c.maChiTietDonHang)
                    .Select(c => c.maChiTietDonHang)
                    .FirstOrDefault();

                int nextChiTietNumber = 1;
                if (!string.IsNullOrEmpty(lastMaChiTiet) && lastMaChiTiet.Length > 4)
                {
                    if (int.TryParse(lastMaChiTiet.Substring(4), out int lastNumber))
                    {
                        nextChiTietNumber = lastNumber + 1;
                    }
                }
                string newMaChiTietDonHang = "CTDH" + nextChiTietNumber.ToString("D6");

                var chiTietDonHang = new chiTietDonHang
                {
                    maChiTietDonHang = newMaChiTietDonHang,
                    maDonHang = donHang.maDonHang,
                    maHangHoa = MaHangHoa,
                    soLuong = soLuongThucTe,
                    donViTinh = DonViTinhKhoiLuong, // Lưu trực tiếp "Tấn" hoặc "Cái"
                    chatLuong = LoaiHang,
                    donGia = 0,
                    tienLuuKho = 0,
                    moTa = $"Danh mục: {danhMucHangHoa.tenDanhMucHangHoa}, Hàng hóa: {hangHoa.tenHangHoa}, Loại đơn hàng: {LoaiDonHang}{(string.IsNullOrEmpty(MoTa) ? "" : $", Mô tả: {MoTa}")}"
                };

                db.chiTietDonHangs.Add(chiTietDonHang);
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
                .Where(d => d.trangThaiDonHang == "Hoàn thành" && d.trangThaiThanhToan == "Chưa thanh toán")
                .ToList();

            var chuaDuyet = donHangs
                .Where(d => d.trangThaiDonHang == "Đang xử lý" && d.trangThaiThanhToan == "Chưa thanh toán")
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