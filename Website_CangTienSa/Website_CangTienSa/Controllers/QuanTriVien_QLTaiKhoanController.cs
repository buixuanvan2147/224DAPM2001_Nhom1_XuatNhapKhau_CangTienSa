using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_CangTienSa.Models;
using System.Dynamic;
using System.IO;
using System.Data.Entity.Validation;

namespace Website_CangTienSa.Controllers
{
    public class QuanTriVien_QLTaiKhoanController : Controller
    {
        private readonly XuatNhapHangTaiCangTienSaEntities db = new XuatNhapHangTaiCangTienSaEntities();

        // GET: Hiển thị form tạo tài khoản khách hàng
        public ActionResult TaoTaiKhoanKhachHang_QuanTriVien()
        {
            // Truyền danh sách trạng thái tài khoản để hiển thị trong dropdown
            ViewBag.TrangThaiList = new SelectList(new[] { "Chờ duyệt", "Đang bị khóa", "Đã duyệt" }, "Chờ duyệt");

            // Luôn truyền một đối tượng model mới để tránh lỗi NullReferenceException khi View render lần đầu
            return View(new khachHang());
        }

        // POST: Xử lý tạo tài khoản khách hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoTaiKhoanKhachHang_QuanTriVien(khachHang model, HttpPostedFileBase imageFile)
        {
            // Truyền lại danh sách trạng thái tài khoản để hiển thị dropdown khi có lỗi
            ViewBag.TrangThaiList = new SelectList(new[] { "Chờ duyệt", "Đang bị khóa", "Đã duyệt" }, model.trangThaiTaiKhoanKhachHang);

            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra các trường UNIQUE trong cơ sở dữ liệu
                    if (db.khachHangs.Any(kh => kh.tenDangNhap == model.tenDangNhap))
                    {
                        ModelState.AddModelError("tenDangNhap", "Tên đăng nhập đã tồn tại.");
                        TempData["ErrorMessage"] = "Tên đăng nhập đã tồn tại."; // Sử dụng TempData
                        return View(model);
                    }
                    if (db.khachHangs.Any(kh => kh.email == model.email))
                    {
                        ModelState.AddModelError("email", "Email đã được sử dụng.");
                        TempData["ErrorMessage"] = "Email đã được sử dụng."; // Sử dụng TempData
                        return View(model);
                    }
                    if (!string.IsNullOrEmpty(model.cccd) && db.khachHangs.Any(kh => kh.cccd == model.cccd))
                    {
                        ModelState.AddModelError("cccd", "Số CCCD đã tồn tại.");
                        TempData["ErrorMessage"] = "Số CCCD đã tồn tại."; // Sử dụng TempData
                        return View(model);
                    }
                    if (db.khachHangs.Any(kh => kh.sdtKhachHang == model.sdtKhachHang))
                    {
                        ModelState.AddModelError("sdtKhachHang", "Số điện thoại đã được sử dụng.");
                        TempData["ErrorMessage"] = "Số điện thoại đã được sử dụng."; // Sử dụng TempData
                        return View(model);
                    }

                    // Tạo mã khách hàng mới (KHxxxxxxxx)
                    int newNumber = 1;
                    string newMaKH = "KH" + newNumber.ToString("D8"); // KH00000001

                    while (db.khachHangs.Any(kh => kh.maKhachHang == newMaKH))
                    {
                        newNumber++;
                        newMaKH = "KH" + newNumber.ToString("D8");
                    }
                    model.maKhachHang = newMaKH;

                    // Xử lý ảnh đại diện
                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        // Kiểm tra định dạng file
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var extension = Path.GetExtension(imageFile.FileName)?.ToLower();
                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("imageFile", "Chỉ hỗ trợ file JPG, PNG hoặc GIF.");
                            TempData["ErrorMessage"] = "Chỉ hỗ trợ file JPG, PNG hoặc GIF."; // Sử dụng TempData
                            return View(model);
                        }

                        // Kiểm tra kích thước file (tối đa 5MB)
                        if (imageFile.ContentLength > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("imageFile", "Kích thước file không được vượt quá 5MB.");
                            TempData["ErrorMessage"] = "Kích thước file không được vượt quá 5MB."; // Sử dụng TempData
                            return View(model);
                        }

                        // Lưu file vào thư mục ~/Content/img/KhachHangUrl/
                        string fileName = Guid.NewGuid().ToString() + extension;
                        string folderPath = Server.MapPath("~/Content/img/KhachHangUrl/");
                        Directory.CreateDirectory(folderPath); // Tạo thư mục nếu chưa tồn tại
                        string filePath = Path.Combine(folderPath, fileName);

