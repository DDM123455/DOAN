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
    public class DonMuasController : Controller
    {
        private CNPMNCEntities db = new CNPMNCEntities();

        // GET: DonMuas
        public ActionResult Index(string searchBy, int search=0)
        {

            if (searchBy == "MaDM" & search != 0)
            {
                return View(db.DonMuas.Where(s => s.MaDM == (int)search).ToList());
            }
            
            else
                return View(db.DonMuas.ToList());
        }

        // GET: DonMuas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonMua donMua = db.DonMuas.Find(id);
            if (donMua == null)
            {
                return HttpNotFound();
            }
            List<DonMua> donMuas = db.DonMuas.ToList();
            List<ChiTietDonMua> chiTietDonMuas = db.ChiTietDonMuas.ToList();
         
            return View(db.ChiTietDonMuas.Where(s => s.MaDM == id).ToList());
        }

        // GET: DonMuas/Create
        public ActionResult Create()
        {
            ViewBag.MaDM = new SelectList(db.ChiTietDonMuas, "MaDM", "MaDM");
            return View();
        }

        // POST: DonMuas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaDM,Ngay,DiemTichLuy,VoucherDaDung,GhiChu")] DonMua donMua)
        {
            if (ModelState.IsValid)
            {
                db.DonMuas.Add(donMua);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaDM = new SelectList(db.ChiTietDonMuas, "MaDM", "MaDM", donMua.MaDM);
            return View(donMua);
        }

        // GET: DonMuas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonMua donMua = db.DonMuas.Find(id);
            if (donMua == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaDM = new SelectList(db.ChiTietDonMuas, "MaDM", "MaDM", donMua.MaDM);
            return View(donMua);
        }

        // POST: DonMuas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaDM,Ngay,DiemTichLuy,VoucherDaDung,GhiChu")] DonMua donMua)
        {
            if (ModelState.IsValid)
            {
                db.Entry(donMua).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaDM = new SelectList(db.ChiTietDonMuas, "MaDM", "MaDM", donMua.MaDM);
            return View(donMua);
        }

        // GET: DonMuas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DonMua donMua = db.DonMuas.Find(id);
            if (donMua == null)
            {
                return HttpNotFound();
            }
            return View(donMua);
        }

        // POST: DonMuas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DonMua donMua = db.DonMuas.Find(id);
            db.DonMuas.Remove(donMua);
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
            /*           MaDM,SoLuong,Thue,TongCong,MaKH"
            */
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[5] {
                   new DataColumn("MaDM"),
                new DataColumn("SoLuong"),
                new DataColumn("Thue"),
                new DataColumn("TongCong"),
                new DataColumn("MaKH"),
               });
            var emps = from ChiTietDonMua in db.ChiTietDonMuas.ToList() select ChiTietDonMua;
            foreach (var khach in emps)
            {
                dt.Rows.Add(khach.MaDM, khach.SoLuong, khach.Thue,
                    khach.TongCong, khach.MaKH);

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
