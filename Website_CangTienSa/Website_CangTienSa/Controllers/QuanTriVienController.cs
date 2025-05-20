using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Website_CangTienSa.Models;
using System.Data.Entity;
using System.Linq.Dynamic;
using System.Dynamic;
using System.IO;

namespace Website_CangTienSa.Controllers
{
    public class QuanTriVienController : Controller
    {
        private readonly XuatNhapHangTaiCangTienSaEntities db = new XuatNhapHangTaiCangTienSaEntities();

        // GET: QuanTriVien

        public ActionResult Index_QuanTriVien()
        {
            ViewBag.Title = "Index_QuanTriVien";
            ViewBag.ActiveSidebar = "TongQuan";

            int tongSoTKKhachHang = db.khachHangs.Count();
            ViewBag.TongSoTaiKhoanKhachHang = tongSoTKKhachHang;

            int tongSoTKNhanVien = db.nhanViens.Count();
            ViewBag.TongSoTaiKhoanNhanVien = tongSoTKNhanVien;

            int TongSoKhachHangChoDuyet = db.khachHangs
            .Count(kh => kh.trangThaiTaiKhoanKhachHang == "Chờ duyệt");
            ViewBag.TongSoKhachHangChoDuyet = TongSoKhachHangChoDuyet;

            int TongSoDonHangDangXuLy = db.donHangs
            .Count(kh => kh.trangThaiDonHang == "Đang xử lý");
            ViewBag.TongSoDonHangDangXuLy = TongSoDonHangDangXuLy;

            return View();
        }

        public ActionResult QuanLyTaiKhoan_QuanTriVien()
        {
            ViewBag.Title = "Quản Lý Tài Khoản";
            ViewBag.ActiveSidebar = "Quản Lý Tài Khoản";

            // Lấy danh sách khách hàng (không thay đổi)
            var allKhachHang = db.khachHangs.ToList();
            ViewBag.KhachHangs = allKhachHang;

            // Lấy danh sách vai trò nhân viên (không thay đổi)
            var allVaiTroNhanVien = db.vaiTroNhanViens.Select(vt => vt.tenLoaiNhanVien).Distinct().ToList();
            ViewBag.Roles = allVaiTroNhanVien;

            // Lấy danh sách nhân viên sử dụng NhanVienViewModel
            var allNhanVien = db.nhanViens
                .Include(nv => nv.vaiTroNhanVien)
                .Select(nv => new QTV_QLTK_NhanVienViewModel // Sử dụng QTV_QLTK_NhanVienViewModel tự tạo
                {
                    MaNhanVien = nv.maNhanVien,
                    AnhDaiDienNhanVienUrl = nv.anhDaiDienNhanVienUrl, // Đảm bảo tên thuộc tính khớp
                    TenDangNhap = nv.tenDangNhap,
                    MatKhau = nv.matKhau,
                    TenHienThi = nv.tenHienThi,
                    SdtNhanVien = nv.sdtNhanVien,
                    DiaChi = nv.diaChi,
                    Email = nv.email,
                    TrangThaiTaiKhoanNhanVien = nv.trangThaiTaiKhoanNhanVien,
                    ThoiGianDangNhapGanNhat = nv.thoiGianDangNhapGanNhat,
                    TenLoaiNhanVien = nv.vaiTroNhanVien.tenLoaiNhanVien
                })
                .ToList();
            ViewBag.NhanViens = allNhanVien;

            return View();
        }

