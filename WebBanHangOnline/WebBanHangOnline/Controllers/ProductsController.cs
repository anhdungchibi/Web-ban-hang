using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Products
        public ActionResult Index(int? page, string timKiem)
        {
            //var items = db.Products.ToList();

            ////var lstsanpham = db.Products.AsNoTracking().OrderBy(x => x.Title).ToList();
            //return View(items);
            int pageSize = 8;
            int pageNumber = page == null || page < 0 ? 1 : page.Value;

            if (timKiem == null)
            {

                var sanphamTimKiem = db.Products.AsNoTracking().OrderBy(x => x.Title);
                PagedList<Product> lst = new PagedList<Product>(sanphamTimKiem, pageNumber, pageSize);


                return View(lst);
            }
            else
            {
                var sanphamTimKiem = db.Products.AsNoTracking().Where(x => x.Title.Trim().ToUpper().Contains(timKiem.Trim().ToUpper())).OrderBy(x => x.Title);
                PagedList<Product> lst = new PagedList<Product>(sanphamTimKiem, pageNumber, pageSize);
                return View(lst);
            }

            //if (timKiem == null)
            //{

            //    var items = db.Products.ToList();
            //    return View(items);
            //}
            //else
            //{
            //    var sanphamTimKiem = db.Products.AsNoTracking().Where(x => x.Title.Trim().ToUpper().Contains(timKiem.Trim().ToUpper())).OrderBy(x => x.Title);
            //    return View(sanphamTimKiem);
            //}

        }

        public ActionResult Detail(string alias, int id)
        {
            var item = db.Products.Find(id);
            if (item != null)
            {
                db.Products.Attach(item);
                item.ViewCount = item.ViewCount + 1;
                db.Entry(item).Property(x => x.ViewCount).IsModified = true;
                db.SaveChanges();
            }
            var countReview = db.ReviewProducts.Where(x => x.ProductId == id).Count();
            ViewBag.CountReview = countReview;
            return View(item);
        }
        public ActionResult ProductCategory(string alias, int id)
        {
            var items = db.Products.ToList();
            if (id > 0)
            {
                items = items.Where(x => x.ProductCategoryId == id).ToList();
            }
            var cate = db.ProductCategories.Find(id);
            if (cate != null)
            {
                ViewBag.CateName = cate.Title;
            }

            ViewBag.CateId = id;
            return View(items);
        }

        public ActionResult Partial_ItemsByCateId(string timKiem)
        {
            //var lstsanpham = db.Products.AsNoTracking().OrderBy(x => x.Title).ToList();

            if (timKiem == null)
            {

                var items = db.Products.Where(x => x.IsHome && x.IsActive).AsNoTracking().OrderBy(x => x.Title).Take(30).ToList();
                return PartialView(items);
            }
            else
            {
                var sanphamTimKiem = db.Products.Where(x => x.IsHome && x.IsActive).AsNoTracking().OrderBy(x => x.Title).Take(30).AsNoTracking().Where(x => x.Title.Trim().ToUpper().Contains(timKiem.Trim().ToUpper())).OrderBy(x => x.Title);
                return PartialView(sanphamTimKiem);
            }

            //var items = db.Products.Where(x => x.IsHome && x.IsActive).AsNoTracking().OrderBy(x => x.Title).Take(16).ToList();
            //return PartialView(items);
        }

        public ActionResult Partial_ProductSales()
        {
            var items = db.Products.Where(x => x.IsSale && x.IsActive).AsNoTracking().OrderBy(x => x.Title).Take(16).ToList();
            return PartialView(items);
        }


    }
}