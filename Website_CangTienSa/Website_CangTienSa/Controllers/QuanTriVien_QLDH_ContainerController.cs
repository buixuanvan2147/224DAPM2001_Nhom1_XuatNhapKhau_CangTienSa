using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Website_CangTienSa.Models;

namespace Website_CangTienSa.Controllers
{
    public class QuanTriVien_QLDH_ContainerController : Controller
    {
        private XuatNhapHangTaiCangTienSaEntities db = new XuatNhapHangTaiCangTienSaEntities();

        // GET: QuanTriVien_QLDH_Container/Details/5
        public ActionResult QuanTriVien_QLDH_ChiTietContainer(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            container container = db.containers.Find(id);
            if (container == null)
            {
                return HttpNotFound();
            }
            return View(container);
        }

        // GET: QuanTriVien_QLDH_Container/Create
        public ActionResult QuanTriVien_QLDH_ThemContainer()
        {
            ViewBag.maChiTietKho = new SelectList(db.chiTietKhoes, "maChiTietKho", "maKho");
            ViewBag.maDanhMucContainer = new SelectList(db.danhMucContainers, "maDanhMucContainer", "tenDanhMucContainer");
            return View();
        }

        // POST: QuanTriVien_QLDH_Container/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuanTriVien_QLDH_ThemContainer([Bind(Include = "maContainer,maDanhMucContainer,maChiTietKho,soHieu,trangThaiContainer,viTriTrongKho,ngayMuaContainer,trongTai")] container container)
        {
            if (ModelState.IsValid)
            {
                db.containers.Add(container);
                db.SaveChanges();
                return RedirectToAction("QuanLyTaiKhoan_QuanTriVien", "QuanTriVien");
            }

            ViewBag.maChiTietKho = new SelectList(db.chiTietKhoes, "maChiTietKho", "maKho", container.maChiTietKho);
            ViewBag.maDanhMucContainer = new SelectList(db.danhMucContainers, "maDanhMucContainer", "tenDanhMucContainer", container.maDanhMucContainer);
            return View(container);
        }

        // GET: QuanTriVien_QLDH_Container/Edit/5
        public ActionResult QuanTriVien_QLDH_SuaContainer(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            container container = db.containers.Find(id);
            if (container == null)
            {
                return HttpNotFound();
            }
            ViewBag.maChiTietKho = new SelectList(db.chiTietKhoes, "maChiTietKho", "maKho", container.maChiTietKho);
            ViewBag.maDanhMucContainer = new SelectList(db.danhMucContainers, "maDanhMucContainer", "tenDanhMucContainer", container.maDanhMucContainer);
            return View(container);
        }

        // POST: QuanTriVien_QLDH_Container/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuanTriVien_QLDH_SuaContainer([Bind(Include = "maContainer,maDanhMucContainer,maChiTietKho,soHieu,trangThaiContainer,viTriTrongKho,ngayMuaContainer,trongTai")] container container)
        {
            if (ModelState.IsValid)
            {
                db.Entry(container).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("QuanLyTaiKhoan_QuanTriVien", "QuanTriVien");
            }
            ViewBag.maChiTietKho = new SelectList(db.chiTietKhoes, "maChiTietKho", "maKho", container.maChiTietKho);
            ViewBag.maDanhMucContainer = new SelectList(db.danhMucContainers, "maDanhMucContainer", "tenDanhMucContainer", container.maDanhMucContainer);
            return View(container);
        }

        // GET: QuanTriVien_QLDH_Container/Delete/5
        public ActionResult QuanTriVien_QLDH_XoaContainer(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            container container = db.containers.Find(id);
            if (container == null)
            {
                return HttpNotFound();
            }
            return View(container);
        }

        // POST: QuanTriVien_QLDH_Container/Delete/5
        [HttpPost, ActionName("QuanTriVien_QLDH_XoaContainer")]
        [ValidateAntiForgeryToken]
        public ActionResult QuanTriVien_QLDH_XoaContainerConfirmed(string id)
        {
            container container = db.containers.Find(id);
            db.containers.Remove(container);
            db.SaveChanges();
            return RedirectToAction("QuanLyTaiKhoan_QuanTriVien", "QuanTriVien");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
