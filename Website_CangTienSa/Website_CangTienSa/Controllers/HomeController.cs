using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using Website_CangTienSa.Models;

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
                nv.tenDangNhap == username && nv.matKhau == password && nv.trangThaiTaiKhoanNhanVien == "Mở");

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

        public ActionResult DangKy()
        {
            return View();
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