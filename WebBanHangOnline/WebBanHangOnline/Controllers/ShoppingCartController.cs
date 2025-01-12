using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanHangOnline.Models;
using WebBanHangOnline.Models.EF;

namespace WebBanHangOnline.Controllers
{
    [Authorize]
    public class ShoppingCartController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public ShoppingCartController()
        {
        }

        public ShoppingCartController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: ShoppingCart
        [AllowAnonymous]
        public ActionResult Index()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;
            }
            return View();
        }
        [AllowAnonymous]
        public ActionResult CheckOut()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                ViewBag.CheckCart = cart;
            }
            return View();
        }
        [AllowAnonymous]
        public ActionResult CheckOutSuccess()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Partial_Item_ThanhToan()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }
        [AllowAnonymous]
        public ActionResult Partial_Item_Cart()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null && cart.Items.Any())
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }

        [AllowAnonymous]
        public ActionResult ShowCount()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                return Json(new { Count = cart.Items.Count }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult Partial_CheckOut()
        {
            var user = UserManager.FindByNameAsync(User.Identity.Name).Result;
            if (user != null)
            {
                ViewBag.User = user;
            }
            return PartialView();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOut(OrderViewModel req)
        {
            var code = new { Success = false, Code = -1, msg = "" };
            if (ModelState.IsValid)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart != null)
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            // Tạo đơn hàng mới
                            Order order = new Order
                            {
                                CustomerName = req.CustomerName,
                                Phone = req.Phone,
                                Address = req.Address,
                                Email = req.Email,
                                TypePayment = req.TypePayment,
                                CreatedDate = DateTime.Now,
                                ModifiedDate = DateTime.Now,
                                CreatedBy = req.Phone,
                                Code = "DH" + new Random().Next(1000, 9999)
                            };

                            if (User.Identity.IsAuthenticated)
                            {
                                order.CustomerId = User.Identity.GetUserId();
                            }

                            // Duyệt qua từng sản phẩm trong giỏ hàng
                            foreach (var cartItem in cart.Items)
                            {
                                var product = db.Products.SingleOrDefault(p => p.Id == cartItem.ProductId);
                                if (product == null || product.Quantity < cartItem.Quantity)
                                {
                                    transaction.Rollback();
                                    code = new { Success = false, Code = -1, msg = "Số lượng sản phẩm không đủ trong kho." };
                                    return Json(code);
                                }

                                // Tạo chi tiết đơn hàng
                                order.OrderDetails.Add(new OrderDetail
                                {
                                    ProductId = cartItem.ProductId,
                                    Quantity = cartItem.Quantity,
                                    Price = cartItem.Price
                                });

                                // Trừ số lượng sản phẩm trong kho
                                product.Quantity -= cartItem.Quantity;
                            }

                            // Tổng số tiền của đơn hàng
                            order.TotalAmount = cart.Items.Sum(x => (x.Price * x.Quantity));

                            db.Orders.Add(order);
                            db.SaveChanges();

                            // Lưu thay đổi số lượng sản phẩm
                            //db.SaveChanges();

                            // Commit transaction
                            transaction.Commit();

                            // Xóa giỏ hàng sau khi đặt hàng thành công
                            cart.ClearCart();
                            Session["Cart"] = null;

                            code = new { Success = true, Code = 1, msg = "Đặt hàng thành công!" };
                            return Json(code);
                        }
                        catch (Exception ex)
                        {
                            // Rollback transaction nếu có lỗi
                            transaction.Rollback();
                            code = new { Success = false, Code = -1, msg = "Có lỗi xảy ra, vui lòng thử lại." };
                            return Json(code);
                        }
                    }
                }
            }

            return Json(code);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult AddToCart(int id, int quantity)
        {
            var code = new { Success = false, msg = "", code = -1, Count = 0 };
            var db = new ApplicationDbContext();
            var checkProduct = db.Products.FirstOrDefault(x => x.Id == id);

            if (checkProduct != null)
            {
                // Kiểm tra số lượng sản phẩm hiện có trong kho
                if (quantity > checkProduct.Quantity)  // Giả sử cột lưu số lượng sản phẩm trong kho là StockQuantity
                {
                    code = new { Success = false, msg = "Số lượng sản phẩm vượt quá số lượng có sẵn trong kho!", code = -1, Count = 0 };
                    return Json(code);
                }

                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart == null)
                {
                    cart = new ShoppingCart();
                }

                // Kiểm tra nếu sản phẩm đã có trong giỏ hàng thì cộng thêm số lượng
                var existingItem = cart.Items.FirstOrDefault(x => x.ProductId == id);
                if (existingItem != null)
                {
                    // Nếu tổng số lượng vượt quá số lượng có sẵn trong kho, trả về lỗi
                    if (existingItem.Quantity + quantity > checkProduct.Quantity)
                    {
                        code = new { Success = false, msg = "Số lượng tổng cộng vượt quá số lượng có sẵn trong kho!", code = -1, Count = cart.Items.Count };
                        return Json(code);
                    }

                    existingItem.Quantity += quantity;
                    existingItem.TotalPrice = existingItem.Quantity * existingItem.Price;
                }
                else
                {
                    ShoppingCartItem item = new ShoppingCartItem
                    {
                        ProductId = checkProduct.Id,
                        ProductName = checkProduct.Title,
                        CategoryName = checkProduct.ProductCategory.Title,
                        Alias = checkProduct.Alias,
                        Quantity = quantity
                    };

                    if (checkProduct.ProductImage.FirstOrDefault(x => x.IsDefault) != null)
                    {
                        item.ProductImg = checkProduct.ProductImage.FirstOrDefault(x => x.IsDefault).Image;
                    }
                    item.Price = checkProduct.Price;
                    if (checkProduct.PriceSale > 0)
                    {
                        item.Price = (decimal)checkProduct.PriceSale;
                    }
                    item.TotalPrice = item.Quantity * item.Price;
                    cart.AddToCart(item, quantity);
                }

                Session["Cart"] = cart;
                code = new { Success = true, msg = "Thêm sản phẩm vào giỏ hàng thành công!", code = 1, Count = cart.Items.Count };
            }
            return Json(code);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Update(int id, int quantity)
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                cart.UpdateQuantity(id, quantity);
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var code = new { Success = false, msg = "", code = -1, Count = 0 };

            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                var checkProduct = cart.Items.FirstOrDefault(x => x.ProductId == id);
                if (checkProduct != null)
                {
                    cart.Remove(id);
                    code = new { Success = true, msg = "", code = 1, Count = cart.Items.Count };
                }
            }
            return Json(code);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult DeleteAll()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                cart.ClearCart();
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }

        public ActionResult OrderHistory()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userStore = new UserStore<ApplicationUser>(new ApplicationDbContext());
                var userManager = new UserManager<ApplicationUser>(userStore);
                var user = userManager.FindByName(User.Identity.Name);
                var items = db.Orders.Where(x => x.CustomerId == user.Id).OrderByDescending(x => x.CreatedDate).ToList();
                return PartialView(items);
            }
            return PartialView();
        }
    }
}