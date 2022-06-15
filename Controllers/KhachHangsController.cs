using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using DOAN.Models;

namespace DOAN.Controllers
{
    public class KhachHangsController : Controller
    {
        private CNPMNCEntities db = new CNPMNCEntities();

        // GET: KhachHangs
        public ActionResult Index(string searchBy, string search)
        {
            if (searchBy == "HoTenKH")
            {
                return View(db.KhachHangs.Where(s => s.HoTenKH.Contains(search)).ToList());
            }


            else if(searchBy=="GiaChi")
                return View(db.KhachHangs.Where(s => s.GiaChi.Contains(search)).ToList());
            else
             return View(db.KhachHangs.ToList());
        }

        // GET: KhachHangs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang khachHang = db.KhachHangs.Find(id);
            if (khachHang == null)
            {
                return HttpNotFound();
            }
            return View(db.ChiTietDonMuas.Where(s => s.MaKH == id).ToList());
        }

        // GET: KhachHangs/Create
        public ActionResult Create()
        {
            ViewBag.MaLoai = new SelectList(db.LoaiKHs, "MaLoai", "TenLoai");
            return View();
        }

        // POST: KhachHangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaKH,HoTenKH,SDT,GioiTinh,GiaChi,GhiChu,MaLoai,DiemKH")] KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                db.KhachHangs.Add(khachHang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaLoai = new SelectList(db.LoaiKHs, "MaLoai", "TenLoai", khachHang.MaLoai);
            return View(khachHang);
        }

        // GET: KhachHangs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang khachHang = db.KhachHangs.Find(id);
            if (khachHang == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaLoai = new SelectList(db.LoaiKHs, "MaLoai", "TenLoai", khachHang.MaLoai);
            return View(khachHang);
        }

        // POST: KhachHangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaKH,HoTenKH,SDT,GioiTinh,GiaChi,GhiChu,MaLoai,DiemKH")] KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(khachHang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaLoai = new SelectList(db.LoaiKHs, "MaLoai", "TenLoai", khachHang.MaLoai);
            return View(khachHang);
        }

        // GET: KhachHangs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang khachHang = db.KhachHangs.Find(id);
            if (khachHang == null)
            {
                return HttpNotFound();
            }
            return View(khachHang);
        }

        // POST: KhachHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KhachHang khachHang = db.KhachHangs.Find(id);
            db.KhachHangs.Remove(khachHang);
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
        [HttpPost]
        public FileResult Export()
        {
/*            MaKH,HoTenKH,SDT,GioiTinh,GiaChi,GhiChu,MaLoai,DiemKH"
*/            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[8] {
                   new DataColumn("MAKH"),
                new DataColumn("HoTenKH"),
                new DataColumn("SDT"),
                new DataColumn("GioiTinh"),
                new DataColumn("GiaChi"),
                new DataColumn("GhiChu"),
                new DataColumn("MaLoai"),
                new DataColumn("DiemKH"),});
            var emps = from KhachHang in db.KhachHangs.ToList() select KhachHang;
            foreach (var khach in emps)
            {
                dt.Rows.Add(khach.MaKH, khach.HoTenKH, khach.SDT,
                    khach.GioiTinh, khach.GiaChi, khach.GhiChu,
                    khach.MaLoai, khach.DiemKH);
            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                }
            }
        }

    }
}
