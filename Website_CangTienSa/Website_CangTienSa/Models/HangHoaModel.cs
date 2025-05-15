namespace Website_CangTienSa.Models
{
    public class HangHoaModel
    {
        public string MaHangHoa { get; set; }
        public string TenHangHoa { get; set; }
        public float SoLuong { get; set; }
        public string DonViTinh { get; set; }
        public float DonGia { get; set; }
        public string MaContainer { get; set; } // Liên kết với container
        public string ViTriTrongKho { get; set; } // Từ container
        public string KichThuoc { get; set; } // Từ container
    }
}