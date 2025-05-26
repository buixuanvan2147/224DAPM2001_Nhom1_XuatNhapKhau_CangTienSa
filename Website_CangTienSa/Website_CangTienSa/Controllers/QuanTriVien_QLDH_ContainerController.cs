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

        // GET: QuanTriVien_QLDH_Container
        public ActionResult QTV_QLDH_IndexDanhmucContainer()
        {
            return View(db.danhMucContainers.ToList());
        }

        // GET: QuanTriVien_QLDH_Container/Details/5
        public ActionResult QTV_QLDH_ChiTietDanhmucContainer(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            danhMucContainer danhMucContainer = db.danhMucContainers.Find(id);
            if (danhMucContainer == null)
            {
                return HttpNotFound();
            }
            return View(danhMucContainer);
        }

        // GET: QuanTriVien_QLDH_Container/QTV_QLDH_ThemDanhmucContainer
        public ActionResult QTV_QLDH_ThemDanhmucContainer()
        {
            return View();
        }

        // POST: QuanTriVien_QLDH_Container/QTV_QLDH_ThemDanhmucContainer
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QTV_QLDH_ThemDanhmucContainer([Bind(Include = "maDanhMucContainer,tenDanhMucContainer,sucChuaToiDa,trongTaiToiDa,chieuDai,chieuRong,chieuCao,moTa")] danhMucContainer danhMucContainer)
        {
            if (ModelState.IsValid)
            {
                db.danhMucContainers.Add(danhMucContainer);
                db.SaveChanges();
            }

            return View(danhMucContainer);
        }

        // GET: QuanTriVien_QLDH_Container/Edit/5
        public ActionResult QTV_QLDH_SuaDanhmucContainer(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            danhMucContainer danhMucContainer = db.danhMucContainers.Find(id);
            if (danhMucContainer == null)
            {
                return HttpNotFound();
            }
            return View(danhMucContainer);
        }

        // POST: QuanTriVien_QLDH_Container/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QTV_QLDH_SuaDanhmucContainer([Bind(Include = "maDanhMucContainer,tenDanhMucContainer,sucChuaToiDa,trongTaiToiDa,chieuDai,chieuRong,chieuCao,moTa")] danhMucContainer danhMucContainer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(danhMucContainer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("QTV_QLDH_IndexDanhmucContainer");
            }
            return View(danhMucContainer);
        }

        // GET: QuanTriVien_QLDH_Container/Delete/5
        public ActionResult QTV_QLDH_XoaDanhmucContainer(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            danhMucContainer danhMucContainer = db.danhMucContainers.Find(id);
            if (danhMucContainer == null)
            {
                return HttpNotFound();
            }
            return View(danhMucContainer);
        }

        // POST: QuanTriVien_QLDH_Container/Delete/5
        [HttpPost, ActionName("QTV_QLDH_XoaDanhmucContainer")]
        [ValidateAntiForgeryToken]
        public ActionResult QTV_QLDH_XoaDanhmucContainerConfirmed(string id)
        {
            danhMucContainer danhMucContainer = db.danhMucContainers.Find(id);
            db.danhMucContainers.Remove(danhMucContainer);
            db.SaveChanges();
            return RedirectToAction("QTV_QLDH_IndexDanhmucContainer");
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