        public ActionResult TaoTaiKhoanNhanVien_QuanTriVien()
        {
            ViewBag.vaiTroNhanVienList = new SelectList(db.vaiTroNhanViens, "maVaiTroNhanVien", "tenLoaiNhanVien");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoTaiKhoanNhanVien_QuanTriVien(nhanVien model, HttpPostedFileBase anhDaiDienNhanVienUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Tạo mã nhân viên mới (VD: NV00000001)
                    string lastMaNV = db.nhanViens
                                        .OrderByDescending(nv => nv.maNhanVien)
                                        .FirstOrDefault()?.maNhanVien;

                    int newNumber = 1;
                    string newMaNV = "NV00000001"; // Mặc định

                    if (!string.IsNullOrEmpty(lastMaNV))
                    {
                        string numberPart = lastMaNV.Substring(2); // Bỏ "NV"
                        if (int.TryParse(numberPart, out int lastNumber))
                        {
                            newNumber = lastNumber + 1;
                        }
                    }

                    newMaNV = "NV" + newNumber.ToString("D8"); // Tạo mã: NV + 8 chữ số → tổng cộng 10 ký tự

                    // Đảm bảo không bị trùng mã nhân viên
                    while (db.nhanViens.Any(nv => nv.maNhanVien == newMaNV))
                    {
                        newNumber++;
                        newMaNV = "NV" + newNumber.ToString("D8");
                    }

                    // Xử lý ảnh bìa
                    if (anhDaiDienNhanVienUrl != null && anhDaiDienNhanVienUrl.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(anhDaiDienNhanVienUrl.FileName);
                        string filePath = Path.Combine(Server.MapPath("~/Content/img/"), fileName); // Đường dẫn lưu ảnh
                        anhDaiDienNhanVienUrl.SaveAs(filePath); // Lưu ảnh vào thư mục trên server
                        model.anhDaiDienNhanVienUrl = fileName; // Lưu đường dẫn ảnh vào cơ sở dữ liệu
                    }

                    // Gán mã sách mới và thêm vào cơ sở dữ liệu
                    model.maNhanVien = newMaNV;
                    model.thoiGianDangNhapGanNhat = DateTime.Now;
                    db.nhanViens.Add(model);
                    db.SaveChanges();

                    // Chuyển hướng về trang danh sách sách
                    //return RedirectToAction("SachList", "SellerDashboard");
                }
                catch (Exception ex)
                {
                    // Ghi nhận lỗi và hiển thị thông báo
                    Console.WriteLine("Error: " + ex.Message); // Log lỗi ra console hoặc log file
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                }
            }

            // Truyền lại danh sách thể loại để hiển thị dropdown
            ViewBag.vaiTroNhanVienList = new SelectList(db.vaiTroNhanViens, "maVaiTroNhanVien", "tenLoaiNhanVien", model.maLoaiNhanVien);
            return View(model);
        }

        // Action để Duyệt tài khoản khách hàng
        [HttpPost]
        public JsonResult ApproveCustomerAccount(string id)
        {
            var khachHang = db.khachHangs.Find(id);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Khách hàng không tồn tại." });
            }

