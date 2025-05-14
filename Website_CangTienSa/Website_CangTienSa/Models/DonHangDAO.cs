using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Website_CangTienSa.Models;

namespace Website_CangTienSa.DAO
{
    public class DonHangDAO
    {
        private readonly string connectionString = "Server=MINHTOAN\\SQLEXPRESS;Database=XuatNhapHangTaiCangTienSa;Trusted_Connection=True;";

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
            SET trangThai = N'Hoàn Thành', nguoiXacNhan = @NguoiXacNhan
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
            ctn.maContainer,
            dh.trangThaiDonHang AS trangThai,
            nv.tenHienThi AS nguoiXacNhan
        FROM donHang dh
        JOIN chiTietDonHang ctdh ON dh.maDonHang = ctdh.maDonHang
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

    }
}