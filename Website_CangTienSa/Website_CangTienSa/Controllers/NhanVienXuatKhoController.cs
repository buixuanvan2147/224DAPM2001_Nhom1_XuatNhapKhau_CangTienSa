﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Website_CangTienSa.Models;
using System.Data.Entity;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Website_CangTienSa.Controllers
{
    public class NhanVienXuatKhoController : Controller
    {
        private readonly XuatNhapHangTaiCangTienSaEntities db = new XuatNhapHangTaiCangTienSaEntities();

        // GET: Trang danh sách
        public ActionResult Index_NhanVienXuatKho()
        {
            var donHangs = db.donHangs
                .Include(d => d.khachHang)
                .ToList();
            return View(donHangs);
        }

        // GET: Chi tiết đơn hàng
        public ActionResult ChiTietDonHang(string id)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            var donHang = db.donHangs
                .Include(d => d.khachHang)
                .FirstOrDefault(d => d.maDonHang == id);

            if (donHang == null)
                return HttpNotFound();

            var chiTiet = db.chiTietDonHangs
                .Where(c => c.maDonHang == id)
                .ToList();

            var viewModel = new DonHangChiTietViewModel
            {
                DonHang = donHang,
                ChiTietDonHang = chiTiet
            };

            return View(viewModel);
        }
        // GET: Hiển thị trang chi tiết phiếu xuất kho (dữ liệu + nút xuất phiếu)
        [HttpGet]
        public ActionResult XuatPhieuXuatKho(string id)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            var donHang = db.donHangs.Include(d => d.khachHang).FirstOrDefault(d => d.maDonHang == id);
            if (donHang == null)
                return HttpNotFound();

            var chiTietDonHangs = db.chiTietDonHangs.Where(c => c.maDonHang == id).ToList();

            var phieuXuat = db.phieuXuats.Where(p => p.maDonHang == id)
                                         .OrderByDescending(p => p.ngayXuatKho)
                                         .FirstOrDefault();

            // Tạo ViewModel (tùy bạn tự tạo nếu cần)
            var viewModel = new PhieuXuatKhoViewModel
            {
                DonHang = donHang,
                ChiTietDonHang = chiTietDonHangs,
                PhieuXuat = phieuXuat
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult XuatKho(string id)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            var donHang = db.donHangs.FirstOrDefault(d => d.maDonHang == id);
            if (donHang == null)
                return HttpNotFound();

            // Cập nhật trạng thái
            donHang.trangThaiDonHang = "Đang gửi yêu cầu";

            // Gán nhân viên kho bãi 

            db.SaveChanges();

            return RedirectToAction("Index_NhanVienXuatKho");
        }


        // POST: Thực hiện tạo và trả về file Word khi người dùng nhấn nút "Xuất phiếu"
        [HttpPost]
        public ActionResult XuatPhieuXuatKho(string id, FormCollection form)
        {
            if (string.IsNullOrEmpty(id))
                return HttpNotFound();

            var donHang = db.donHangs.Include(d => d.khachHang).FirstOrDefault(d => d.maDonHang == id);
            if (donHang == null)
                return HttpNotFound();

            var chiTietDonHangs = db.chiTietDonHangs.Where(c => c.maDonHang == id).ToList();

            var phieuXuat = db.phieuXuats.Where(p => p.maDonHang == id)
                                         .OrderByDescending(p => p.ngayXuatKho)
                                         .FirstOrDefault();

            using (MemoryStream ms = new MemoryStream())
            {
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
                {
                    MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
                    mainPart.Document = new Document();
                    Body body = new Body();

                    // Tiêu đề
                    Paragraph title = new Paragraph(new Run(new Text("Phiếu xuất kho")));
                    title.ParagraphProperties = new ParagraphProperties
                    {
                        Justification = new Justification() { Val = JustificationValues.Center },
                        ParagraphStyleId = new ParagraphStyleId() { Val = "Title" }
                    };
                    body.AppendChild(title);
                    body.AppendChild(new Paragraph(new Run(new Text(""))));

                    void AppendParagraph(string text)
                    {
                        body.AppendChild(new Paragraph(new Run(new Text(text))));
                    }

                    AppendParagraph($"Mã đơn hàng: {donHang.maDonHang}");
                    AppendParagraph($"Mã phiếu xuất: {(phieuXuat != null ? phieuXuat.maPhieuXuat : "Chưa có phiếu xuất")}");
                    AppendParagraph($"Khách hàng: {(donHang.khachHang != null ? donHang.khachHang.tenKhachHang : "Không xác định")}");
                    AppendParagraph($"Ngày tạo đơn hàng: {donHang.ngayTaoDonHang.ToString("dd/MM/yyyy")}");
                    AppendParagraph($"Trạng thái xuất hàng: {(phieuXuat != null ? phieuXuat.trangThaiXuatHang : "Chưa có trạng thái")}");
                    AppendParagraph($"Ngày xuất kho: {(phieuXuat != null ? phieuXuat.ngayXuatKho.ToString("dd/MM/yyyy") : "Chưa có ngày xuất")}");
                    AppendParagraph($"Mô tả phiếu xuất: {(phieuXuat != null ? phieuXuat.moTa : "")}");

                    body.AppendChild(new Paragraph(new Run(new Text(""))));

                    // Tạo bảng chi tiết
                    Table table = new Table();

                    TableProperties tblProp = new TableProperties(
                        new TableBorders(
                            new TopBorder() { Val = BorderValues.Single, Size = 4 },
                            new BottomBorder() { Val = BorderValues.Single, Size = 4 },
                            new LeftBorder() { Val = BorderValues.Single, Size = 4 },
                            new RightBorder() { Val = BorderValues.Single, Size = 4 },
                            new InsideHorizontalBorder() { Val = BorderValues.Single, Size = 4 },
                            new InsideVerticalBorder() { Val = BorderValues.Single, Size = 4 }
                        )
                    );
                    table.AppendChild(tblProp);

                    // Header row
                    TableRow headerRow = new TableRow();
                    string[] headers = { "Mã chi tiết", "Mã hàng hóa", "Số lượng", "Đơn vị tính", "Chất lượng", "Đơn giá", "Tiền lưu kho", "Tổng tiền", "Mô tả" };
                    foreach (var header in headers)
                    {
                        TableCell cell = new TableCell(new Paragraph(new Run(new Text(header))));
                        cell.TableCellProperties = new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2400" });
                        headerRow.Append(cell);
                    }
                    table.Append(headerRow);

                    // Data rows
                    foreach (var detail in chiTietDonHangs)
                    {
                        TableRow row = new TableRow();

                        decimal donGia = Convert.ToDecimal(detail.donGia);
                        decimal tienLuuKho = detail.tienLuuKho.HasValue ? Convert.ToDecimal(detail.tienLuuKho.Value) : 0m;
                        decimal soLuong = Convert.ToDecimal(detail.soLuong);
                        decimal tongTien = (soLuong * donGia) + tienLuuKho;

                        string[] cells = {
                    detail.maChiTietDonHang,
                    detail.maHangHoa,
                    detail.soLuong.ToString(),
                    detail.donViTinh,
                    detail.chatLuong,
                    donGia.ToString("C0"),
                    tienLuuKho.ToString("C0"),
                    tongTien.ToString("C0"),
                    detail.moTa ?? ""
                };

                        foreach (var cellText in cells)
                        {
                            TableCell cell = new TableCell(new Paragraph(new Run(new Text(cellText))));
                            cell.TableCellProperties = new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Dxa, Width = "2400" });
                            row.Append(cell);
                        }
                        table.Append(row);
                    }

                    body.Append(table);
                    mainPart.Document.Append(body);
                    mainPart.Document.Save();
                }

                byte[] fileContents = ms.ToArray();
                string fileName = $"PhieuXuatKho_{id}_{DateTime.Now:yyyyMMddHHmmss}.docx";

                return File(fileContents, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
            }
        }
    }
}
