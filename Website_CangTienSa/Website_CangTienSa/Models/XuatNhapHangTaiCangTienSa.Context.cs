﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class XuatNhapHangTaiCangTienSaEntities : DbContext
    {
        public XuatNhapHangTaiCangTienSaEntities()
            : base("name=XuatNhapHangTaiCangTienSaEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<chiTietDonHang> chiTietDonHangs { get; set; }
        public virtual DbSet<chiTietKho> chiTietKhoes { get; set; }
        public virtual DbSet<chiTietPhieuNhap> chiTietPhieuNhaps { get; set; }
        public virtual DbSet<container> containers { get; set; }
        public virtual DbSet<danhMucContainer> danhMucContainers { get; set; }
        public virtual DbSet<danhMucHangHoa> danhMucHangHoas { get; set; }
        public virtual DbSet<donHang> donHangs { get; set; }
        public virtual DbSet<hangHoa> hangHoas { get; set; }
        public virtual DbSet<khachHang> khachHangs { get; set; }
        public virtual DbSet<kho> khoes { get; set; }
        public virtual DbSet<loaiKho> loaiKhoes { get; set; }
        public virtual DbSet<nhanVien> nhanViens { get; set; }
        public virtual DbSet<phieuNhap> phieuNhaps { get; set; }
        public virtual DbSet<phieuXuat> phieuXuats { get; set; }
        public virtual DbSet<vaiTroNhanVien> vaiTroNhanViens { get; set; }
        public object PhieuXuat { get; internal set; }
    }
}
