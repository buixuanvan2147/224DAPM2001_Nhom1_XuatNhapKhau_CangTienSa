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
    public class QuanTriVien_QLDH_chiTietKhoController : Controller
    {
        private XuatNhapHangTaiCangTienSaEntities db = new XuatNhapHangTaiCangTienSaEntities();

        // GET: QuanTriVien_QLDH_chiTietKho
        public ActionResult Index()
        {
            var chiTietKhoes = db.chiTietKhoes.Include(c => c.kho);
            return View(chiTietKhoes.ToList());
        }

        // GET: QuanTriVien_QLDH_chiTietKho/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            chiTietKho chiTietKho = db.chiTietKhoes.Find(id);
            if (chiTietKho == null)
            {
                return HttpNotFound();
            }
            return View(chiTietKho);
        }

        // GET: QuanTriVien_QLDH_chiTietKho/Create
        public ActionResult Create()
        {
            ViewBag.maKho = new SelectList(db.khoes, "maKho", "maLoaiKho");
            return View();
        }

        // POST: QuanTriVien_QLDH_chiTietKho/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "maChiTietKho,maKho,sucChuaToiDa,tonKho,trangThaiChiTietKho,ngayCapNhatCuoi,moTa")] chiTietKho chiTietKho)
        {
            if (ModelState.IsValid)
            {
                db.chiTietKhoes.Add(chiTietKho);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.maKho = new SelectList(db.khoes, "maKho", "maLoaiKho", chiTietKho.maKho);
            return View(chiTietKho);
        }

        // GET: QuanTriVien_QLDH_chiTietKho/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            chiTietKho chiTietKho = db.chiTietKhoes.Find(id);
            if (chiTietKho == null)
            {
                return HttpNotFound();
            }
            ViewBag.maKho = new SelectList(db.khoes, "maKho", "maLoaiKho", chiTietKho.maKho);
            return View(chiTietKho);
        }

        // POST: QuanTriVien_QLDH_chiTietKho/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "maChiTietKho,maKho,sucChuaToiDa,tonKho,trangThaiChiTietKho,ngayCapNhatCuoi,moTa")] chiTietKho chiTietKho)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chiTietKho).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.maKho = new SelectList(db.khoes, "maKho", "maLoaiKho", chiTietKho.maKho);
            return View(chiTietKho);
        }

        // GET: QuanTriVien_QLDH_chiTietKho/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            chiTietKho chiTietKho = db.chiTietKhoes.Find(id);
            if (chiTietKho == null)
            {
                return HttpNotFound();
            }
            return View(chiTietKho);
        }

        // POST: QuanTriVien_QLDH_chiTietKho/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            chiTietKho chiTietKho = db.chiTietKhoes.Find(id);
            db.chiTietKhoes.Remove(chiTietKho);
            db.SaveChanges();
            return RedirectToAction("Index");
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
