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
    public class DonMuasController : Controller
    {
        private CNPMNCEntities db = new CNPMNCEntities();

        // GET: DonMuas
        public ActionResult Index(string sortProperty, string sortOrder, string searchBy, int search = 0)
        {



            if (searchBy == "MaDM" & search != 0)
            {
                return View(db.DonMuas.Where(m => m.MaDM == (int)search).ToList());
            }

            ViewBag.SortOrder = String.IsNullOrEmpty(sortOrder) ? "desc" : "";

            // 2. Lấy tất cả tên thuộc tính của lớp Link (LinkID, LinkName, LinkURL,...)
            var properties = typeof(DonMua).GetProperties();
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
            var links = from l in db.DonMuas
                        select l;

            // 4. Tạo thuộc tính sắp xếp mặc định là "LinkID"
            if (String.IsNullOrEmpty(sortProperty)) sortProperty = "MaDM";

            // 5. Sắp xếp tăng/giảm bằng phương thức OrderBy sử dụng trong thư viện Dynamic LINQ
            if (sortOrder == "desc") links = links.OrderBy(sortProperty + " desc");
            else links = links.OrderBy(sortProperty);

            // 6. Trả kết quả về Views
            return View(links.ToList());


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
