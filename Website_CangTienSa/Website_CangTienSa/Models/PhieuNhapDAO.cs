using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Website_CangTienSa.Models;

namespace Website_CangTienSa.DAO
{
    public class PhieuNhapDAO
    {
        private readonly string connectionString = "Server=DINHTHI\\DINHTHIMSSQLSV;Database=XuatNhapHangTaiCangTienSa;Trusted_Connection=True;";

        public List<PhieuNhapModel> GetAllPhieuNhap()
        {
            List<PhieuNhapModel> phieuNhapList = new List<PhieuNhapModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Truy vấn lấy danh sách phiếu nhập và thông tin liên quan
                string query = @"
                    SELECT 
                        pn.maPhieuNhap,
                        dh.ngayNhapCang,
                        dh.tenNguoiGui,
                        k.tenKho
                    FROM phieuNhap pn
                    JOIN donHang dh ON pn.maDonHang = dh.maDonHang
                    JOIN chiTietPhieuNhap ctpn ON pn.maPhieuNhap = ctpn.maPhieuNhap
                    JOIN container c ON ctpn.maContainer = c.maContainer
                    JOIN chiTietKho ctk ON c.maChiTietKho = ctk.maChiTietKho
                    JOIN kho k ON ctk.maKho = k.maKho
                    GROUP BY pn.maPhieuNhap, dh.ngayNhapCang, dh.tenNguoiGui, k.tenKho";

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PhieuNhapModel phieuNhap = new PhieuNhapModel
                        {
                            MaPhieuNhap = reader["maPhieuNhap"].ToString(),
                            NgayNhapCang = reader["ngayNhapCang"] != DBNull.Value ? Convert.ToDateTime(reader["ngayNhapCang"]) : (DateTime?)null,
                            TenNguoiGui = reader["tenNguoiGui"].ToString(),
                            TenKho = reader["tenKho"].ToString()
                        };
                        phieuNhapList.Add(phieuNhap);
                    }
                }

                // Lấy danh sách chi tiết hàng hóa và container cho từng phiếu nhập
                foreach (var phieuNhap in phieuNhapList)
                {
                    string detailQuery = @"
                        SELECT 
                            ctdh.maHangHoa,
                            hh.tenHangHoa,
                            ctdh.soLuong,
                            ctdh.donViTinh,
                            ctdh.donGia,
                            c.maContainer,
                            c.viTriTrongKho,
                            CONCAT(dmc.chieuDai, 'x', dmc.chieuRong, 'x', dmc.chieuCao) AS kichThuoc
                        FROM phieuNhap pn
                        JOIN donHang dh ON pn.maDonHang = dh.maDonHang
                        JOIN chiTietDonHang ctdh ON dh.maDonHang = ctdh.maDonHang
                        JOIN hangHoa hh ON ctdh.maHangHoa = hh.maHangHoa
                        JOIN chiTietPhieuNhap ctpn ON pn.maPhieuNhap = ctpn.maPhieuNhap
                        JOIN container c ON ctpn.maContainer = c.maContainer
                        JOIN danhMucContainer dmc ON c.maDanhMucContainer = dmc.maDanhMucContainer
                        WHERE pn.maPhieuNhap = @MaPhieuNhap";

                    using (SqlCommand command = new SqlCommand(detailQuery, connection))
                    {
                        command.Parameters.AddWithValue("@MaPhieuNhap", phieuNhap.MaPhieuNhap);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                HangHoaModel item = new HangHoaModel
                                {
                                    MaHangHoa = reader["maHangHoa"].ToString(),
                                    TenHangHoa = reader["tenHangHoa"].ToString(),
                                    SoLuong = reader["soLuong"] != DBNull.Value ? Convert.ToSingle(reader["soLuong"]) : 0,
                                    DonViTinh = reader["donViTinh"].ToString(),
                                    DonGia = reader["donGia"] != DBNull.Value ? Convert.ToSingle(reader["donGia"]) : 0,
                                    MaContainer = reader["maContainer"].ToString(),
                                    ViTriTrongKho = reader["viTriTrongKho"].ToString(),
                                    KichThuoc = reader["kichThuoc"].ToString()
                                };
                                phieuNhap.Items.Add(item);
                            }
                        }
                    }
                }
            }

            return phieuNhapList;
        }

        // Phương thức để lấy danh sách kho (trả về List<string> cho dropdown)
        public List<string> GetAllKho()
        {
            List<string> khoList = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT tenKho FROM kho";
                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        khoList.Add(reader["tenKho"].ToString());
                    }
                }
            }

            return khoList;
        }

        // Phương thức mới để lấy danh sách kho dưới dạng List<KhoModel>
        public List<KhoModel> GetAllKhoList()
        {
            List<KhoModel> khoList = new List<KhoModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT 
                        maKho,
                        tenKho,
                        maLoaiKho,
                        sucChuaToiDa,
                        tonKho,
                        trangThaiKho
                    FROM kho";

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        KhoModel kho = new KhoModel
                        {
                            MaKho = reader["maKho"].ToString(),
                            TenKho = reader["tenKho"].ToString(),
                            MaLoaiKho = reader["maLoaiKho"].ToString(),
                            SucChuaToiDa = reader["sucChuaToiDa"] != DBNull.Value ? Convert.ToSingle(reader["sucChuaToiDa"]) : 0,
                            TonKho = reader["tonKho"] != DBNull.Value ? Convert.ToSingle(reader["tonKho"]) : 0,
                            TrangThaiKho = reader["trangThaiKho"].ToString()
                        };
                        khoList.Add(kho);
                    }
                }
            }

            return khoList;
        }
    }
}