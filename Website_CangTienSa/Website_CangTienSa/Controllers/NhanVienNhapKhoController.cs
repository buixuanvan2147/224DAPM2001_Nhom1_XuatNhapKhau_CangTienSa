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

                var khoDetailList = _phieuNhapDAO.GetAllKhoList();
                ViewBag.KhoDetailList = khoDetailList;

                return View(phieuNhapList);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi khi lấy dữ liệu: " + ex.Message;
                return View(new List<PhieuNhapModel>());
            }
        }

        public ActionResult GetPhieuNhapModal(string maPhieuNhap)
        {
            try
            {
                var phieuNhapList = _phieuNhapDAO.GetAllPhieuNhap();
                var phieuNhap = phieuNhapList.FirstOrDefault(p => p.MaPhieuNhap == maPhieuNhap);
                if (phieuNhap == null)
                {
                    return HttpNotFound();
                }
                return PartialView("_PhieuNhapKhoModal", phieuNhap);
            }
            catch (Exception ex)
            {
                return Content("Lỗi: " + ex.Message);
            }
        }
    }
}