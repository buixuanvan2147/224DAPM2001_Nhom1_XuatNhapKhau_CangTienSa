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
    
    public partial class danhMucContainer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public danhMucContainer()
        {
            this.containers = new HashSet<container>();
        }
    
        public string maDanhMucContainer { get; set; }
        public string tenDanhMucContainer { get; set; }
        public double sucChuaToiDa { get; set; }
        public double trongTaiToiDa { get; set; }
        public double chieuDai { get; set; }
        public double chieuRong { get; set; }
        public double chieuCao { get; set; }
        public string moTa { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<container> containers { get; set; }
    }
}
