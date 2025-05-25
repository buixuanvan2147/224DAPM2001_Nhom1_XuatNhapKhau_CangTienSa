using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using Website_CangTienSa.Models;
using System.Linq.Dynamic;
using BCrypt.Net;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;

namespace Website_CangTienSa.Controllers
{
    public class HomeController : System.Web.Mvc.Controller
    {
        private readonly XuatNhapHangTaiCangTienSaEntities db = new XuatNhapHangTaiCangTienSaEntities();
        // GET: TaiKhoan

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult TraCuuDonHang(string maDonHang)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Nhận yêu cầu TraCuuDonHang với maDonHang: {maDonHang}");

                if (string.IsNullOrEmpty(maDonHang))
                {
                    System.Diagnostics.Debug.WriteLine("Mã đơn hàng rỗng.");
                    return Json(new { Success = false, Message = "Vui lòng nhập Mã Đơn Hàng." });
                }

                // Kiểm tra định dạng mã đơn hàng
                if (!Regex.IsMatch(maDonHang, @"^[a-zA-Z0-9-]+$"))
                {
                    System.Diagnostics.Debug.WriteLine($"Mã đơn hàng không hợp lệ: {maDonHang}");
                    return Json(new { Success = false, Message = "Mã đơn hàng chỉ chứa chữ cái, số và dấu gạch ngang." });
                }

                // Thực hiện logic tra cứu đơn hàng từ database
                System.Diagnostics.Debug.WriteLine($"Bắt đầu truy vấn database với maDonHang: {maDonHang}");
                var donHang = db.donHangs.FirstOrDefault(dh => dh.maDonHang == maDonHang);

                if (donHang != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Tìm thấy đơn hàng: {donHang.maDonHang}");

                    // Đảm bảo maDonHang không null (dựa trên mô hình, đây là string, không nullable)
                    if (string.IsNullOrEmpty(donHang.maDonHang))
                    {
                        System.Diagnostics.Debug.WriteLine($"Mã đơn hàng rỗng trong database cho maDonHang: {maDonHang}");
                        return Json(new { Success = false, Message = "Dữ liệu đơn hàng không hợp lệ (mã đơn hàng rỗng)." });
                    }

                    // Đảm bảo các trường không nullable có giá trị hợp lệ
                    if (donHang.ngayTaoDonHang == default(DateTime))
                    {
                        System.Diagnostics.Debug.WriteLine($"Ngày tạo đơn hàng không hợp lệ cho maDonHang: {maDonHang}");
                        return Json(new { Success = false, Message = "Dữ liệu đơn hàng không hợp lệ (ngày tạo đơn hàng không hợp lệ)." });
                    }

                    if (donHang.thoiGianLuuTru <= 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Thời gian lưu trữ không hợp lệ cho maDonHang: {maDonHang}");
                        return Json(new { Success = false, Message = "Dữ liệu đơn hàng không hợp lệ (thời gian lưu trữ không hợp lệ)." });
                    }

                    // Lưu thông tin đơn hàng vào TempData (chỉ lấy các trường cần thiết)
                    TempData["DonHang"] = new
                    {
                        MaDonHang = donHang.maDonHang,
                        //TenNguoiGui = donHang.tenNguoiGui ?? string.Empty,
                        TenNguoiNhan = donHang.tenNguoiNhan ?? string.Empty,
                        CangDichDen = donHang.cangDichDen ?? string.Empty,
                        NgayTaoDonHang = donHang.ngayTaoDonHang.ToString("dd/MM/yyyy"),
                        ThoiGianLuuTru = donHang.thoiGianLuuTru.ToString(),
                        NgayXuatCang = donHang.ngayXuatCang.HasValue ? donHang.ngayXuatCang.Value.ToString("dd/MM/yyyy") : null,
                        NgayNhapCang = donHang.ngayNhapCang.HasValue ? donHang.ngayNhapCang.Value.ToString("dd/MM/yyyy") : null,
                        TrangThaiDonHang = donHang.trangThaiDonHang ?? string.Empty,
                        TrangThaiThanhToan = donHang.trangThaiThanhToan ?? string.Empty,
                        TongTien = donHang.tongTien.HasValue ? donHang.tongTien.Value.ToString("#,##0") : null,
                        MoTa = donHang.moTa ?? string.Empty
                    };
                    System.Diagnostics.Debug.WriteLine($"TempData[DonHang] set: MaDonHang = {donHang.maDonHang}");
                    return Json(new { Success = true, MaDonHang = donHang.maDonHang });
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Không tìm thấy đơn hàng với mã: {maDonHang}");
                    return Json(new { Success = false, Message = "Không tìm thấy đơn hàng với mã: " + maDonHang });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi trong TraCuuDonHang: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return Json(new { Success = false, Message = "Đã có lỗi xảy ra trong quá trình tra cứu: " + ex.Message });
            }
        }

