using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DOAN.Models;

namespace DOAN.Controllers
{
    public class LoaiKHsController : Controller
    {
        private CNPMNCEntities db = new CNPMNCEntities();

        // GET: LoaiKHs
        public ActionResult Index(string searchBy, string search)
        {
            if (searchBy == "TenLoai")
            {
                return View(db.LoaiKHs.Where(s => s.TenLoai.Contains(search)).ToList());
            }
           

            else
                return View(db.LoaiKHs.ToList());
        }

        // GET: LoaiKHs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoaiKH loaiKH = db.LoaiKHs.Find(id);
            List<KhachHang> khachHangs = db.KhachHangs.ToList();
            List<LoaiKH> loaiKHs = db.LoaiKHs.ToList();

            if (loaiKH == null)
            {
                return HttpNotFound();
            }
            var list = from i in khachHangs
                       join c in loaiKHs on i.MaLoai equals c.MaLoai
                       where c.MaLoai == id.Value
                       select new DanhSachLoaiKH
                       {
                           TenLoai = c.TenLoai,
                           MaKH = i.MaKH,
                           HoTenKH = i.HoTenKH,
                           SDT= (int)i.SDT,
                           GiaChi=i.GioiTinh,
                           GioiTinh=i.GioiTinh,
                           GhiChu=i.GhiChu,

                       };
            return View(list);
        }

        // GET: LoaiKHs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LoaiKHs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaLoai,TenLoai,Giam")] LoaiKH loaiKH)
        {
            if (ModelState.IsValid)
            {
                db.LoaiKHs.Add(loaiKH);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(loaiKH);
        }

        // GET: LoaiKHs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoaiKH loaiKH = db.LoaiKHs.Find(id);
            if (loaiKH == null)
            {
                return HttpNotFound();
            }
            return View(loaiKH);
        }

        // POST: LoaiKHs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaLoai,TenLoai,Giam")] LoaiKH loaiKH)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loaiKH).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(loaiKH);
        }

        // GET: LoaiKHs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoaiKH loaiKH = db.LoaiKHs.Find(id);
            if (loaiKH == null)
            {
                return HttpNotFound();
            }
            return View(loaiKH);
        }

        // POST: LoaiKHs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LoaiKH loaiKH = db.LoaiKHs.Find(id);
            db.LoaiKHs.Remove(loaiKH);
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