            if (khachHang.trangThaiTaiKhoanKhachHang == "Chờ duyệt" || khachHang.trangThaiTaiKhoanKhachHang == "Đã từ chối")
            {
                khachHang.trangThaiTaiKhoanKhachHang = "Đã duyệt";
                db.Entry(khachHang).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Tài khoản khách hàng đã được duyệt." });
            }
            return Json(new { success = false, message = "Tài khoản khách hàng không ở trạng thái 'Chờ duyệt' hoặc 'Đã từ chối' để duyệt." });
        }

        // Action để Từ chối yêu cầu đăng ký tài khoản khách hàng
        [HttpPost]
        public JsonResult RejectCustomerAccount(string id)
        {
            var khachHang = db.khachHangs.Find(id);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Khách hàng không tồn tại." });
            }

            if (khachHang.trangThaiTaiKhoanKhachHang == "Chờ duyệt")
            {
                khachHang.trangThaiTaiKhoanKhachHang = "Đã từ chối";
                db.Entry(khachHang).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Tài khoản khách hàng đã bị từ chối." });
            }
            return Json(new { success = false, message = "Tài khoản khách hàng không ở trạng thái 'Chờ duyệt' để từ chối." });
        }

        // Action để Khóa tài khoản khách hàng
        [HttpPost]
        public JsonResult LockCustomerAccount(string id)
        {
            var khachHang = db.khachHangs.Find(id);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Khách hàng không tồn tại." });
            }

            if (khachHang.trangThaiTaiKhoanKhachHang == "Đã duyệt")
            {
                khachHang.trangThaiTaiKhoanKhachHang = "Đang bị khóa";
                db.Entry(khachHang).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Tài khoản khách hàng đã bị khóa." });
            }
            return Json(new { success = false, message = "Tài khoản khách hàng không ở trạng thái 'Đã duyệt' để khóa." });
        }

        // Action để Mở khóa tài khoản khách hàng
        [HttpPost]
        public JsonResult UnlockCustomerAccount(string id)
        {
            var khachHang = db.khachHangs.Find(id);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Khách hàng không tồn tại." });
            }

            if (khachHang.trangThaiTaiKhoanKhachHang == "Đang bị khóa")
            {
                khachHang.trangThaiTaiKhoanKhachHang = "Đã duyệt"; // Mở khóa về trạng thái "Đã duyệt"
                db.Entry(khachHang).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Tài khoản khách hàng đã được mở khóa." });
            }
            return Json(new { success = false, message = "Tài khoản khách hàng không ở trạng thái 'Đang bị khóa' để mở khóa." });
        }

        // Action để Xóa tài khoản khách hàng
        [HttpPost]
        public JsonResult DeleteCustomerAccount(string id)
        {
            var khachHang = db.khachHangs.Find(id);
            if (khachHang == null)
            {
                return Json(new { success = false, message = "Khách hàng không tồn tại." });
            }

            // Cân nhắc thực hiện Soft Delete (đặt cờ IsDeleted) thay vì Hard Delete để giữ lại lịch sử
            // Ví dụ: khachHang.IsDeleted = true;
            // db.Entry(khachHang).State = EntityState.Modified;
            db.khachHangs.Remove(khachHang); // Thực hiện Hard Delete theo yêu cầu hiện tại
            db.SaveChanges();

            return Json(new { success = true, message = "Tài khoản khách hàng đã được xóa vĩnh viễn." });
        }

        // Action để Khóa tài khoản nhân viên
        [HttpPost]
        public JsonResult LockNhanVienAccount(string id)
        {
            var nhanVien = db.nhanViens.Find(id);
            if (nhanVien == null)
            {
                return Json(new { success = false, message = "Nhân viên không tồn tại." });
            }

            // Không cho phép khóa tài khoản đang bị khóa hoặc tài khoản của chính quản trị viên đang đăng nhập (tùy nghiệp vụ)
            if (nhanVien.trangThaiTaiKhoanNhanVien == "Đang hoạt động")
            {
                nhanVien.trangThaiTaiKhoanNhanVien = "Đang bị khóa";
                db.Entry(nhanVien).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Tài khoản nhân viên đã bị khóa." });
            }
            return Json(new { success = false, message = "Tài khoản nhân viên không ở trạng thái 'Đang hoạt động' để khóa." });
        }

        // Action để Mở khóa tài khoản nhân viên
        [HttpPost]
        public JsonResult UnlockNhanVienAccount(string id)
        {
            var nhanVien = db.nhanViens.Find(id);
            if (nhanVien == null)
            {
                return Json(new { success = false, message = "Nhân viên không tồn tại." });
            }

            if (nhanVien.trangThaiTaiKhoanNhanVien == "Đang bị khóa")
            {
                nhanVien.trangThaiTaiKhoanNhanVien = "Đang hoạt động"; // Mở khóa về trạng thái "Đang hoạt động"
                db.Entry(nhanVien).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true, message = "Tài khoản nhân viên đã được mở khóa." });
            }
            return Json(new { success = false, message = "Tài khoản nhân viên không ở trạng thái 'Đang bị khóa' để mở khóa." });
        }


        // Action để Xóa tài khoản nhân viên
        [HttpPost]
        public JsonResult DeleteNhanVienAccount(string id)
        {
            var nhanVien = db.nhanViens.Find(id);
            if (nhanVien == null)
            {
                return Json(new { success = false, message = "Nhân viên không tồn tại." });
            }

            // Không cho phép xóa tài khoản của chính quản trị viên đang đăng nhập (tùy nghiệp vụ)
            // if (User.Identity.Name == nhanVien.tenDangNhap)
            // {
            //     return Json(new { success = false, message = "Bạn không thể tự xóa tài khoản của mình." });
            // }

            // Cân nhắc Soft Delete
            db.nhanViens.Remove(nhanVien); // Thực hiện Hard Delete theo yêu cầu hiện tại
            db.SaveChanges();

            return Json(new { success = true, message = "Tài khoản nhân viên đã được xóa vĩnh viễn." });
        }

        // GET: Chỉnh sửa tài khoản nhân viên
        public ActionResult ChinhSuaNhanVien_QuanTriVien(string id)
        {
            ViewBag.Title = "Chỉnh Sửa Thông Tin Nhân Viên";
            ViewBag.ActiveSidebar = "Quản Lý Tài Khoản";

            var nhanVien = db.nhanViens.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }

            // Truyền danh sách vai trò cho dropdown list
            ViewBag.vaiTroNhanVienList = new SelectList(db.vaiTroNhanViens, "maVaiTroNhanVien", "tenLoaiNhanVien", nhanVien.maLoaiNhanVien);

            // Chuyển đổi sang ViewModel nếu bạn muốn hiển thị/chỉnh sửa một tập hợp con các thuộc tính
            // Hiện tại, tôi sẽ trả về model nhanVien trực tiếp, đảm bảo View chấp nhận model này
            return View(nhanVien);
        }

        // POST: Xử lý chỉnh sửa tài khoản nhân viên
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChinhSuaNhanVien_QuanTriVien(nhanVien model, HttpPostedFileBase anhDaiDienNhanVienFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existingNhanVien = db.nhanViens.Find(model.maNhanVien);
                    if (existingNhanVien == null)
                    {
                        ModelState.AddModelError("", "Nhân viên không tồn tại.");
                        ViewBag.vaiTroNhanVienList = new SelectList(db.vaiTroNhanViens, "maVaiTroNhanVien", "tenLoaiNhanVien", model.maLoaiNhanVien);
                        return View(model);
                    }

                    // Cập nhật các trường thông tin từ model gửi lên
                    existingNhanVien.tenHienThi = model.tenHienThi;
                    existingNhanVien.sdtNhanVien = model.sdtNhanVien;
                    existingNhanVien.diaChi = model.diaChi;
                    existingNhanVien.email = model.email;
                    existingNhanVien.maLoaiNhanVien = model.maLoaiNhanVien;
                    existingNhanVien.trangThaiTaiKhoanNhanVien = model.trangThaiTaiKhoanNhanVien; // Cho phép chỉnh sửa trạng thái nếu cần

                    // Cần CẨN THẬN khi cập nhật mật khẩu.
                    // Thông thường, bạn sẽ không gửi mật khẩu từ form chỉnh sửa trực tiếp
                    // mà sẽ có một form riêng để đổi mật khẩu.
                    // Nếu bạn muốn cho phép cập nhật, hãy đảm bảo đã mã hóa mật khẩu trước khi lưu.
                    // Ví dụ: if (!string.IsNullOrEmpty(model.matKhau)) { existingNhanVien.matKhau = YourHashFunction(model.matKhau); }

                    // Xử lý ảnh đại diện nếu có file mới được upload
                    if (anhDaiDienNhanVienFile != null && anhDaiDienNhanVienFile.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(anhDaiDienNhanVienFile.FileName);
                        string filePath = Path.Combine(Server.MapPath("~/Content/img/"), fileName);
                        anhDaiDienNhanVienFile.SaveAs(filePath);
                        existingNhanVien.anhDaiDienNhanVienUrl = fileName;
                    }
                    // Giữ ảnh cũ nếu không có ảnh mới được upload và ảnh cũ không null
                    else if (string.IsNullOrEmpty(existingNhanVien.anhDaiDienNhanVienUrl))
                    {
                        // Nếu không có ảnh mới và ảnh cũ là null/rỗng, gán ảnh mặc định
                        existingNhanVien.anhDaiDienNhanVienUrl = "default_user_image.png";
                    }


                    db.Entry(existingNhanVien).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Cập nhật thông tin nhân viên thành công!";
                    return RedirectToAction("QuanLyTaiKhoan_QuanTriVien"); // Chuyển hướng về trang quản lý tài khoản
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error updating staff account: " + ex.Message);
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật thông tin nhân viên: " + ex.Message);
                }
            }
            // Nếu ModelState không hợp lệ hoặc có lỗi, truyền lại danh sách vai trò
            ViewBag.vaiTroNhanVienList = new SelectList(db.vaiTroNhanViens, "maVaiTroNhanVien", "tenLoaiNhanVien", model.maLoaiNhanVien);
            return View(model);
        }

        public ActionResult QuanLyDonHang_QuanTriVien()
        {
            ViewBag.Title = "QuanLyDonHang_QuanTriVien";
            ViewBag.ActiveSidebar = "QuanLyDonHang";
            return View();
        }

        public ActionResult ThongKeBaoCao_QuanTriVien()
        {
            ViewBag.Title = "ThongKeBaoCao_QuanTriVien";
            ViewBag.ActiveSidebar = "ThongKeBaoCao";
            return View();
        }

        public ActionResult TrangCaNhan_QuanTriVien()
        {
            ViewBag.Title = "TrangCaNhan_QuanTriVien";
            ViewBag.ActiveSidebar = "TrangCaNhan";

            string tenDangNhap = User.Identity.Name;

            var nhanVienData = db.nhanViens
                .Where(nv => nv.tenDangNhap == tenDangNhap && nv.vaiTroNhanVien.tenLoaiNhanVien == "Quản trị viên")
                .FirstOrDefault();

            QuanTriVienViewModel quanTriVienViewModel = null;

            if (nhanVienData != null)
            {
                quanTriVienViewModel = new QuanTriVienViewModel
                {
                    MaNhanVien = nhanVienData.maNhanVien,
                    TenDangNhap = nhanVienData.tenDangNhap,
                    TenHienThi = nhanVienData.tenHienThi,
                    Email = nhanVienData.email,
                    SdtNhanVien = nhanVienData.sdtNhanVien,
                    DiaChi = nhanVienData.diaChi,
                    ThoiGianDangNhapGanNhat = nhanVienData.thoiGianDangNhapGanNhat,
                    AnhDaiDienNhanVienUrl = nhanVienData.anhDaiDienNhanVienUrl
                };

                // Kiểm tra nếu AnhDaiDienNhanVienUrl là null hoặc rỗng, thì gán giá trị mặc định
                if (string.IsNullOrEmpty(quanTriVienViewModel.AnhDaiDienNhanVienUrl))
                {
                    quanTriVienViewModel.AnhDaiDienNhanVienUrl = "~/Content/img/default_user_image.png";
                }
            }
            else
            {
                return Content("Không tìm thấy thông tin quản trị viên.");
            }

            return View(quanTriVienViewModel);
        }

        public ActionResult DangXuat_QuanTriVien()
        {
            FormsAuthentication.SignOut(); // Xóa cookie xác thực
            Session.Abandon(); // Hủy bỏ toàn bộ session
            return RedirectToAction("Index", "Home"); // Chuyển hướng về trang chủ
        }

    }
}