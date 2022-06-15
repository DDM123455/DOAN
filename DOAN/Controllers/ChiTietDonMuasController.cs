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
using System.Linq.Dynamic.Core;


namespace DOAN.Controllers
{
    public class ChiTietDonMuasController : Controller
    {
        private CNPMNCEntities db = new CNPMNCEntities();

        // GET: ChiTietDonMuas
        public ActionResult Index(string sortOrder, string sortProperty, String search)
        {
            ViewBag.MaSortParm = String.IsNullOrEmpty(sortOrder) ? "MaDM" : "";
            ViewBag.TenSortParm = sortOrder == "KhachHang" ? "KhachHangde" : "KhachHang";
            var students = db.ChiTietDonMuas.AsQueryable();
            switch (sortOrder)
            {
                case "MaDM":
                    students = students.OrderByDescending(s => s.MaDM);
                    break;
                case "KhachHang":
                    students = students.OrderBy(s => s.KhachHang);
                    break;
                case "KhachHangde":
                    students = students.OrderByDescending(s => s.KhachHang);
                    break;
                default:
                    students = students.OrderBy(s => s.SoLuong);
                    break;
            }
            return View(students.ToList());

        }

        // GET: ChiTietDonMuas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiTietDonMua chiTietDonMua = db.ChiTietDonMuas.Find(id);
            if (chiTietDonMua == null)
            {
                return HttpNotFound();
            }
            return View(chiTietDonMua);
        }

        // GET: ChiTietDonMuas/Create
        public ActionResult Create()
        {
            ViewBag.MaDM = new SelectList(db.DonMuas, "MaDM", "MaDM");
            ViewBag.MaKH = new SelectList(db.KhachHangs, "MaKH", "MaKH");
            return View();
        }

        // POST: ChiTietDonMuas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaDM,SoLuong,Thue,TongCong,MaKH")] ChiTietDonMua chiTietDonMua)
        {
            if (ModelState.IsValid)
            {
                db.ChiTietDonMuas.Add(chiTietDonMua);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaDM = new SelectList(db.DonMuas, "MaDM", "VoucherDaDung", chiTietDonMua.MaDM);
            ViewBag.MaKH = new SelectList(db.KhachHangs, "MaKH", "HoTenKH", chiTietDonMua.MaKH);
            return View(chiTietDonMua);
        }
        // GET: ChiTietDonMuas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiTietDonMua chiTietDonMua = db.ChiTietDonMuas.Find(id);
            if (chiTietDonMua == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaDM = new SelectList(db.DonMuas, "MaDM", "MaDM", chiTietDonMua.MaDM);
            ViewBag.MaKH = new SelectList(db.KhachHangs, "MaKH", "MaKH", chiTietDonMua.MaKH);
            return View(chiTietDonMua);
        }

        // POST: ChiTietDonMuas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaDM,SoLuong,Thue,TongCong,MaKH")] ChiTietDonMua chiTietDonMua)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chiTietDonMua).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaDM = new SelectList(db.DonMuas, "MaDM", "MaDM", chiTietDonMua.MaDM);
            ViewBag.MaKH = new SelectList(db.KhachHangs, "MaKH", "MaKH", chiTietDonMua.MaKH);
            return View(chiTietDonMua);
        }

        // GET: ChiTietDonMuas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiTietDonMua chiTietDonMua = db.ChiTietDonMuas.Find(id);
            if (chiTietDonMua == null)
            {
                return HttpNotFound();
            }
            return View(chiTietDonMua);
        }

        // POST: ChiTietDonMuas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ChiTietDonMua chiTietDonMua = db.ChiTietDonMuas.Find(id);
            db.ChiTietDonMuas.Remove(chiTietDonMua);
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
            */
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[3] {
                   new DataColumn("MaLoai"),
                new DataColumn("TenLoai"),
                new DataColumn("Giam"),
              });
            var emps = from LoaiKH in db.LoaiKHs.ToList() select LoaiKH;
            foreach (var khach in emps)
            {
                dt.Rows.Add(khach.MaLoai, khach.TenLoai, khach.Giam);

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