                        imageFile.SaveAs(filePath);
                        model.anhDaiDienKhachHangUrl = fileName; // Lưu tên file vào DB
                    }
                    else
                    {
                        // Nếu không có ảnh, gán ảnh mặc định
                        model.anhDaiDienKhachHangUrl = "default_user_image.png"; // Lưu tên file mặc định vào DB
                    }

                    // Mã hóa mật khẩu trước khi lưu (Giả định BCrypt.Net đã được cài đặt)
                    // if (model.matKhau != null) // Kiểm tra null để tránh lỗi nếu matKhau không được gửi
                    // {
                    //     model.matKhau = BCrypt.Net.BCrypt.HashPassword(model.matKhau);
                    // }
                    // Bỏ comment dòng trên nếu bạn đã cài BCrypt.Net và muốn mã hóa mật khẩu

                    // Gán ngày đăng ký
                    model.ngayDangKy = DateTime.Now;

                    // Kiểm tra trạng thái tài khoản hợp lệ (mặc dù đã có DropDownList, nhưng vẫn nên kiểm tra ở backend)
                    var validStatuses = new[] { "Chờ duyệt", "Đang bị khóa", "Đã duyệt" };
                    if (!validStatuses.Contains(model.trangThaiTaiKhoanKhachHang))
                    {
                        ModelState.AddModelError("trangThaiTaiKhoanKhachHang", "Trạng thái tài khoản không hợp lệ.");
                        TempData["ErrorMessage"] = "Trạng thái tài khoản không hợp lệ."; // Sử dụng TempData
                        return View(model);
                    }

                    // Thêm vào cơ sở dữ liệu
                    db.khachHangs.Add(model);
                    db.SaveChanges();

