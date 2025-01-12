using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;

namespace WebBanHangOnline.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,Employee")]
    public class SubscribeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Subscribe
        public ActionResult Index()
        {
            var items = db.Subscribes.OrderByDescending(x => x.CreatedDate).ToList();
            return View(items);
        }
    }
}