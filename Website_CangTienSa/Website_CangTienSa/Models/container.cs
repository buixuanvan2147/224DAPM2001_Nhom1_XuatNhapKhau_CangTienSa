//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Website_CangTienSa.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class container
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public container()
        {
            this.chiTietPhieuNhaps = new HashSet<chiTietPhieuNhap>();
        }
    
        public string maContainer { get; set; }
        public string maDanhMucContainer { get; set; }
        public string maChiTietKho { get; set; }
        public string soHieu { get; set; }
        public string trangThaiContainer { get; set; }
        public string viTriTrongKho { get; set; }
        public Nullable<System.DateTime> ngayMuaContainer { get; set; }
        public double trongTai { get; set; }
    
        public virtual chiTietKho chiTietKho { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<chiTietPhieuNhap> chiTietPhieuNhaps { get; set; }
        public virtual danhMucContainer danhMucContainer { get; set; }
    }
}
