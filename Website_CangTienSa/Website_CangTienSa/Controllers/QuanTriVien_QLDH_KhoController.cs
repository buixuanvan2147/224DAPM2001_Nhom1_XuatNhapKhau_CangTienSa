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
    public class QuanTriVien_QLDH_KhoController : Controller
    {
        private XuatNhapHangTaiCangTienSaEntities db = new XuatNhapHangTaiCangTienSaEntities();

        // GET: QuanTriVien_QLDH_Kho
        public ActionResult Index()
        {
            var khoes = db.khoes.Include(k => k.loaiKho);
            return View(khoes.ToList());
        }

        // GET: QuanTriVien_QLDH_Kho/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Dòng này sẽ lấy thông tin của 'kho' dựa trên 'id'
            // VÀ tải (Include) tất cả các 'chiTietKho' có 'maKho' tương ứng với 'id' đó.
            kho kho = db.khoes.Include(k => k.chiTietKhoes).FirstOrDefault(k => k.maKho == id);

            if (kho == null)
            {
                return HttpNotFound();
            }

            // Đối tượng 'kho' này bây giờ đã chứa danh sách 'chiTietKhoes' bên trong nó
            return View(kho);
        }

        // GET: QuanTriVien_QLDH_Kho/Create
        public ActionResult Create()
        {
            ViewBag.maLoaiKho = new SelectList(db.loaiKhoes, "maLoaiKho", "tenLoaiKho");
            return View();
        }

        // POST: QuanTriVien_QLDH_Kho/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "maKho,maLoaiKho,tenKho,diaChiKho,trangThaiKho,sucChuaToiDa,tonKho,moTa")] kho kho)
        {
            if (ModelState.IsValid)
            {
                db.khoes.Add(kho);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.maLoaiKho = new SelectList(db.loaiKhoes, "maLoaiKho", "tenLoaiKho", kho.maLoaiKho);
            return View(kho);
        }

        // GET: QuanTriVien_QLDH_Kho/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            kho kho = db.khoes.Find(id);
            if (kho == null)
            {
                return HttpNotFound();
            }
            ViewBag.maLoaiKho = new SelectList(db.loaiKhoes, "maLoaiKho", "tenLoaiKho", kho.maLoaiKho);
            return View(kho);
        }

        // POST: QuanTriVien_QLDH_Kho/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "maKho,maLoaiKho,tenKho,diaChiKho,trangThaiKho,sucChuaToiDa,tonKho,moTa")] kho kho)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kho).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.maLoaiKho = new SelectList(db.loaiKhoes, "maLoaiKho", "tenLoaiKho", kho.maLoaiKho);
            return View(kho);
        }

        // GET: QuanTriVien_QLDH_Kho/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            kho kho = db.khoes.Find(id);
            if (kho == null)
            {
                return HttpNotFound();
            }
            return View(kho);
        }

        // POST: QuanTriVien_QLDH_Kho/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            kho kho = db.khoes.Find(id);
            db.khoes.Remove(kho);
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
