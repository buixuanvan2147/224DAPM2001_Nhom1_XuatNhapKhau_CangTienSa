using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Website_CangTienSa.Models;

namespace Website_CangTienSa.DAO
{
    public class DonHangDAO
    {
        private readonly string connectionString = "Server=DINHTHI\\DINHTHIMSSQLSV;Database=XuatNhapHangTaiCangTienSa;Trusted_Connection=True;";

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
                dh.trangThaiDonHang AS trangThai,
                nv.tenHienThi AS nguoiXacNhan
            FROM donHang dh
            JOIN chiTietDonHang ctdh ON dh.maDonHang = ctdh.maDonHang
            JOIN nhanVien nv ON dh.maNhanVien = nv.maNhanVien
            WHERE dh.trangThaiDonHang = N'Đang vận chuyển'";

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
                            TrangThai = reader["trangThai"].ToString(),
                            NguoiXacNhan = reader["nguoiXacNhan"].ToString()
                        };
                        donHangList.Add(donHang);
                    }
                }
            }

            return donHangList;
        }

        public bool ConfirmTransport(string maPhieuXuat, string nguoiXacNhan)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
            UPDATE donHang
            SET trangThaiDonHang = N'Hoàn Thành', nguoiXacNhan = @NguoiXacNhan
            FROM donHang dh
            JOIN chiTietPhieuXuat ctpx ON dh.maDonHang = ctpx.maDonHang
            WHERE ctpx.maPhieuXuat = @MaPhieuXuat";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MaPhieuXuat", maPhieuXuat);
                    command.Parameters.AddWithValue("@NguoiXacNhan", nguoiXacNhan);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
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
            // Giảm độ dài mã xuống còn 8 ký tự (2 ký tự prefix + 6 số)
            string query = "SELECT 'PN' + RIGHT('000000' + CAST(NEXT VALUE FOR Seq_PhieuNhap AS VARCHAR(6)), 6)";
            using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
            {
                return cmd.ExecuteScalar()?.ToString();
            }
        }

        private string GenerateChiTietPhieuNhapCode(SqlConnection connection, SqlTransaction transaction)
        {
            // Giảm độ dài mã xuống còn 8 ký tự (4 ký tự prefix + 4 số)
            string query = "SELECT 'CTPN' + RIGHT('0000' + CAST(NEXT VALUE FOR Seq_ChiTietPhieuNhap AS VARCHAR(4)), 4)";
            using (SqlCommand cmd = new SqlCommand(query, connection, transaction))
            {
                return cmd.ExecuteScalar()?.ToString();
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