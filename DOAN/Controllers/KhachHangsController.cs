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
    public class KhachHangsController : Controller
    {
        private CNPMNCEntities db = new CNPMNCEntities();

        // GET: KhachHangs
        public ActionResult Index(string sortProperty, string sortOrder, string searchBy, string search)
        {
            if (searchBy == "HoTenKH")
            {
                return View(db.KhachHangs.Where(m => m.HoTenKH.Contains(search)).ToList());
            }


            else if (searchBy == "GiaChi")
                return View(db.KhachHangs.Where(m => m.GiaChi.Contains(search)).ToList());


            ViewBag.SortOrder = String.IsNullOrEmpty(sortOrder) ? "desc" : "";

            // 2. Lấy tất cả tên thuộc tính của lớp Link (LinkID, LinkName, LinkURL,...)
            var properties = typeof(KhachHang).GetProperties();
            string s = String.Empty;
            foreach (var item in properties)
            {
                // 2.1 Kiểm tra xem thuộc tính nào là virtual (public virtual Category Category...)
                var isVirtual = item.GetAccessors()[0].IsVirtual;

                // 2.2. Thuộc tính bình thường thì cho phép sắp xếp
                if (!isVirtual)
                {
                    ViewBag.Headings += "<th><a href='?sortProperty=" + item.Name + "&sortOrder=" +
                        ViewBag.SortOrder + "'>" + item.Name + "</a></th>";
                }
                // 2.3. Thuộc tính virtual (public virtual Category Category...) thì không sắp xếp được
                // cho nên không cần tạo liên kết
                else ViewBag.Headings += "<th>" + item.Name + "</th>";
            }

            // 3. Truy vấn lấy tất cả đường dẫn
            var links = from l in db.KhachHangs
                        select l;


            // 4. Tạo thuộc tính sắp xếp mặc định là "LinkID"
            if (String.IsNullOrEmpty(sortProperty)) sortProperty = "MaKH";

            // 5. Sắp xếp tăng/giảm bằng phương thức OrderBy sử dụng trong thư viện Dynamic LINQ
            if (sortOrder == "desc") links = links.OrderBy(sortProperty + " desc");
            else links = links.OrderBy(sortProperty);

            // 6. Trả kết quả về Views
            return View(links.ToList());

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
            */
            DataTable dt = new DataTable("Grid");
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