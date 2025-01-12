using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]

    public class SupplierController : Controller
    {
        // GET: Admin/Supplier
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(string Searchtext, int? page)
        {
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<Supplier> items = db.Suppliers.OrderBy(x=>x.SupplierName);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                items = items.Where(x => x.SupplierName.Contains(Searchtext));
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Supplier model)
        {
            if (ModelState.IsValid)
            {
                db.Suppliers.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var item = db.Suppliers.Find(id);
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Supplier model)
        {
            if (ModelState.IsValid)
            {
                db.Suppliers.Attach(model);
                db.Entry(model).Property(x => x.SupplierName).IsModified = true;
                db.Entry(model).Property(x => x.Phone).IsModified = true;
                db.Entry(model).Property(x => x.Address).IsModified = true;
                db.Entry(model).Property(x => x.Email).IsModified = true;
                db.Entry(model).Property(x => x.Description).IsModified = true;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = db.Suppliers.Find(id);
            if (item != null)
            {
                //var DeleteItem = db.Categories.Attach(item);
                db.Suppliers.Remove(item);
                db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

    }
}