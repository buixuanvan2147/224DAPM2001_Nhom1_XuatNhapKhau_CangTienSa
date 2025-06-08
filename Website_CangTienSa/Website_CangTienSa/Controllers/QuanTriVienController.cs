using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Website_CangTienSa.Models;
using System.Data.Entity;
using System.Linq.Dynamic;


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
                    SdtNhanVien  = nv.sdtNhanVien,
                    DiaChi = nv.diaChi,
                    Email = nv.email,
                    TrangThaiTaiKhoanNhanVien = nv.trangThaiTaiKhoanNhanVien,
                    ThoiGianDangNhapGanNhat = nv.thoiGianDangNhapGanNhat,
                    TenLoaiNhanVien = nv.vaiTroNhanVien.tenLoaiNhanVien
                })
                .ToList();
            ViewBag.NhanViens = allNhanVien;

            // Lấy ánh xạ vai trò cho modal phân quyền
            // Tạo ánh xạ vai trò cho modal phân quyền dưới dạng Dictionary
            var roleMappingDict = db.vaiTroNhanViens.ToDictionary(
                v => v.tenLoaiNhanVien,
                v => v.maVaiTroNhanVien
            );
            ViewBag.RoleMapping = roleMappingDict;

            return View();
        }

        public ActionResult QuanLyDonHang_QuanTriVien()
        {
            ViewBag.Title = "QuanLyDonHang_QuanTriVien";
            ViewBag.ActiveSidebar = "QuanLyDonHang";

            // Danh mục container để làm select option cho container
            // Đã thay đổi "maDanhMucContainer" thành "tenDanhMucContainer" cho cả valueField và textField
            ViewBag.dmContainers = new SelectList(db.danhMucContainers, "tenDanhMucContainer", "tenDanhMucContainer");

            // Danh sách container
            var allConTaiNer = db.containers
                .Include(ctn => ctn.danhMucContainer)
                .Select(ctn => new QTV_QLDH_ContainerViewModel
                {
                    MaContainer = ctn.maContainer,
                    TenMaDanhMucContainer = ctn.danhMucContainer.tenDanhMucContainer,
                    MaChiTietKho = ctn.maChiTietKho,
                    SoHieu = ctn.soHieu,
                    TrangThaiContainer = ctn.trangThaiContainer,
                    ViTriTrongKho = ctn.viTriTrongKho,
                    NgayMuaContainer = ctn.ngayMuaContainer,
                    TrongTai = ctn.trongTai
                })
                .ToList();
            ViewBag.ConTaiNers = allConTaiNer;

            // Danh sách phiếu nhập
            var allPhieuNhap = db.phieuNhaps.ToList();
            ViewBag.PhieuNhaps = allPhieuNhap;

            // Danh sách phiếu xuất
            var allPhieuXuat = db.phieuXuats.ToList();
            ViewBag.PhieuXuats = allPhieuXuat;

            // Trạng thái đơn hàng để làm select option cho đơn hàng
            // Đã thêm .Trim() để loại bỏ khoảng trắng thừa từ DB
            var ttdh = db.donHangs.Select(d => d.trangThaiDonHang.Trim()).Distinct().ToList();
            ViewBag.trangThaiDonHangs = ttdh;

            // Trạng thái thanh toán để làm select option cho đơn hàng
            // Đã thêm .Trim() để loại bỏ khoảng trắng thừa từ DB
            var tttt = db.donHangs.Select(d => d.trangThaiThanhToan.Trim()).Distinct().ToList();
            ViewBag.trangThaiThanhToans = tttt;

            // Danh sách đơn hàng
            var allDonHang = db.donHangs.ToList();
            ViewBag.DonHangs = allDonHang;

            return View();
        }

        public ActionResult ThongKeBaoCao_QuanTriVien()
        {
            ViewBag.Title = "ThongKeBaoCao_QuanTriVien";
            ViewBag.ActiveSidebar = "ThongKeBaoCao";

            //Tổng quan
            {
                // Số lượng đơn hàng đã hoàn thành và tổng số lượng đơn hàng
                double donHangDaHoanThanh = db.donHangs.Count(d => d.trangThaiDonHang == "Hoàn thành");
                double tongSoLuongDonHang = db.donHangs.Count();

                // Số lượng tồn kho và sức chứa tối đa
                double tongTonKho = db.khoes.Sum(k => k.tonKho);
                double tongSucChuaToiDa = db.khoes.Sum(k => k.sucChuaToiDa);

                // Tính mốc thời gian 6 tháng trước
                // Sử dụng DateTime.Today để lấy ngày hiện tại (không tính giờ)
                DateTime sixMonthsAgo = DateTime.Today.AddMonths(-6);

                var qtv_tongQuan = new QTV_TKBC_TongQuan
                {
                    //Tổng số khách hàng 
                    TongSoKhachHang = db.khachHangs.Count(),

                    //Tổng số khách hàng mới 6 tháng gần nhất
                    TongSoKhachHangMoi = db.khachHangs.Count(kh =>
                                        // Sử dụng DbFunctions.AddMonths cho EF6 hoặc EF.Functions.DateAdd cho EF Core
                                        // Ở đây, tôi sẽ dùng cách tạo mốc thời gian trước để tránh vấn đề dịch trực tiếp trong LINQ
                                        kh.ngayDangKy >= sixMonthsAgo &&
                                        kh.trangThaiTaiKhoanKhachHang == "Đã duyệt"), // Khách Hàng mới trong 6 tháng vừa qua
                    
                    //Tổng số nhan viên
                    TongSoNhanVien = db.nhanViens.Count(),

                    //Tổng số đơn hàng
                    TongSoDonHang = db.donHangs.Count(),

                    //Doanh thu 6 tháng gần nhất
                    TongSoDoanhThu6ThangQua = db.donHangs
                                            .Where(d => d.trangThaiDonHang == "Đã hoàn thành" &&
                                                        d.trangThaiThanhToan == "Đã thanh toán" &&
                                                        d.ngayNhapCang.HasValue && // Đảm bảo ngayNhapCang không null
                                                        d.ngayNhapCang.Value >= sixMonthsAgo) // Lọc 6 tháng gần nhất
                                            .Sum(d => (float?)d.tongTien) ?? 0,

                    // Tổng đơn hàng đang lưu kho
                    TongDonHangDangLuuKho = db.donHangs.Count(d =>
                                                d.ngayNhapCang != null && // Đã nhập kho
                                                d.ngayXuatCang == null && // Chưa xuất kho
                                                (d.trangThaiDonHang == "Đang xử lý" || d.trangThaiDonHang == "Đang yêu cầu")),
                    
                    //Tổng số container hiện có
                    TongSoContainerHienCo = db.containers.Count(c => c.trangThaiContainer == "Hoạt động"),

                    //Tỷ lệ % đơn hàng đã hoàn thành
                    TyLeDonHangHoanThanh = tongSoLuongDonHang == 0 ? 0 : (donHangDaHoanThanh / tongSoLuongDonHang) * 100,
                    
                    //Số phiếu nhập 6 tháng gần nhất
                    SoPhieuNhapMoi6ThangQua = db.donHangs.Count(p => p.ngayNhapCang.Value >= sixMonthsAgo),
                    
                    //Số phiếu xuất 6 tháng gần nhất
                    SoPhieuXuatMoi6ThangQua = db.phieuXuats.Count(p => p.ngayXuatKho >= sixMonthsAgo),
                    
                    //Công suất hoạt động của kho (= tồn kho / sức chứa * 100)
                    CongXuatSuDungKho = tongSucChuaToiDa == 0 ? 0 : (tongTonKho / tongSucChuaToiDa) * 100
                };

                ViewBag.TongQuan = qtv_tongQuan;
            }

            //Khách hàng
            {
                // Lấy thống kê tổng số khách hàng theo thời gian đăng ký mỗi 6 tháng
                {
                    var thongKeKhachHangTheo6Thang = db.khachHangs
                    .Where(kh => kh.ngayDangKy != null) // Đảm bảo ngày đăng ký có dữ liệu
                    .ToList() // Đưa dữ liệu về bộ nhớ trước để xử lý các hàm không hỗ trợ trong LINQ to Entities
                    .GroupBy(kh => new
                    {
                        Nam = kh.ngayDangKy.Year,
                        Ky6Thang = (kh.ngayDangKy.Month - 1) / 6 + 1 // 1 cho tháng 1-6, 2 cho 7-12
                    })
                    .Select(g => new QTV_TKBC_KhachHang_TheoThoiGianDangKy
                    {
                        // Tạo chuỗi hiển thị thời gian đăng ký
                        ThoiGianDangKy = $"Tháng {(g.Key.Ky6Thang == 1 ? "1–6" : "7–12")}/{g.Key.Nam}",

                        // Đếm số lượng khách hàng
                        SoLuongKhachHang = g.Count(),

                        // Mã thời gian để dùng khi xem chi tiết (ví dụ: "2024-H1", "2024-H2")
                        MaThoiGian = $"{g.Key.Nam}-H{g.Key.Ky6Thang}"
                    })
                    .OrderByDescending(s => s.MaThoiGian) // Sắp xếp theo thời gian mới nhất trước
                    .ToList(); // Đưa kết quả về danh sách để dùng tiếp

                    // Đưa danh sách vào ViewBag
                    ViewBag.ThongKeKhachHangTheo6Thang = thongKeKhachHangTheo6Thang;
                }

                // Lấy thống kê tổng số khách hàng theo mỗi trạng thái
                { 
                    // Định nghĩa tất cả các trạng thái tài khoản khách hàng
                    List<string> tatCaTrangThaiTaiKhoanHienThi = new List<string>
                                {
                                    "Đã duyệt",
                                    "Chờ duyệt",
                                    "Đang bị khóa",
                                    "Đã từ chối"
                                };
                    var duLieuTrangThaiKhachHangDaCo = db.khachHangs
                        .GroupBy(kh => kh.trangThaiTaiKhoanKhachHang)
                        .Select(g => new QTV_TKBC_KhachHang_TongKhachHangTheoTrangThai
                        {
                            TrangThai = g.Key,
                            SoLuong = g.Count()
                        })
                            .ToList();
                    // LEFT JOIN dữ liệu đã có với tất cả các trạng thái có thể có
                    var thongKeTrangThaiTaiKhoanKhachHangHoanChinh = tatCaTrangThaiTaiKhoanHienThi
                        .GroupJoin(duLieuTrangThaiKhachHangDaCo,
                                   tatCaTrangThai => tatCaTrangThai, // Key từ danh sách tất cả trạng thái
                                   duLieuDaCo => duLieuDaCo.TrangThai, // Key từ dữ liệu đã có
                                   (tatCaTrangThai, groupKhachHang) => new QTV_TKBC_KhachHang_TongKhachHangTheoTrangThai // Sử dụng Model đã đổi tên
                                   {
                                       TrangThai = tatCaTrangThai,
                                       SoLuong = groupKhachHang.Sum(kh => kh.SoLuong) // Sẽ là 0 nếu không có dữ liệu cho trạng thái này
                                   })
                            .OrderBy(s => s.TrangThai) // Sắp xếp để hiển thị theo thứ tự mong muốn
                            .ToList();
                    // Đưa danh sách vào ViewBag
                    ViewBag.ThongKeKhachHangTheoTrangThai = thongKeTrangThaiTaiKhoanKhachHangHoanChinh;
                }

                // Lấy Top 5 Khách Hàng Có Số Lượng Đơn Hàng Nhiều Nhất
                {
                    var topKhachHang = db.khachHangs // Tên DbSet cho bảng KháchHangs
                            .Join(db.donHangs, // Tên DbSet cho bảng DonHangs
                                  kh => kh.maKhachHang,
                                  dh => dh.maKhachHang,
                                  (kh, dh) => new { kh, dh })
                            .GroupBy(x => new { x.kh.maKhachHang, x.kh.tenKhachHang })
                            .Select(g => new QTV_TKBC_DonHang_Top5KhMaxDh
                            {
                                MaKhachHang = g.Key.maKhachHang,
                                TenKhachHang = g.Key.tenKhachHang,
                                SoLuongDonHang = g.Count() // Đếm số lượng đơn hàng trong mỗi nhóm
                            })
                            .OrderByDescending(x => x.SoLuongDonHang)
                            .Take(5) // Lấy 5 dòng đầu tiên
                            .ToList();
                    // Đưa danh sách vào ViewBag
                    ViewBag.Top5KhachHangDonHangNhieuNhat = topKhachHang;
                }
            }

            //Đơn hàng
            {
                // Lấy thống kê tổng số đơn hàng theo mỗi trạng thái hoàn thành
                {
                    // Định nghĩa tất cả các đơn hàng theo trạng thái hoàn thành
                    List<string> tatCaTrangThaiHoanThanhCuaDonHang = new List<string>
                                {
                                    "Hoàn thành",
                                    "Đang xử lý",
                                    "Đang vận chuyển xuất kho",
                                    "Đang yêu cầu"
                                };
                    var duLieuTrangThaiDonHangDaCo = db.donHangs
                        .GroupBy(kh => kh.trangThaiDonHang)
                        .Select(g => new QTV_TKBC_DonHang_TrangThaiHoanThanh
                        {
                            TrangThai = g.Key,
                            SoLuong = g.Count()
                        })
                            .ToList();
                    // LEFT JOIN dữ liệu đã có với tất cả các trạng thái có thể có
                    var thongKeTrangThaiHoanThanhCuaDonHang = tatCaTrangThaiHoanThanhCuaDonHang
                        .GroupJoin(duLieuTrangThaiDonHangDaCo,
                                   tatCaTrangThai => tatCaTrangThai, // Key từ danh sách tất cả trạng thái
                                   duLieuDaCo => duLieuDaCo.TrangThai, // Key từ dữ liệu đã có
                                   (tatCaTrangThai, groupDonHang) => new QTV_TKBC_DonHang_TrangThaiHoanThanh // Sử dụng Model đã đổi tên
                                   {
                                       TrangThai = tatCaTrangThai,
                                       SoLuong = groupDonHang.Sum(kh => kh.SoLuong) // Sẽ là 0 nếu không có dữ liệu cho trạng thái này
                                   })
                            .OrderBy(s => s.TrangThai) // Sắp xếp để hiển thị theo thứ tự mong muốn
                            .ToList();
                    // Đưa danh sách vào ViewBag
                    ViewBag.ThongKeDonHangTheoTrangThaiHoanThanh = thongKeTrangThaiHoanThanhCuaDonHang;
                }

                // Lấy thống kê tổng số đơn hàng theo mỗi trạng thái thanh toán
                {
                    // Định nghĩa tất cả các đơn hàng theo trạng thái thanh toán
                    List<string> tatCaTrangThaiThanhToanCuaDonHang = new List<string>
                                {
                                    "Đã thanh toán",
                                    "Chưa thanh toán"
                                };
                    var duLieuTrangThaiDonHangDaCo = db.donHangs
                        .GroupBy(kh => kh.trangThaiThanhToan)
                        .Select(g => new QTV_TKBC_DonHang_TrangThaiThanhToan
                        {
                            TrangThai = g.Key,
                            SoLuong = g.Count(),
                            TongGiaTri = g.Sum(x => x.tongTien)
                        })
                            .ToList();
                    // LEFT JOIN để đảm bảo tất cả trạng thái đều xuất hiện
                    var thongKeTrangThaiThanhToanCuaDonHang = tatCaTrangThaiThanhToanCuaDonHang
                        .GroupJoin(duLieuTrangThaiDonHangDaCo,
                                   tatCa => tatCa,
                                   duLieu => duLieu.TrangThai,
                                   (tatCa, groupDonHang) => new QTV_TKBC_DonHang_TrangThaiThanhToan
                                   {
                                       TrangThai = tatCa,
                                       SoLuong = groupDonHang.Sum(x => x.SoLuong),
                                       TongGiaTri = groupDonHang.Sum(x => x.TongGiaTri)
                                   })
                        .OrderBy(x => x.TrangThai)
                        .ToList();

                    // Truyền dữ liệu sang View
                    ViewBag.ThongKeDonHangTheoTrangThaiThanhToan = thongKeTrangThaiThanhToanCuaDonHang;
                }

                // Thống kê hàng hóa đang lưu kho
                {
                    var hangHoaDangLuuKho = db.chiTietDonHangs
                        .Where(ct => ct.donHang.trangThaiDonHang == "Đang xử lý" || ct.donHang.trangThaiDonHang == "Đang yêu cầu")
                        .GroupBy(ct => new
                        {
                            ct.hangHoa.maHangHoa,
                            ct.hangHoa.tenHangHoa,
                            ct.hangHoa.donViTinh,
                            ct.hangHoa.maDanhMucHangHoa,
                            TenDanhMuc = ct.hangHoa.danhMucHangHoa.tenDanhMucHangHoa
                        })
                        .Select(g => new QTV_TKBC_DonHang_HangHoaDangLuuKho
                        {
                            MaHangHoa = g.Key.maHangHoa,
                            TenHangHoa = g.Key.tenHangHoa,
                            TenDanhMuc = g.Key.TenDanhMuc,
                            DonViTinh = g.Key.donViTinh,
                            SoLuong = g.Sum(x => x.soLuong)
                        })
                        .OrderBy(x => x.TenHangHoa)
                        .ToList();

                    // Truyền dữ liệu sang View
                    ViewBag.HangHoaDangLuuKho = hangHoaDangLuuKho;
                }
            }

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