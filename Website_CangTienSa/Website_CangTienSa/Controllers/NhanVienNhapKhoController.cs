using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Website_CangTienSa.DAO;
using Website_CangTienSa.Models;

namespace Website_CangTienSa.Controllers
{
    public class NhanVienNhapKhoController : Controller
    {
        private readonly PhieuNhapDAO _phieuNhapDAO;

        public NhanVienNhapKhoController()
        {
            _phieuNhapDAO = new PhieuNhapDAO();
        }

        public ActionResult Index_NhanVienNhapKho()
        {
            try
            {
                var phieuNhapList = _phieuNhapDAO.GetAllPhieuNhap();
                var khoList = _phieuNhapDAO.GetAllKho();
                ViewBag.KhoList = khoList;

                // Lấy danh sách kho dưới dạng List<KhoModel>
                var khoDetailList = _phieuNhapDAO.GetAllKhoList();
                ViewBag.KhoDetailList = khoDetailList; // Truyền vào ViewBag để sử dụng trong view nếu cần

                return View(phieuNhapList);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi lấy dữ liệu: " + ex.Message;
                return View(new List<PhieuNhapModel>());
            }
        }
    }
}