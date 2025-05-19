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
            ViewBag.Title = "Quản Lý Tài Khoản Khách Hàng";
            ViewBag.ActiveSidebar = "Quản Lý Tài Khoản";

            var allKhachHang = db.khachHangs.ToList();

            return View( allKhachHang);
        }

        public ActionResult TaoTaoKhoan_QuanTriVien()
        {
            ViewBag.vaiTroNhanVienList = new SelectList(db.vaiTroNhanViens, "maVaiTroNhanVien", "tenLoaiNhanVien");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TaoTaoKhoan_QuanTriVien(nhanVien model, HttpPostedFileBase anhDaiDienNhanVienUrl)
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