                    // Thông báo thành công và chuyển hướng
                    TempData["SuccessMessage"] = "Tạo tài khoản khách hàng thành công!"; // Sử dụng TempData
                    return RedirectToAction("TaoTaiKhoanKhachHang_QuanTriVien");
                }
                catch (Exception ex)
                {
                    // Ghi log lỗi (nên sử dụng logging framework như Serilog hoặc NLog trong ứng dụng thực tế)
                    System.Diagnostics.Debug.WriteLine("Lỗi khi tạo tài khoản: " + ex.Message);
                    ModelState.AddModelError("", "Có lỗi xảy ra khi tạo tài khoản: " + ex.Message);
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi tạo tài khoản: " + ex.Message; // Sử dụng TempData
                }
            }
            else
            {
                // Nếu ModelState không hợp lệ, hiển thị lỗi chung
                TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin nhập."; // Sử dụng TempData
            }

            // Trả về View với model hiện tại để hiển thị lỗi và dữ liệu đã nhập
            return View(model);
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
                    // Kiểm tra các trường bắt buộc
                    if (string.IsNullOrWhiteSpace(model.maLoaiNhanVien))
                    {
                        ModelState.AddModelError("maLoaiNhanVien", "Vai trò là bắt buộc.");
                    }
                    if (string.IsNullOrWhiteSpace(model.tenDangNhap))
                    {
                        ModelState.AddModelError("tenDangNhap", "Tên đăng nhập là bắt buộc.");
                    }
                    else if (db.nhanViens.Any(nv => nv.tenDangNhap == model.tenDangNhap))
                    {
                        ModelState.AddModelError("tenDangNhap", "Tên đăng nhập đã tồn tại.");
                    }
                    if (string.IsNullOrWhiteSpace(model.matKhau))
                    {
                        ModelState.AddModelError("matKhau", "Mật khẩu là bắt buộc.");
                    }
                    if (string.IsNullOrWhiteSpace(model.email))
                    {
                        ModelState.AddModelError("email", "Email là bắt buộc.");
                    }
                    else if (db.nhanViens.Any(nv => nv.email == model.email))
                    {
                        ModelState.AddModelError("email", "Email đã tồn tại.");
                    }
                    if (string.IsNullOrWhiteSpace(model.trangThaiTaiKhoanNhanVien))
                    {
                        ModelState.AddModelError("trangThaiTaiKhoanNhanVien", "Trạng thái tài khoản là bắt buộc.");
                    }
                    if (!string.IsNullOrWhiteSpace(model.sdtNhanVien) && db.nhanViens.Any(nv => nv.sdtNhanVien == model.sdtNhanVien))
                    {
                        ModelState.AddModelError("sdtNhanVien", "Số điện thoại đã tồn tại.");
                    }
                    if (!string.IsNullOrWhiteSpace(model.diaChi) && db.nhanViens.Any(nv => nv.diaChi == model.diaChi))
                    {
                        ModelState.AddModelError("diaChi", "Địa chỉ đã tồn tại.");
                    }

                    // Kiểm tra khóa ngoại
                    if (!string.IsNullOrWhiteSpace(model.maLoaiNhanVien) && !db.vaiTroNhanViens.Any(v => v.maVaiTroNhanVien == model.maLoaiNhanVien))
                    {
                        ModelState.AddModelError("maLoaiNhanVien", "Vai trò không hợp lệ.");
                    }

                    if (!ModelState.IsValid)
                    {
                        ViewBag.vaiTroNhanVienList = new SelectList(db.vaiTroNhanViens, "maVaiTroNhanVien", "tenLoaiNhanVien", model.maLoaiNhanVien);
                        return View(model);
                    }

                    // Tạo mã nhân viên mới
                    string lastMaNV = db.nhanViens
                                        .OrderByDescending(nv => nv.maNhanVien)
                                        .FirstOrDefault()?.maNhanVien;

                    int newNumber = 1;
                    string newMaNV = "NV00000001";

                    if (!string.IsNullOrEmpty(lastMaNV))
                    {
                        string numberPart = lastMaNV.Substring(2);
                        if (int.TryParse(numberPart, out int lastNumber))
                        {
                            newNumber = lastNumber + 1;
                        }
                    }

                    newMaNV = "NV" + newNumber.ToString("D8");

                    while (db.nhanViens.Any(nv => nv.maNhanVien == newMaNV))
                    {
                        newNumber++;
                        newMaNV = "NV" + newNumber.ToString("D8");
                    }

                    // Xử lý ảnh
                    if (anhDaiDienNhanVienUrl != null && anhDaiDienNhanVienUrl.ContentLength > 0)
                    {
                        string fileName = Path.GetFileName(anhDaiDienNhanVienUrl.FileName);
                        string filePath = Path.Combine(Server.MapPath("~/Content/img/NhanVienUrl/"), fileName);
                        anhDaiDienNhanVienUrl.SaveAs(filePath);
                        model.anhDaiDienNhanVienUrl = fileName;
                    }
                    else
                    {
                        model.anhDaiDienNhanVienUrl = "default_user_image.png";
                    }

                    // Gán các giá trị
                    model.maNhanVien = newMaNV;
                    model.thoiGianDangNhapGanNhat = DateTime.Now;

                    // Thêm vào cơ sở dữ liệu
                    db.nhanViens.Add(model);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => $"{x.PropertyName}: {x.ErrorMessage}");
                    foreach (var error in errorMessages)
                    {
                        ModelState.AddModelError("", error);
                    }
                    Console.WriteLine("Validation Errors: " + string.Join(", ", errorMessages));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                    Console.WriteLine("Error: " + ex.Message);
                }
            }

            ViewBag.vaiTroNhanVienList = new SelectList(db.vaiTroNhanViens, "maVaiTroNhanVien", "tenLoaiNhanVien", model.maLoaiNhanVien);
            return View(model);
        }

        [HttpPost] // Chuyển sang POST để tăng bảo mật
        public ActionResult DuyetKhachHangHangLoat_QuanTriVien()
        {
            try
            {
                var khachHangsChoDuyet = db.khachHangs
                    .Where(k => k.trangThaiTaiKhoanKhachHang == "Chờ duyệt")
                    .ToList();

                if (khachHangsChoDuyet.Any())
                {
                    // Cập nhật trạng thái thành "Đã duyệt" trong một transaction để đảm bảo tính toàn vẹn
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            foreach (var khachHang in khachHangsChoDuyet)
                            {
                                khachHang.trangThaiTaiKhoanKhachHang = "Đã duyệt";
                            }

                            db.SaveChanges();
                            transaction.Commit();

                            return Json(new { success = true, message = $"Đã duyệt thành công {khachHangsChoDuyet.Count} tài khoản!" });
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw; // Ném ngoại lệ để xử lý ở catch bên ngoài
                        }
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Không có tài khoản nào đang chờ duyệt." });
                }
            }
            catch (Exception ex)
            {
                // Log lỗi chi tiết (ví dụ: sử dụng log4net hoặc Serilog)
                // Logger.LogError(ex, "Lỗi khi duyệt hàng loạt khách hàng");
                return Json(new { success = false, message = "Có lỗi xảy ra. Vui lòng thử lại sau." });
            }
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
    }
}