        public ActionResult KhachVangLai_TraCuuDonHang(string maDonHang)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Nhận yêu cầu KhachVangLai_TraCuuDonHang với maDonHang: {maDonHang}");

                // Ưu tiên lấy từ TempData trước
                if (TempData["DonHang"] != null)
                {
                    ViewBag.DonHang = TempData["DonHang"];
                    var maDonHangValue = (TempData["DonHang"] as dynamic)?.MaDonHang ?? "null";
                    System.Diagnostics.Debug.WriteLine($"ViewBag.DonHang set từ TempData: MaDonHang = {maDonHangValue}");
                }
                else if (!string.IsNullOrEmpty(maDonHang))
                {
                    // Kiểm tra định dạng mã đơn hàng
                    if (!Regex.IsMatch(maDonHang, @"^[a-zA-Z0-9-]+$"))
                    {
                        ViewBag.ErrorMessage = "Mã đơn hàng không hợp lệ.";
                        System.Diagnostics.Debug.WriteLine("Mã đơn hàng không hợp lệ: " + maDonHang);
                    }
                    else
                    {
                        // Tra cứu lại từ database nếu có mã đơn hàng
                        System.Diagnostics.Debug.WriteLine($"Bắt đầu truy vấn database với maDonHang: {maDonHang}");
                        var donHang = db.donHangs.FirstOrDefault(dh => dh.maDonHang == maDonHang);
                        if (donHang != null)
                        {
                            // Kiểm tra các trường không nullable
                            if (donHang.ngayTaoDonHang == default(DateTime))
                            {
                                System.Diagnostics.Debug.WriteLine($"Ngày tạo đơn hàng không hợp lệ cho maDonHang: {maDonHang}");
                                ViewBag.ErrorMessage = "Dữ liệu đơn hàng không hợp lệ (ngày tạo đơn hàng không hợp lệ).";
                                return View();
                            }

                            if (donHang.thoiGianLuuTru <= 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"Thời gian lưu trữ không hợp lệ cho maDonHang: {maDonHang}");
                                ViewBag.ErrorMessage = "Dữ liệu đơn hàng không hợp lệ (thời gian lưu trữ không hợp lệ).";
                                return View();
                            }

                            ViewBag.DonHang = new
                            {
                                MaDonHang = donHang.maDonHang,
                                //TenNguoiGui = donHang.tenNguoiGui ?? string.Empty,
                                TenNguoiNhan = donHang.tenNguoiNhan ?? string.Empty,
                                CangDichDen = donHang.cangDichDen ?? string.Empty,
                                NgayTaoDonHang = donHang.ngayTaoDonHang.ToString("dd/MM/yyyy"),
                                ThoiGianLuuTru = donHang.thoiGianLuuTru.ToString(),
                                NgayXuatCang = donHang.ngayXuatCang.HasValue ? donHang.ngayXuatCang.Value.ToString("dd/MM/yyyy") : null,
                                NgayNhapCang = donHang.ngayNhapCang.HasValue ? donHang.ngayNhapCang.Value.ToString("dd/MM/yyyy") : null,
                                TrangThaiDonHang = donHang.trangThaiDonHang ?? string.Empty,
                                TrangThaiThanhToan = donHang.trangThaiThanhToan ?? string.Empty,
                                TongTien = donHang.tongTien.HasValue ? donHang.tongTien.Value.ToString("#,##0") : null,
                                MoTa = donHang.moTa ?? string.Empty
                            };
                            System.Diagnostics.Debug.WriteLine($"ViewBag.DonHang set từ database: MaDonHang = {donHang.maDonHang}");
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Không tìm thấy đơn hàng với mã: " + maDonHang;
                            System.Diagnostics.Debug.WriteLine($"Không tìm thấy đơn hàng với mã: {maDonHang}");
                        }
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Không có mã đơn hàng để tra cứu.";
                    System.Diagnostics.Debug.WriteLine("Không có mã đơn hàng để tra cứu.");
                }

                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi trong KhachVangLai_TraCuuDonHang: {ex.Message}\nStackTrace: {ex.StackTrace}");
                ViewBag.ErrorMessage = "Đã có lỗi xảy ra: " + ex.Message;
                return View();
            }
        }

        public JsonResult GetOrderDetails(string maDonHang)
        {
            if (string.IsNullOrWhiteSpace(maDonHang) || maDonHang.Length > 10 || !Regex.IsMatch(maDonHang, @"^[a-zA-Z0-9-]+$"))
            {
                System.Diagnostics.Debug.WriteLine($"Mã đơn hàng không hợp lệ: {maDonHang}");
                return Json(new { success = false, message = "Mã đơn hàng không hợp lệ" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var orderDetails = db.chiTietDonHangs
                    .Include("hangHoa.danhMucHangHoa")
                    .Where(ctdh => ctdh.maDonHang == maDonHang) // Giữ lambda vì không có System.Linq.Dynamic
                    .Select(ctdh => new
                    {
                        ctdh.maDonHang,
                        ctdh.soLuong,
                        ctdh.donViTinh,
                        ctdh.moTa,
                        ctdh.donGia,
                        ctdh.tienLuuKho,
                        ctdh.chatLuong,
                        tenHH = ctdh.hangHoa != null ? ctdh.hangHoa.tenHangHoa : "Không có tên",
                        tenDMHH = ctdh.hangHoa != null && ctdh.hangHoa.danhMucHangHoa != null
                            ? ctdh.hangHoa.danhMucHangHoa.tenDanhMucHangHoa
                            : "Không có danh mục"
                    })
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Tải thành công {orderDetails.Count} chi tiết đơn hàng cho maDonHang: {maDonHang}");
                return Json(new { success = true, orderDetails }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Lỗi trong GetOrderDetails: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return Json(new { success = false, message = "Đã xảy ra lỗi khi lấy chi tiết đơn hàng" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(string username, string password)
        {
            // 1. Kiểm tra thông tin đăng nhập của khách hàng
            var khachHang = db.khachHangs.FirstOrDefault(kh =>
                kh.tenDangNhap == username && kh.matKhau == password && kh.trangThaiTaiKhoanKhachHang == "Đã duyệt");

            if (khachHang != null)
            {
                FormsAuthentication.SetAuthCookie(khachHang.tenDangNhap, false); // Tạo cookie xác thực
                Session["KhachHang"] = khachHang; // ⚠️ Thêm dòng này để các controller khác biết ai đang đăng nhập
                Session["LoggedInUserId"] = khachHang.maKhachHang;
                Session["UserRole"] = "_LayoutKhachHang";
                return RedirectToAction("Index_KhachHang", "KhachHang");
            }


            // 2. Kiểm tra thông tin đăng nhập của nhân viên
            var nhanVien = db.nhanViens.FirstOrDefault(nv =>
                nv.tenDangNhap == username && nv.matKhau == password && nv.trangThaiTaiKhoanNhanVien == "Đang hoạt động");

            if (nhanVien != null)
            {
                // Lấy vai trò của nhân viên từ database
                var vaiTroNhanVien = db.vaiTroNhanViens.FirstOrDefault(vt => vt.maVaiTroNhanVien == nhanVien.maLoaiNhanVien);

                if (vaiTroNhanVien != null)
                {
                    string userRole = ConvertRoleNameToLayoutName(vaiTroNhanVien.tenLoaiNhanVien);
                    FormsAuthentication.SetAuthCookie(nhanVien.tenDangNhap, false); // Tạo cookie xác thực
                    Session["LoggedInUserId"] = nhanVien.maNhanVien; // Lưu ID nhân viên
                    Session["UserRole"] = userRole;

                    // Cập nhật thời gian đăng nhập gần nhất cho nhân viên
                    nhanVien.thoiGianDangNhapGanNhat = DateTime.Now;
                    db.SaveChanges(); // Lưu thay đổi vào database

                    // Chuyển hướng dựa trên vai trò của nhân viên
                    switch (vaiTroNhanVien.tenLoaiNhanVien)
                    {
                        case "Quản trị viên":
                            return RedirectToAction("Index_QuanTriVien", "QuanTriVien");
                        case "Nhân viên nhập kho":
                            return RedirectToAction("Index_NhanVienNhapKho", "NhanVienNhapKho");
                        case "Nhân viên xuất kho":
                            return RedirectToAction("Index_NhanVienXuatKho", "NhanVienXuatKho");
                        case "Nhân viên kế toán":
                            return RedirectToAction("Index_NhanVienKeToan", "NhanVienKeToan");
                        case "Nhân viên hải quan":
                            return RedirectToAction("Index_NhanVienHaiQuan", "NhanVienHaiQuan");
                        case "Nhân viên kho bãi":
                            return RedirectToAction("Index_NhanVienKhoBai", "NhanVienKhoBai");
                        default:
                            return RedirectToAction("Index", "Home"); // Trang mặc định nếu không khớp vai trò cụ thể
                    }
                }
                else
                {
                    ViewBag.Message = "Đăng nhập thành công nhưng không xác định được vai trò.";
                    return View();
                }
            }

            // Nếu không tìm thấy người dùng phù hợp
            ViewBag.Message = "Đăng nhập không thành công. Vui lòng kiểm tra lại tên đăng nhập và mật khẩu.";
            return View();
        }

        private string ConvertRoleNameToLayoutName(string roleName)
        {
            if (roleName == "Quản trị viên")
            {
                return "_LayoutQuanTriVien";
            }
            else if (roleName == "Nhân viên nhập kho")
            {
                return "_LayoutNhanVienNhapKho";
            }
            else if (roleName == "Nhân viên xuất kho")
            {
                return "_LayoutNhanVienXuatKho";
            }
            else if (roleName == "Nhân viên kế toán")
            {
                return "_LayoutNhanVienKeToan";
            }
            else if (roleName == "Nhân viên hải quan")
            {
                return "_LayoutNhanVienHaiQuan";
            }
            else if (roleName == "Nhân viên kho bãi")
            {
                return "_LayoutNhanVienKhoBai";
            }
            else
            {
                return "_LayoutKhachVangLai";
            }
        }

        // GET: DangKy
        public ActionResult DangKy()
        {
            return View(new KVL_DangKyViewModel());
        }

        // POST: DangKy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DangKy(KVL_DangKyViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra trùng lặp
                if (db.khachHangs.Any(kh => kh.tenDangNhap.ToLower() == model.tenDangNhap.ToLower()))
                {
                    ModelState.AddModelError("tenDangNhap", "Tên đăng nhập này đã tồn tại.");
                    return View(model);
                }
                if (db.khachHangs.Any(kh => kh.email.ToLower() == model.email.ToLower()))
                {
                    ModelState.AddModelError("email", "Email này đã được sử dụng.");
                    return View(model);
                }
                if (db.khachHangs.Any(kh => kh.cccd == model.cccd))
                {
                    ModelState.AddModelError("cccd", "CCCD này đã được đăng ký.");
                    return View(model);
                }

                // Tạo mã khách hàng
                string lastMaKH = db.khachHangs
                                    .OrderByDescending(kh => kh.maKhachHang)
                                    .FirstOrDefault()?.maKhachHang;
                int newNumber = 1;
                string newMaKH = "KH00000001";
                if (!string.IsNullOrEmpty(lastMaKH))
                {
                    string numberPart = lastMaKH.Substring(2);
                    if (int.TryParse(numberPart, out int lastNumber))
                    {
                        newNumber = lastNumber + 1;
                    }
                }
                newMaKH = "KH" + newNumber.ToString("D8");
                while (db.khachHangs.Any(kh => kh.maKhachHang == newMaKH))
                {
                    newNumber++;
                    newMaKH = "KH" + newNumber.ToString("D8");
                }

                // Map ViewModel sang entity khachHang
                var khachHang = new khachHang
                {
                    maKhachHang = newMaKH,
                    tenDangNhap = model.tenDangNhap,
                    matKhau = BCrypt.Net.BCrypt.HashPassword(model.matKhau), // Use BCrypt for password hashing
                    tenKhachHang = model.tenKhachHang,
                    cccd = model.cccd,
                    diaChiLienLac = model.diaChiLienLac,
                    sdtKhachHang = model.sdtKhachHang,
                    email = model.email,
                    ngayDangKy = DateTime.Now,
                    trangThaiTaiKhoanKhachHang = "Chờ duyệt",
                    tenCongTy = string.IsNullOrWhiteSpace(model.tenCongTy) ? null : model.tenCongTy,
                    maSoThueCongTy = string.IsNullOrWhiteSpace(model.maSoThueCongTy) ? null : model.maSoThueCongTy,
                    anhDaiDienKhachHangUrl = null
                };

                try
                {
                    db.khachHangs.Add(khachHang);
                    await db.SaveChangesAsync();         

                    TempData["SuccessMessage"] = "Gửi yêu cầu đăng ký thành công. Vui lòng kiểm tra email để xem thông tin tài khoản.";
                    return RedirectToAction("DangNhap");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Có lỗi xảy ra: {ex.Message}");
                    return View(model); // Return KVL_DangKyViewModel
                }
            }

            // If ModelState is invalid, return the View with the same KVL_DangKyViewModel
            return View(model);
        }

        public ActionResult QuenMatKhau()
        {
            return View();
        }

        [Authorize] // Yêu cầu người dùng phải đăng nhập để truy cập action này
        public ActionResult DoiMatKhau()
        {
            string currentUsername = User.Identity.Name;
            string tenHienThi = "";

            var khachHang = db.khachHangs.FirstOrDefault(kh => kh.tenDangNhap == currentUsername);
            if (khachHang != null)
            {
                tenHienThi = khachHang.tenKhachHang; // Giả sử bạn có trường tenHienThi
            }
            else
            {
                var nhanVien = db.nhanViens.FirstOrDefault(nv => nv.tenDangNhap == currentUsername);
                if (nhanVien != null)
                {
                    tenHienThi = nhanVien.tenHienThi; // Giả sử bạn có trường tenNhanVien
                }
            }

            var model = new DoiMatKhauViewModel
            {
                TenHienThi = tenHienThi
            };

            return View(model);
        }

        // POST: DoiMatKhau
        [HttpPost]
        [Authorize] // Yêu cầu người dùng phải đăng nhập để truy cập action này
        [ValidateAntiForgeryToken] // Xác thực token chống CSRF từ form
        public ActionResult DoiMatKhau(DoiMatKhauViewModel model)
        {
            if (ModelState.IsValid)
            {
                string currentUsername = User.Identity.Name;
                bool passwordChanged = false;

                var khachHang = db.khachHangs.FirstOrDefault(kh => kh.tenDangNhap == currentUsername);
                if (khachHang != null)
                {
                    // **Quan trọng:** So sánh mật khẩu cũ đã nhập với mật khẩu đã hash trong database.
                    // Sử dụng Crypto.VerifyHashedPassword (CHỈ CHO MỤC ĐÍCH DEMO)
                    if (Crypto.VerifyHashedPassword(khachHang.matKhau, model.MatKhauCu))
                    {
                        // Hash mật khẩu mới trước khi lưu
                        khachHang.matKhau = Crypto.HashPassword(model.MatKhauMoi);
                        db.SaveChanges();
                        ViewBag.Message = "Đổi mật khẩu thành công.";
                        passwordChanged = true;
                    }
                    else
                    {
                        ModelState.AddModelError("MatKhauCu", "Mật khẩu cũ không đúng.");
                    }
                }
                else
                {
                    var nhanVien = db.nhanViens.FirstOrDefault(nv => nv.tenDangNhap == currentUsername);
                    if (nhanVien != null)
                    {
                        // **Quan trọng:** So sánh mật khẩu cũ đã nhập với mật khẩu đã hash trong database.
                        // Sử dụng Crypto.VerifyHashedPassword (CHỈ CHO MỤC ĐÍCH DEMO)
                        if (Crypto.VerifyHashedPassword(nhanVien.matKhau, model.MatKhauCu))
                        {
                            // Hash mật khẩu mới
                            nhanVien.matKhau = Crypto.HashPassword(model.MatKhauMoi);
                            db.SaveChanges();
                            ViewBag.Message = "Đổi mật khẩu thành công.";
                            passwordChanged = true;
                        }
                        else
                        {
                            ModelState.AddModelError("MatKhauCu", "Mật khẩu cũ không đúng.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Không tìm thấy tài khoản người dùng.");
                    }
                }

                if (passwordChanged)
                {
                    return View();
                }
            }
            return View(model);
        }

        public ActionResult GioiThieu()
        {
            return View();
        }

        public ActionResult DichVu()
        {
            return View();
        }
        public ActionResult DichVu_YeuCauBaoGia()
        {
            return View();
        }

        public ActionResult DichVu_TimHieuThem()
        {
            return View();
        }
        public ActionResult BangGia()
        {
            return View();
        }

        public ActionResult BangGia_DatDichVu()
        {
            return View();
        }

        public ActionResult TinTuc()
        {
            return View();
        }

        public ActionResult LienHe()
        {
            return View();
        }

        
    }
}