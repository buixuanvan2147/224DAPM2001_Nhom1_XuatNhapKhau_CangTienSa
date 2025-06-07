using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Website_CangTienSa.Models;

namespace Website_CangTienSa.DAO
{
    public class DonHangDAO
    {
        private readonly string connectionString = "Server=BUIXUANVANPC\\MSSQLSERVER2022;Database=XuatNhapHangTaiCangTienSa;Trusted_Connection=True;";
        public List<DonHangModel> GetDonHangDangVanChuyen()
        {
            var donHangList = new List<DonHangModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
            SELECT 
                dh.maDonHang,
                dh.tenNguoiGui,
                dh.ngayNhapCang,
                ctdh.soLuong,
                dh.trangThaiThanhToan AS trangThai,
                nv.tenHienThi AS nguoiXacNhan,
                dh.moTa
            FROM donHang dh
            JOIN chiTietDonHang ctdh ON dh.maDonHang = ctdh.maDonHang
            JOIN nhanVien nv ON dh.maNhanVien = nv.maNhanVien
            WHERE dh.moTa LIKE N'%Đơn hàng nhập khẩu%'
            AND dh.trangThaiThanhToan = N'Đã thanh toán' AND dh.trangThaiDonHang = N'Đang vận chuyển nhập kho'";

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var donHang = new DonHangModel
                        {
                            MaDonHang = reader["maDonHang"].ToString(),
                            TenNguoiGui = reader["tenNguoiGui"].ToString(),
                            NgayNhapCang = reader["ngayNhapCang"] != DBNull.Value ? Convert.ToDateTime(reader["ngayNhapCang"]) : (DateTime?)null,
                            SoLuong = reader["soLuong"] != DBNull.Value ? Convert.ToSingle(reader["soLuong"]) : 0,
                            TrangThai = reader["trangThai"].ToString(), // Lấy trạng thái thanh toán
                            NguoiXacNhan = reader["nguoiXacNhan"].ToString(),
                            MoTa = reader["moTa"].ToString()
                        };
                        donHangList.Add(donHang);
                    }
                }
            }

            return donHangList;
        }
        public List<DonHangModel> GetDonHangXuatKhau()
        {
            var donHangList = new List<DonHangModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
            SELECT 
                dh.maDonHang,
                dh.tenNguoiGui,
                dh.ngayNhapCang,
                ctdh.soLuong,
                dh.trangThaiThanhToan AS trangThai,
                dh.trangThaiDonHang,
                nv.tenHienThi AS nguoiXacNhan,
                dh.moTa
            FROM donHang dh
            JOIN chiTietDonHang ctdh ON dh.maDonHang = ctdh.maDonHang
            JOIN nhanVien nv ON dh.maNhanVien = nv.maNhanVien
            WHERE dh.moTa LIKE N'%Đơn hàng xuất khẩu%'
            AND dh.trangThaiThanhToan = N'Đã thanh toán' AND dh.trangThaiDonHang = N'Đang vận chuyển xuất kho'
            AND dh.trangThaiDonHang NOT IN (N'Đang xử lý', N'Đang yêu cầu')";

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var donHang = new DonHangModel
                        {
                            MaDonHang = reader["maDonHang"].ToString(),
                            TenNguoiGui = reader["tenNguoiGui"].ToString(),
                            NgayNhapCang = reader["ngayNhapCang"] != DBNull.Value ? Convert.ToDateTime(reader["ngayNhapCang"]) : (DateTime?)null,
                            SoLuong = reader["soLuong"] != DBNull.Value ? Convert.ToSingle(reader["soLuong"]) : 0,
                            TrangThai = reader["trangThai"].ToString(), // Lấy trạng thái thanh toán
                            TrangThaiDonHang = reader["trangThaiDonHang"].ToString(), // Lấy trạng thái đơn hàng
                            NguoiXacNhan = reader["nguoiXacNhan"].ToString(),
                            MoTa = reader["moTa"].ToString()
                        };
                        donHangList.Add(donHang);
                    }
                }
            }

            return donHangList;
        }

        public DonHangModel GetChiTietDonHang(string maDonHang)
        {
            DonHangModel donHang = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
            SELECT 
                dh.maDonHang,
                dh.tenNguoiGui,
                dh.ngayNhapCang,
                ctdh.soLuong,
                ctdh.maHangHoa,
                hh.tenHangHoa AS tenSanPham,
                hh.donViTinh,
                ctn.maContainer,
                dh.trangThaiDonHang AS trangThai,
                nv.tenHienThi AS nguoiXacNhan
            FROM donHang dh
            JOIN chiTietDonHang ctdh ON dh.maDonHang = ctdh.maDonHang
            JOIN hangHoa hh ON ctdh.maHangHoa = hh.maHangHoa
            LEFT JOIN chiTietPhieuNhap ctpn ON dh.maDonHang = ctpn.maPhieuNhap
            LEFT JOIN container ctn ON ctpn.maContainer = ctn.maContainer
            JOIN nhanVien nv ON dh.maNhanVien = nv.maNhanVien
            WHERE dh.maDonHang = @MaDonHang";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MaDonHang", maDonHang);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            donHang = new DonHangModel
                            {
                                MaDonHang = reader["maDonHang"].ToString(),
                                TenNguoiGui = reader["tenNguoiGui"].ToString(),
                                NgayNhapCang = reader["ngayNhapCang"] != DBNull.Value ? Convert.ToDateTime(reader["ngayNhapCang"]) : (DateTime?)null,
                                MaContainer = reader["maContainer"]?.ToString(),
                                MaHangHoa = reader["maHangHoa"]?.ToString(),
                                TenSanPham = reader["tenSanPham"]?.ToString(),
                                DonViTinh = reader["donViTinh"]?.ToString(),
                                SoLuong = reader["soLuong"] != DBNull.Value ? Convert.ToSingle(reader["soLuong"]) : 0,
                                TrangThai = reader["trangThai"].ToString(),
                                NguoiXacNhan = reader["nguoiXacNhan"].ToString()
                            };
                        }
                    }
                }
            }

            return donHang;
        }

        public bool UpdateTrangThaiVaNgayXuatCang(string maDonHang, string trangThaiMoi, DateTime ngayXuatCang)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Kiểm tra đơn hàng tồn tại
                    string checkQuery = "SELECT maDonHang FROM donHang WHERE maDonHang = @MaDonHang";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, connection, transaction))
                    {
                        checkCmd.Parameters.AddWithValue("@MaDonHang", maDonHang);
                        var result = checkCmd.ExecuteScalar();
                        if (result == null)
                        {
                            System.Diagnostics.Debug.WriteLine($"Đơn hàng {maDonHang} không tồn tại.");
                            transaction.Rollback();
                            return false;
                        }
                    }

                    // Cập nhật trạng thái và ngày xuất cảng
                    string query = @"
                UPDATE donHang 
                SET 
                    trangThaiDonHang = @TrangThai,
                    ngayXuatCang = CASE 
                        WHEN ngayXuatCang IS NULL THEN @NgayXuatCang 
                        ELSE ngayXuatCang 
                    END
                WHERE maDonHang = @MaDonHang";

                    using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
                    {
                        cmd.Parameters.AddWithValue("@TrangThai", trangThaiMoi);
                        cmd.Parameters.AddWithValue("@NgayXuatCang", ngayXuatCang);
                        cmd.Parameters.AddWithValue("@MaDonHang", maDonHang);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected == 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"Không thể cập nhật đơn hàng {maDonHang}.");
                            transaction.Rollback();
                            return false;
                        }
                    }

                    transaction.Commit();
                    System.Diagnostics.Debug.WriteLine($"Cập nhật thành công: maDonHang={maDonHang}, trangThai={trangThaiMoi}, ngayXuatCang={ngayXuatCang}");
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    System.Diagnostics.Debug.WriteLine($"Lỗi khi cập nhật trạng thái và ngày xuất cảng: {ex.Message}");
                    return false;
                }
            }
        }
        public bool GanDonHangVaoContainer(string maDonHang, string maContainer)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Log dữ liệu đầu vào
                    Console.WriteLine($"DAO Input - maDonHang: '{maDonHang}', maContainer: '{maContainer}'");
                    Console.WriteLine($"DAO Input Length - maDonHang: {Encoding.UTF8.GetByteCount(maDonHang)}, maContainer: {Encoding.UTF8.GetByteCount(maContainer)}");

                    // Kiểm tra độ dài và trim
                    if (maDonHang.Length > 10 || maContainer.Length > 10)
                    {
                        throw new Exception($"Dữ liệu không hợp lệ: maDonHang ({maDonHang.Length}) hoặc maContainer ({maContainer.Length}) vượt quá 10 ký tự.");
                    }

                    // Kiểm tra dữ liệu
                    if (!ValidateContainerAndOrder(connection, transaction, maContainer, maDonHang))
                    {
                        throw new Exception("Dữ liệu không hợp lệ: Container không rỗng hoặc không tồn tại, hoặc đơn hàng không tồn tại.");
                    }

                    // Sinh mã
                    string maPhieuNhap = GeneratePhieuNhapCode(connection, transaction);
                    string maChiTietPhieuNhap = GenerateChiTietPhieuNhapCode(connection, transaction);

                    // Log mã đã sinh
                    Console.WriteLine($"DAO Generated - maPhieuNhap: '{maPhieuNhap}', maChiTietPhieuNhap: '{maChiTietPhieuNhap}'");

                    // Chèn dữ liệu
                    InsertPhieuNhap(connection, transaction, maPhieuNhap, maDonHang, maContainer);
                    InsertChiTietPhieuNhap(connection, transaction, maChiTietPhieuNhap, maPhieuNhap, maContainer);

                    // Cập nhật container
                    UpdateContainerStatus(connection, transaction, maContainer);

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception($"Lỗi khi thêm đơn hàng vào container: {ex.Message}");
                }
            }
        }

        private bool ValidateContainerAndOrder(SqlConnection connection, SqlTransaction transaction, string maContainer, string maDonHang)
        {
            // Kiểm tra container
            string checkContainerQuery = "SELECT trangThaiContainer FROM container WHERE maContainer = @maContainer";
            using (SqlCommand cmd = new SqlCommand(checkContainerQuery, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@maContainer", maContainer);
                var status = cmd.ExecuteScalar()?.ToString();
                if (string.IsNullOrEmpty(status) || status != "Rỗng")
                {
                    return false;
                }
            }

            // Kiểm tra đơn hàng
            string checkOrderQuery = "SELECT maDonHang FROM donHang WHERE maDonHang = @maDonHang";
            using (SqlCommand cmd = new SqlCommand(checkOrderQuery, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@maDonHang", maDonHang);
                var donHangId = cmd.ExecuteScalar()?.ToString();
                if (string.IsNullOrEmpty(donHangId))
                {
                    return false;
                }
            }

            return true;
        }

        private string GeneratePhieuNhapCode(SqlConnection connection, SqlTransaction transaction)
        {
            // Tạo mã phiếu nhập dạng PN + số tự tăng (6 chữ số)
            string query = "SELECT COUNT(*) FROM phieuNhap";
            using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
            {
                int count = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
                return "PN" + count.ToString("D6");
            }
        }

        private string GenerateChiTietPhieuNhapCode(SqlConnection connection, SqlTransaction transaction)
        {
            // Tạo mã chi tiết phiếu nhập dạng CTPN + số tự tăng (6 chữ số)
            string query = "SELECT COUNT(*) FROM chiTietPhieuNhap";
            using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
            {
                int count = Convert.ToInt32(cmd.ExecuteScalar()) + 1;
                return "CTPN" + count.ToString("D6");
            }
        }
        public bool UpdateTrangThaiDonHang(string maDonHang, string trangThaiMoi)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE donHang SET trangThaiDonHang = @TrangThai WHERE maDonHang = @MaDonHang";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@TrangThai", trangThaiMoi);
                    cmd.Parameters.AddWithValue("@MaDonHang", maDonHang);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
    
        private void InsertChiTietPhieuNhap(SqlConnection connection, SqlTransaction transaction, string maChiTietPhieuNhap, string maPhieuNhap, string maContainer)
        {
            string moTa = "Chi tiết nhập container " + maContainer;
            if (moTa.Length > 255)
            {
                moTa = moTa.Substring(0, 255);
            }

            // Log trước khi chèn
            Console.WriteLine($"DAO InsertChiTietPhieuNhap - maChiTietPhieuNhap: '{maChiTietPhieuNhap}', maPhieuNhap: '{maPhieuNhap}', maContainer: '{maContainer}', moTa: '{moTa}'");
            Console.WriteLine($"DAO InsertChiTietPhieuNhap Length - maChiTietPhieuNhap: {Encoding.UTF8.GetByteCount(maChiTietPhieuNhap)}, maPhieuNhap: {Encoding.UTF8.GetByteCount(maPhieuNhap)}, maContainer: {Encoding.UTF8.GetByteCount(maContainer)}, moTa: {Encoding.UTF8.GetByteCount(moTa)}");

            string query = "INSERT INTO chiTietPhieuNhap (maChiTietPhieuNhap, maPhieuNhap, maContainer, moTa) VALUES (@maCTPN, @maPhieuNhap, @maContainer, @moTa)";
            using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@maCTPN", maChiTietPhieuNhap);
                cmd.Parameters.AddWithValue("@maPhieuNhap", maPhieuNhap);
                cmd.Parameters.AddWithValue("@maContainer", maContainer);
                cmd.Parameters.AddWithValue("@moTa", moTa);
                cmd.ExecuteNonQuery();
            }
        }
        private void InsertPhieuNhap(SqlConnection connection, SqlTransaction transaction, string maPhieuNhap, string maDonHang, string maContainer)
        {
            string moTa = "Nhập hàng vào container " + maContainer;
            // Đảm bảo mô tả không vượt quá 255 ký tự
            moTa = moTa.Length > 255 ? moTa.Substring(0, 255) : moTa;

            // Log giá trị trước khi insert
            Console.WriteLine($"DAO - maPhieuNhap: {maPhieuNhap} (length: {maPhieuNhap.Length})");
            Console.WriteLine($"DAO - maDonHang: {maDonHang} (length: {maDonHang.Length})");

            string query = "INSERT INTO phieuNhap (maPhieuNhap, maDonHang, moTa) VALUES (@maPhieuNhap, @maDonHang, @moTa)";
            using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@maPhieuNhap", maPhieuNhap);
                cmd.Parameters.AddWithValue("@maDonHang", maDonHang);
                cmd.Parameters.AddWithValue("@moTa", moTa);
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateContainerStatus(SqlConnection connection, SqlTransaction transaction, string maContainer)
        {
            string query = "UPDATE container SET trangThaiContainer = N'Đang sử dụng' WHERE maContainer = @maContainer";
            using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
            {
                cmd.Parameters.AddWithValue("@maContainer", maContainer);
                cmd.ExecuteNonQuery();
            }
        }

        public List<ContainerModel> GetDanhSachContainerRong()
        {
            var list = new List<ContainerModel>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
            SELECT c.maContainer, c.viTriTrongKho, k.tenKho, lk.tenLoaiKho
            FROM container c
            JOIN chiTietKho ctk ON c.maChiTietKho = ctk.maChiTietKho
            JOIN kho k ON ctk.maKho = k.maKho
            JOIN loaiKho lk ON k.maLoaiKho = lk.maLoaiKho
            WHERE c.trangThaiContainer = N'Rỗng'";

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ContainerModel
                        {
                            MaContainer = reader["maContainer"].ToString(),
                            ViTriTrongKho = reader["viTriTrongKho"].ToString(),
                            TenKho = reader["tenKho"].ToString(),
                            TenLoaiKho = reader["tenLoaiKho"].ToString()
                        });
                    }
                }
            }
            return list;
        }
    }
}