using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class ImportController : Controller
    {
        // GET: Admin/Import
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(string Searchtext, int? page)
        {
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<Import> items = db.Imports.OrderByDescending(x => x.CreatedBy);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                items = items.Where(x => x.CreatedBy.Contains(Searchtext));
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
        }
        public ActionResult Add()
        {
            ViewBag.Supplier = new SelectList(db.Suppliers.ToList(), "Id", "SupplierName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Import model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
                db.Imports.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Supplier = new SelectList(db.Suppliers.ToList(), "Id", "SupplierName");
            return View(model);
        }
        public ActionResult Edit(int id)
        {
            ViewBag.Supplier = new SelectList(db.Suppliers.ToList(), "Id", "SupplierName");
            var item = db.Imports.Find(id);
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Import model)
        {
            if (ModelState.IsValid)
            {
                model.ModifiedDate = DateTime.Now;
                db.Imports.Attach(model);
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        public ActionResult ViewImport(int id)
        {
            var item = db.Imports.Find(id);
            return View(item);
        }
        public ActionResult Partial_DetailImport(int id)
        {
            var items = db.ImportDetails.Where(x => x.ImportId == id).ToList();
            return PartialView(items);
        }
        public ActionResult AddImportDetail()
        {
            ViewBag.Product = new SelectList(db.Products.ToList(), "Id", "Title");
            ViewBag.Import = new SelectList(db.Imports.ToList(), "Id", "CreatedBy");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddImportDetail(ImportDetail model)
        {
            if (ModelState.IsValid)
            {
                var product = db.Products.SingleOrDefault(p => p.Id == model.ProductId);
                if (product != null)
                {
                    // Cộng số lượng từ chi tiết phiếu nhập vào sản phẩm
                    product.Quantity += model.Quantity;

                    // Thêm chi tiết phiếu nhập vào cơ sở dữ liệu
                    db.ImportDetails.Add(model);

                    var import = db.Imports.SingleOrDefault(i => i.Id == model.ImportId);
                    if (import != null)
                    {
                        import.TotalAmount += model.Price * model.Quantity;
                        db.Entry(import).State = EntityState.Modified;
                    }
                    // Lưu thay đổi vào cơ sở dữ liệu
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                //db.ImportDetails.Add(model);
                //db.SaveChanges();
                //return RedirectToAction("Index");
            }
            ViewBag.Product = new SelectList(db.Products.ToList(), "Id", "Title");
            ViewBag.Import = new SelectList(db.Imports.ToList(), "Id", "CreatedBy");
            return View(model);
        }
    }
}