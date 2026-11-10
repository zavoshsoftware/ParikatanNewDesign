using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Core.Enums;
using Helpers;
using Models;
using Models.Entities;
using ViewModels.Basket;

//using ViewModels;
//using ViewModels.Basket;

namespace Parikatan.Web.Controllers
{
    public class BasketController : Controller
    {
        private DatabaseContext db = new DatabaseContext();


        [HttpPost]
        public ActionResult AddToCart(string code, string qty)
        {

            int Id = Convert.ToInt32(code);


            Product product = db.Products.FirstOrDefault(c => c.Id == Id &&
                                                             c.IsDeleted == false);

            if (product != null)
            {
                SetCookie(product.Id.ToString(), qty);
                return Json("true", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("invalid", JsonRequestBehavior.AllowGet);
            }

        }

        //basket

        [Route("Basket")]
        public ActionResult Basket()
        {
            CartViewModel cart = new CartViewModel();

            List<ProductInCart> productInCarts = GetProductInBasketByCoockie();

            cart.Products = productInCarts;

            decimal subTotal = GetSubtotal(productInCarts);

            cart.SubTotal = subTotal.ToString("n0") + " تومان";

            decimal discountAmount = GetDiscount();

            cart.DiscountAmount = discountAmount.ToString("n0") + " تومان";

            decimal total = subTotal - discountAmount;
           // decimal total = subTotal;

            decimal shipmentAmount = GetShipmentAmountByTotal(total);
            cart.ShipmentAmount = shipmentAmount.ToString("N0") + " تومان";



            cart.Total = (total + shipmentAmount).ToString("n0") + " تومان";
            cart.RelatedProducts = GetRelatedProduct(productInCarts);

            cart.SubTotalValue = subTotal;
            cart.DiscountValue = discountAmount;
            cart.ShipmentValue = shipmentAmount;
            cart.TotalValue = total + shipmentAmount;
            cart.ItemCount = productInCarts.Sum(c => c.Quantity);

            ViewBag.Title = "سبد خرید | پریکاتن";
            ViewBag.Robots = "noindex, follow";
            return View(cart);
        }

        /// <summary>Current cart totals as numbers (the cart page formats them itself).</summary>
        private object CartTotals()
        {
            List<ProductInCart> items = GetProductInBasketByCoockie();
            decimal subTotal = GetSubtotal(items);
            decimal discount = Math.Min(GetDiscount(), subTotal);
            decimal shipment = items.Count == 0 ? 0 : GetShipmentAmountByTotal(subTotal - discount);

            return new
            {
                count = items.Sum(c => c.Quantity),
                subTotal = subTotal,
                discount = discount,
                shipping = shipment,
                total = subTotal - discount + shipment,
                rows = items.Select(c => new
                {
                    id = c.Product.Id,
                    qty = c.Quantity,
                    amount = (c.Product.IsInPromotion && c.Product.DiscountAmount != null ? c.Product.DiscountAmount.Value : c.Product.Amount) * c.Quantity
                }).ToList()
            };
        }

        [HttpPost]
        public ActionResult SetQuantity(int code, int qty)
        {
            Product product = db.Products.FirstOrDefault(c => c.Id == code && c.IsDeleted == false);
            if (product == null)
                return Json(new { ok = false, message = "محصول یافت نشد." });

            int max = product.Stock > 0 ? Math.Min(10, product.Stock) : 10;
            qty = Math.Max(1, Math.Min(max, qty));

            SetCookie(code.ToString(), qty.ToString());
            RemoveCookie("discount");   // the discount depends on the subtotal

            // the response cookie is not visible to GetCookie() in this request, so compute from the edited list
            var items = GetProductInBasketByCoockie();
            var row = items.FirstOrDefault(c => c.Product.Id == code);
            if (row != null)
                row.Quantity = qty;
            else
                items.Add(new ProductInCart { Product = product, Quantity = qty });

            decimal subTotal = GetSubtotal(items);
            decimal shipment = items.Count == 0 ? 0 : GetShipmentAmountByTotal(subTotal);

            return Json(new
            {
                ok = true,
                count = items.Sum(c => c.Quantity),
                subTotal = subTotal,
                discount = 0,
                shipping = shipment,
                total = subTotal + shipment,
                rows = items.Select(c => new
                {
                    id = c.Product.Id,
                    qty = c.Quantity,
                    amount = (c.Product.IsInPromotion && c.Product.DiscountAmount != null ? c.Product.DiscountAmount.Value : c.Product.Amount) * c.Quantity
                }).ToList()
            });
        }

        [HttpPost]
        public ActionResult RemoveItem(int code)
        {
            var items = GetProductInBasketByCoockie().Where(c => c.Product.Id != code).ToList();

            string cookievalue = string.Concat(items.Select(c => c.Product.Id + "^" + c.Quantity + "/"));
            HttpContext.Response.Cookies.Set(new HttpCookie("basket-parikatan")
            {
                Name = "basket-parikatan",
                Value = cookievalue,
                Expires = DateTime.Now.AddDays(1)
            });
            RemoveCookie("discount");

            decimal subTotal = GetSubtotal(items);
            decimal shipment = items.Count == 0 ? 0 : GetShipmentAmountByTotal(subTotal);

            return Json(new
            {
                ok = true,
                count = items.Sum(c => c.Quantity),
                subTotal = subTotal,
                discount = 0,
                shipping = shipment,
                total = subTotal + shipment,
                rows = items.Select(c => new { id = c.Product.Id, qty = c.Quantity }).ToList()
            });
        }

        [HttpGet]
        public ActionResult CartSummary()
        {
            return Json(CartTotals(), JsonRequestBehavior.AllowGet);
        }

        public List<Product> GetRelatedProduct(List<ProductInCart> currentProducts)
        {
            List<Product> products = db.Products.Where(c => c.IsDeleted == false && c.IsActive&&c.Stock>0).ToList();

            foreach (var product in products.ToList())
            {
                if (currentProducts.Any(c => c.Product.Id == product.Id))
                    products.Remove(product);
            }

            return products;
        }

        [Route("QuickBasket")]
        public ActionResult QuickBasket()
        {
            CartViewModel cart = new CartViewModel();

            List<ProductInCart> productInCarts = GetProductInBasketByCoockie();

            cart.Products = productInCarts;

            decimal subTotal = GetSubtotal(productInCarts);

            cart.SubTotal = subTotal.ToString("n0") + " تومان";

            //decimal discountAmount = GetDiscount();

            //cart.DiscountAmount = discountAmount.ToString("n0") + " تومان";

            //decimal total = subTotal - discountAmount;
            decimal total = subTotal;

            decimal shipmentAmount = GetShipmentAmountByTotal(total);
            cart.ShipmentAmount = shipmentAmount.ToString("N0") + " تومان";



            cart.Total = (total + shipmentAmount).ToString("n0") + " تومان";

            return View(cart);
        }
        private BaseViewModelHelper baseviewmodel = new BaseViewModelHelper();

        public decimal GetShipmentAmountByTotal(decimal totalAmount)
        {
            decimal shipmentAmount = Convert.ToDecimal(baseviewmodel.GetTextItemByName("shipping-amount", "summery"));
            decimal freeShipmentLimitAmount = Convert.ToDecimal(baseviewmodel.GetTextItemByName("free-shipping-amount", "summery"));

            if (totalAmount >= freeShipmentLimitAmount)
                return 0;

            return shipmentAmount;
        }
        //public decimal GetVatByTotal(decimal subTotal)
        //{
        //    //return (subTotal * 9) / 100;
        //    return 0;
        //}

        [Route("Basket/remove/{code}")]
        public ActionResult RemoveFromBasket(string code)
        {
            string[] coockieItems = GetCookie();


            for (int i = 0; i < coockieItems.Length - 1; i++)
            {
                string[] coockieItem = coockieItems[i].Split('^');

                if (coockieItem[0] == code)
                {
                    string removeArray = coockieItem[0] + "^" + coockieItem[1];
                    coockieItems = coockieItems.Where(current => current != removeArray).ToArray();
                    break;
                }
            }

            string cookievalue = null;
            for (int i = 0; i < coockieItems.Length - 1; i++)
            {
                cookievalue = cookievalue + coockieItems[i] + "/";
            }

            HttpContext.Response.Cookies.Set(new HttpCookie("basket-parikatan")
            {
                Name = "basket-parikatan",
                Value = cookievalue,
                Expires = DateTime.Now.AddDays(1)
            });
            RemoveCookie("discount");
            return RedirectToAction("basket");
        }
        //DiscountRequestPost
        [AllowAnonymous]
        public ActionResult DiscountRequestPost(string coupon)
        {
            DiscountCode discount =
                db.DiscountCodes.FirstOrDefault(current => current.Code == coupon &&
                                                           current.IsDeleted == false && current.IsActive);

            List<ProductInCart> productInCarts = GetProductInBasketByCoockie();

            // user-bound / first-order codes are validated against the signed-in user
            if (discount != null && (discount.IsForFirstOrder || discount.UserId != null) && !User.Identity.IsAuthenticated)
                return Json("login", JsonRequestBehavior.AllowGet);

            string result = CheckCouponValidation(discount, GetSubtotal(productInCarts));

            if (result != "true")
                return Json(result, JsonRequestBehavior.AllowGet);


            decimal subTotal = GetSubtotal(productInCarts);

            decimal total = subTotal;

            DiscountHelper helper = new DiscountHelper();

            decimal discountAmount = helper.CalculateDiscountAmount(discount, total);

            SetDiscountCookie(discountAmount.ToString(), coupon);

            return Json("true", JsonRequestBehavior.AllowGet);
        }

        public decimal GetSubtotal(List<ProductInCart> orderDetails)
        {
            decimal subTotal = 0;

            foreach (ProductInCart orderDetail in orderDetails)
            {
                decimal amount = orderDetail.Product.Amount;
                var parentProduct = db.Products.FirstOrDefault(p => p.Id == orderDetail.Product.Id);

                if (parentProduct != null)
                {
                    if (parentProduct.IsInPromotion)
                    {
                        if (orderDetail.Product.DiscountAmount != null)
                            amount = orderDetail.Product.DiscountAmount.Value;
                    }
                }

                subTotal = subTotal + (amount * orderDetail.Quantity);
            }

            return subTotal;
        }
        public List<ProductInCart> GetProductInBasketByCoockie()
        {
            List<ProductInCart> productInCarts = new List<ProductInCart>();

            string[] basketItems = GetCookie();

            if (basketItems != null)
            {
                for (int i = 0; i < basketItems.Length - 1; i++)
                {
                    string[] productItem = basketItems[i].Split('^');

                    int productCode = Convert.ToInt32(productItem[0]);

                    Product product =
                        db.Products.FirstOrDefault(current =>
                                current.IsDeleted == false && current.Id == productCode);

                    if (productItem[1] != "undefined" && productItem[1] != "0")
                    {
                        int qty = Convert.ToInt32(productItem[1]);

                        if (qty > 0 && product != null)
                        {
                            productInCarts.Add(new ProductInCart()
                            {
                                Product = product,
                                Quantity = qty,

                            });
                        }
                    }
                }
            }

            return productInCarts;
        }

        //public decimal GetSubTotal()
        //{
        //    List<ProductInCart> productInCarts = new List<ProductInCart>();
        //    decimal subtotal = 0;
        //    string[] basketItems = GetCookie();

        //    if (basketItems != null)
        //    {
        //        for (int i = 0; i < basketItems.Length - 1; i++)
        //        {
        //            string[] productItem = basketItems[i].Split('^');

        //            int productCode = Convert.ToInt32(productItem[0]);

        //            Product product =
        //                db.Products.Include(c => c.Color).Include(c => c.Size).FirstOrDefault(current =>
        //                    current.IsDeleted == false && current.Id == productCode);

        //            if (product != null)
        //            {
        //                if (productItem[1] != "undefined" && productItem[1] != "0")
        //                {
        //                    int qty = Convert.ToInt32(productItem[1]);

        //                    decimal amount = product.Amount;

        //                    if (product.IsInPromotion && product.DiscountAmount != null)
        //                        amount = product.DiscountAmount.Value;

        //                    if (qty > 0)
        //                    {
        //                        subtotal = subtotal + (amount * qty);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return subtotal;
        //}

        public void SetCookie(string code, string quantity)
        {
            string cookievalue = null;

            if (Request.Cookies["basket-parikatan"] != null)
            {
                bool changeCurrentItem = false;

                cookievalue = Request.Cookies["basket-parikatan"].Value;

                string[] coockieItems = cookievalue.Split('/');

                for (int i = 0; i < coockieItems.Length - 1; i++)
                {
                    string[] coockieItem = coockieItems[i].Split('^');

                    if (coockieItem[0] == code)
                    {
                        coockieItem[1] = (quantity).ToString();
                        changeCurrentItem = true;
                        coockieItems[i] = coockieItem[0] + "^" + coockieItem[1];
                        break;
                    }
                }

                if (changeCurrentItem)
                {
                    cookievalue = null;
                    for (int i = 0; i < coockieItems.Length - 1; i++)
                    {
                        cookievalue = cookievalue + coockieItems[i] + "/";
                    }

                }
                else
                    cookievalue = cookievalue + code + "^" + quantity + "/";

            }
            else
                cookievalue = code + "^" + quantity + "/";

            HttpContext.Response.Cookies.Set(new HttpCookie("basket-parikatan")
            {
                Name = "basket-parikatan",
                Value = cookievalue,
                Expires = DateTime.Now.AddDays(1)
            });
        }

        public string[] GetCookie()
        {
            if (Request.Cookies["basket-parikatan"] != null)
            {
                string cookievalue = Request.Cookies["basket-parikatan"].Value;

                string[] basketItems = cookievalue.Split('/');

                return basketItems;
            }

            return null;
        }

        [AllowAnonymous]
        public string CheckCouponValidation(DiscountCode discount, decimal total)
        {
            //     Guid roleIdCustomer = new Guid("5812D0C0-264F-4A9B-96BB-42DFC70538E6");

            //if (total < 300000)
            //    return "maxtotal";

            if (discount == null)
                return "Invald";


            if (!discount.IsMultiUsing)
            {
                if (discount.IsUsed)
                    return "Used";
            }

            if (discount.ExpireDate < DateTime.Today)
                return "Expired";


            if (discount.IsForFirstOrder)
            {
                var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;

                string name = identity.FindFirst(System.Security.Claims.ClaimTypes.Name).Value;

                long userId = Int64.Parse(name);

                if (db.Orders.Any(c => c.UserId == userId && c.IsPaid && c.SaleReferenceId != null))
                {
                    return "notFirstOrder";
                }

                return "true";
            }

            if (discount.UserId != null)
            {
                long userId = Int64.Parse(User.Identity.Name);

                if (discount.UserId != userId)
                {
                    User user =
                        db.Users.FirstOrDefault(c => c.Id == userId && c.IsDeleted == false && c.SecurityRole == SecurityRole.Customer);

                    var otherUser = db.Users.Where(c =>
                        c.CellNum == user.CellNum && c.IsDeleted == false && c.SecurityRole == SecurityRole.Customer).ToList();

                    foreach (User user1 in otherUser)
                    {
                        if (user1.Id == discount.UserId)
                            return "true";
                    }

                    return "Invald";
                }
            }


            return "true";
        }


        public void SetDiscountCookie(string discountAmount, string discountCode)
        {
            HttpContext.Response.Cookies.Set(new HttpCookie("discount")
            {
                Name = "discount",
                Value = discountAmount + "|" + discountCode,
                Expires = DateTime.Now.AddDays(1)
            });
        }
        public decimal GetDiscount()
        {
            if (Request.Cookies["discount"] != null)
            {
                try
                {
                    string cookievalue = Request.Cookies["discount"].Value;

                    string[] basketItems = cookievalue.Split('|');
                    return Convert.ToDecimal(basketItems[0]);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            return 0;
        }

        //checkout
        [Route("checkout")]
        public ActionResult CheckOut()
        {
            if (User.Identity.IsAuthenticated)
            {
                //RemoveCookie("discount");

                CheckOutViewModel checkOut = new CheckOutViewModel();
                var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
                string role = identity.FindFirst(System.Security.Claims.ClaimTypes.Role).Value;
                string id = identity.FindFirst(System.Security.Claims.ClaimTypes.Name).Value;
                long userId = Int64.Parse(id);

                User user = db.Users.Find(userId);


                if (user != null)
                {


                    UserInformation userInformation = new UserInformation()
                    {
                        FullName = user.FullName,
                        CellNumber = user.CellNum,
                        //Address = user.Address,
                        //PostalCode = user.PostalCode,
                        //ProvinceId = user.ProvinceId.ToString(),
                        //CityId = user.CityId.ToString()
                    };

                    checkOut.UserInformation = userInformation;
                }

                if (role != SecurityRole.Customer)
                {
                    return Redirect("/login?ReturnUrl=checkout&type=role");
                }



                List<ProductInCart> productInCarts = GetProductInBasketByCoockie();

                checkOut.Products = productInCarts;

                decimal subTotal = GetSubtotal(productInCarts);

                checkOut.SubTotal = subTotal.ToString("n0") + " تومان";


                decimal discountAmount = GetDiscount();

                checkOut.DiscountAmount = discountAmount.ToString("n0") + " تومان";

                decimal total = subTotal - discountAmount;

                decimal shipmentAmount = GetShipmentAmountByTotal(total);

                checkOut.ShipmentAmount = shipmentAmount.ToString("N0") + " تومان";

                //decimal vatAmount = GetVatByTotal(total);
                //checkOut.VatAmount = vatAmount.ToString("N0") + " تومان";


                checkOut.Total = (total + shipmentAmount).ToString("n0") + " تومان";

                checkOut.SubTotalValue = subTotal;
                checkOut.DiscountValue = discountAmount;
                checkOut.ShipmentValue = shipmentAmount;
                checkOut.TotalValue = total + shipmentAmount;
                checkOut.ItemCount = productInCarts.Sum(c => c.Quantity);
                ViewBag.Title = "تکمیل سفارش | پریکاتن";
                ViewBag.Robots = "noindex, nofollow";


                checkOut.Provinces = db.Provinces.OrderBy(current => current.Title).ToList();
                checkOut.Cities = db.Cities.OrderBy(current => current.Title).ToList();

                checkOut.OnlinePay = GetTextByName("online-payment");
                checkOut.TransferPay = GetTextByName("transfer-payment");
                checkOut.RecievePay = GetTextByName("recieve-payment");

                ViewBag.CityId = new SelectList(db.Cities, "Id", "Title");



                return View(checkOut);
            }

            return Redirect("/login?ReturnUrl=checkout");

        }

        public string GetTextByName(string name)
        {
            var textItem = db.TextItems.Where(c => c.Name == name).Select(c => c.Summery).FirstOrDefault();

            if (textItem != null)
                return textItem;
            return string.Empty;
        }
        public ActionResult FillCities(string id)
        {
            long provinceId = Int64.Parse(id);
            //   ViewBag.cityId = ReturnCities(provinceId);
            var cities = db.Cities.Where(c => c.ProvinceId == provinceId).OrderBy(current => current.Title).ToList();
            List<CityItemViewModel> cityItems = new List<CityItemViewModel>();
            foreach (City city in cities)
            {
                cityItems.Add(new CityItemViewModel()
                {
                    Text = city.Title,
                    Value = city.Id.ToString()
                });
            }
            return Json(cityItems, JsonRequestBehavior.AllowGet);
        }

        public List<long> GetNonStockProductList(List<OrderDetail> orderDetails)
        {

            List<long> nonStockProducts = new List<long>();

            foreach (OrderDetail orderDetail in orderDetails)
            {
                Product product = db.Products.FirstOrDefault(current =>
                    current.IsDeleted == false && current.IsActive && current.Id == orderDetail.ProductId);

                if (product == null || product.Stock < orderDetail.Quantity)
                {
                    nonStockProducts.Add(orderDetail.ProductId);
                }
            }

            return nonStockProducts;
        }

        ZarinPalHelper zp = new ZarinPalHelper();

        public long? GetPaymentType(string bank)
        {
            if (bank == "zarinpal")
            {
                var paymentType = db.PaymentTypes.Find(1);
                if (paymentType != null)
                    return paymentType.Id;

                return null;
            }

            return null;
        }

        public ActionResult Finalize(string notes, string cellnumber, string postal,
                                    string address, string city, string fullname, string bank)
        {
            try
            {
                // a signed-in customer always orders with the number of their own account
                // (the checkout page shows it read-only; this keeps a tampered request from changing it)
                if (User.Identity.IsAuthenticated)
                {
                    var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;
                    var role = identity.FindFirst(System.Security.Claims.ClaimTypes.Role);
                    var cell = identity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                    if (role != null && role.Value == SecurityRole.Customer && cell != null && !string.IsNullOrEmpty(cell.Value))
                        cellnumber = cell.Value;
                }

                //if (cellnumber.Length < 11)
                //{

                //    TempData["WrongMobile"] = "شماره موبایل وارد شده صحیح نمی باشد";
                //    return View();
                //}

                if (notes == "quickBasket")
                {



                    List<ProductInCart> productInCarts = GetProductInBasketByCoockie();

                    if (productInCarts.Count == 0)
                    {
                        return Json("emptyBasket", JsonRequestBehavior.AllowGet);
                    }

                    CookieToOrderViewModel orderAndDetails = ConvertCoockieToOrder(productInCarts);

                    Order order = orderAndDetails.Order;
                    string referrerSession = Session["Referrer"]?.ToString();
                    if (!string.IsNullOrEmpty(referrerSession) && !string.IsNullOrWhiteSpace(referrerSession))
                    {
                        var referral = db.Referrals.FirstOrDefault(r => r.IsDeleted == false && r.IsActive && r.ReferralLink == referrerSession);
                        if (referral != null)
                        {
                            order.ReferralId = referral.Id;
                            db.SaveChanges();
                        }
                    }
                    if (order != null)
                    {
                        List<long> nonStockProducts = GetNonStockProductList(orderAndDetails.OrderDetails);


                        User user = db.Users.FirstOrDefault(c => c.CellNum == cellnumber && c.SecurityRole == SecurityRole.Customer);

                        if (user != null)
                        {
                            if (string.IsNullOrEmpty(user.FullName))
                            {
                                user.FullName = fullname;
                                db.Entry(user).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        else
                        {

                            user = new User()
                            {

                                CellNum = cellnumber,
                                IsDeleted = false,
                                IsActive = true,
                                CreationDate = DateTime.Now,
                                SecurityRole = SecurityRole.Customer,
                                FullName = fullname
                            };

                            db.Users.Add(user);
                            db.SaveChanges();
                        }

                        if (!nonStockProducts.Any())
                        {
                            order.UserId = user.Id;
                            order.DeliverFullName = fullname;
                            order.DeliverCellNumber = cellnumber;
                            //order.Address = address;
                            //order.PostalCode = postal;
                            order.Description = notes;
                            //order.CityId = Int64.Parse(city);
                            order.TotalAmount = GetTotalAmount(order.SubTotal, order.DiscountAmount,
                                order.ShippingAmount, 0);
                            order.PaymentTypeId = GetPaymentType(bank);

                            db.SaveChanges();
                            

                            string res = "";
                            decimal onlineAmount = Convert.ToDecimal(order.TotalAmount * 10);



                            if (bank == "zarinpal")
                            {
                                res = zp.ZarinPalRedirect(order, order.TotalAmount);
                            }

                            return Json(res, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            string returnProducts = "";
                            foreach (long productId in nonStockProducts)
                            {
                                Product product = db.Products.Find(productId);

                                if (product != null)
                                    returnProducts = returnProducts + product.Title + "|";
                            }

                            return Json("nonstock|" + returnProducts, JsonRequestBehavior.AllowGet);

                        }
                    }

                    return Json("/login?ReturnUrl=checkout", JsonRequestBehavior.AllowGet);

                }
                else
                {
                    if (User.Identity.IsAuthenticated)
                    {
                        var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;

                        string name = identity.FindFirst(System.Security.Claims.ClaimTypes.Name).Value;

                        long userId = Int64.Parse(name);

                        List<ProductInCart> productInCarts = GetProductInBasketByCoockie();

                        if (productInCarts.Count == 0)
                        {
                            return Json("emptyBasket", JsonRequestBehavior.AllowGet);
                        }

                        CookieToOrderViewModel orderAndDetails = ConvertCoockieToOrder(productInCarts);

                        Order order = orderAndDetails.Order;
                        string referrerSession = Session["Referrer"]?.ToString();
                        if (!string.IsNullOrEmpty(referrerSession) && !string.IsNullOrWhiteSpace(referrerSession))
                        {
                            var domain = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority);

                            string referrerLink = domain + referrerSession;

                            var referral = db.Referrals.FirstOrDefault(r => r.IsDeleted == false && r.IsActive && r.ReferralLink == referrerLink);
                            if (referral != null)
                            {
                                order.ReferralId = referral.Id;
                                db.SaveChanges();
                            }
                        }
                        if (order != null)
                        {
                            List<long> nonStockProducts = GetNonStockProductList(orderAndDetails.OrderDetails);


                            User user = db.Users.Find(userId);

                            if (user != null)
                            {
                                if (string.IsNullOrEmpty(user.FullName))
                                {
                                    user.FullName = fullname;
                                    db.Entry(user).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }

                            if (!nonStockProducts.Any())
                            {
                                order.UserId = userId;
                                order.DeliverFullName = fullname;
                                order.DeliverCellNumber = cellnumber;
                                order.Address = address;
                                order.PostalCode = postal;
                                order.CustomerDesc = notes;
                                order.CityId = Int64.Parse(city);
                                order.TotalAmount = GetTotalAmount(order.SubTotal, order.DiscountAmount,
                                    order.ShippingAmount, 0);
                                order.PaymentTypeId = GetPaymentType(bank);

                                db.SaveChanges();

                                string res = "";
                                decimal onlineAmount = Convert.ToDecimal(order.TotalAmount * 10);



                                if (bank == "zarinpal")
                                {
                                    res = zp.ZarinPalRedirect(order, order.TotalAmount);
                                }

                                return Json(res, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                string returnProducts = "";
                                foreach (long productId in nonStockProducts)
                                {
                                    Product product = db.Products.Find(productId);

                                    if (product != null)
                                        returnProducts = returnProducts + product.Title + "|";
                                }

                                return Json("nonstock|" + returnProducts, JsonRequestBehavior.AllowGet);

                            }
                        }
                    }

                    return Json("/login?ReturnUrl=checkout", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json("false", JsonRequestBehavior.AllowGet);

            }
        }


        public ActionResult FinalizeInCallBack(string orderId, string cellnumber, string postal,
                                    string address, string city, string fullname)
        {
            try
            {




                long id = Convert.ToInt64(orderId);
                ;

                Order order = db.Orders.Find(id);

                if (order != null)
                {



                    order.DeliverFullName = fullname;
                    order.DeliverCellNumber = cellnumber;
                    order.Address = address;
                    order.PostalCode = postal;
                    order.CustomerDesc = city;
                  
                    db.SaveChanges();

                  

                    return Json("true", JsonRequestBehavior.AllowGet);


                }

                return Json("false", JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {
                return Json("false", JsonRequestBehavior.AllowGet);

            }
        }

        //[Route("gateway")]
        //public ActionResult Gateway()
        //{
        //    return View();
        //}
        //public void SendTanksSms(string cellNumber, string code)
        //{
        //    // SendSms.SendOtpSms(cellNumber, code);
        //}





        //#region Finalize



        public decimal GetTotalAmount(decimal? subtotal, decimal? discount, decimal? shippment, decimal? vatAmount)
        {
            decimal discountAmount = 0;
            if (discount != null)
                discountAmount = (decimal)discount;

            decimal shipmentAmount = 0;
            if (shippment != null)
                shipmentAmount = (decimal)shippment;

            if (subtotal == null)
                subtotal = 0;

            decimal vatAmountDecimal = 0;
            if (vatAmount != null)
                vatAmountDecimal = (decimal)vatAmount;

            return (decimal)subtotal - discountAmount + shipmentAmount + vatAmountDecimal;
        }
        public CookieToOrderViewModel ConvertCoockieToOrder(List<ProductInCart> products)
        {
            try
            {


                Order order = new Order();


                order.IsActive = true;
                order.IsDeleted = false;
                order.IsPaid = false;
                order.CreationDate = DateTime.Now;
                order.LastModifiedDate = DateTime.Now;
                order.OrderStatus = OrderStatus.WaitForPayment;
                order.SubTotal = GetSubtotal(products);
                order.OrderType = OrderType.Site;
                order.DiscountAmount = GetDiscount();
                order.DiscountCodeId = GetDiscountId();
                order.ShippingAmount =
                    GetShipmentAmountByTotal(Convert.ToDecimal(order.SubTotal - order.DiscountAmount));

                //order.VatAmount = GetVatByTotal(Convert.ToDecimal(order.SubTotal - order.DiscountAmount));

                order.TotalAmount = Convert.ToDecimal(order.SubTotal + order.ShippingAmount - order.DiscountAmount);
                
                db.Orders.Add(order);

                List<OrderDetail> orderDetails = new List<OrderDetail>();
                foreach (ProductInCart product in products)
                {
                    decimal amount = product.Product.Amount;

                    if (product.Product.IsInPromotion)
                    {
                        if (product.Product.DiscountAmount != null)
                            amount = product.Product.DiscountAmount.Value;
                    }

                    OrderDetail orderDetail = new OrderDetail()
                    {
                        ProductId = product.Product.Id,
                        Quantity = product.Quantity,
                        RowAmount = amount * product.Quantity,
                        IsDeleted = false,
                        IsActive = true,
                        CreationDate = DateTime.Now,
                        OrderId = order.Id,
                        Amount = amount,
                        PurchaseAmount = product.Product.PurchaseAmount,
                        PurchaseRowAmount = product.Product.PurchaseAmount * product.Quantity
                    };
                    orderDetails.Add(orderDetail);
                    db.OrderDetails.Add(orderDetail);
                }
                CookieToOrderViewModel result = new CookieToOrderViewModel()
                {
                    Order = order,
                    OrderDetails = orderDetails
                };
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public long? GetDiscountId()
        {
            if (Request.Cookies["discount"] != null)
            {
                try
                {
                    string cookievalue = Request.Cookies["discount"].Value;

                    string[] basketItems = cookievalue.Split('|');
                    string code = basketItems[1];
                    DiscountCode discountCode =
                        db.DiscountCodes.FirstOrDefault(current => current.Code == code);

                    return discountCode?.Id;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            return null;
        }
        //public User CreateUser(string fullName, string cellNumber)
        //{


        //    Random rnd = new Random();

        //    User oUser =
        //        db.Users.FirstOrDefault(current => current.CellNum == cellNumber && current.IsDeleted == false);

        //    if (oUser == null)
        //    {
        //        User user = new User()
        //        {
        //            CellNum = cellNumber,
        //            FullName = fullName,
        //            Password = rnd.Next(1000, 9999).ToString(),
        //            IsDeleted = false,
        //            IsActive = true,
        //            CreationDate = DateTime.Now,
        //            SecurityRole = SecurityRole.Customer

        //        };

        //        db.Users.Add(user);
        //        db.SaveChanges();
        //        return user;
        //    }
        //    return oUser;
        //}


        public void RemoveCookie(string name)
        {
            if (Request.Cookies[name] != null)
            {
                Response.Cookies[name].Expires = DateTime.Now.AddDays(-1);
            }
        }
        //#endregion


        private String MerchantId = WebConfigurationManager.AppSettings["MerchantId"];



        [Route("callback/zp")]
        public ActionResult Zarinpalcallback(string authority, string status)
        {
            String Status = status;
            CallBackViewModel callBack = new CallBackViewModel();
            ZarinPalHelper zarinPal = new ZarinPalHelper();
            if (Status != "OK")
            {
                callBack.IsSuccess = false;
                callBack.RefrenceId = "414";
                Order order = zarinPal.GetOrderByAuthority(authority);
                if (order != null)
                {
                    callBack.Order = order;
                    callBack.OrderDetails = db.OrderDetails
                                .Where(c => c.OrderId == order.Id && c.IsDeleted == false).Include(c => c.Product).ToList();
                }
            }

            else
            {
                try
                {
                    var zarinpal = ZarinPal.ZarinPal.Get();
                   zarinpal.DisableSandboxMode();
                   //  zarinpal.EnableSandboxMode();
                    String Authority = authority;
                    long Amount = zarinPal.GetAmountByAuthority(Authority);

                    var verificationRequest = new ZarinPal.PaymentVerification(MerchantId, Amount, Authority);
                    var verificationResponse = zarinpal.InvokePaymentVerification(verificationRequest);
                    if (verificationResponse.Status == 100 || verificationResponse.Status == 101)
                    {
                        Order order = zarinPal.GetOrderByAuthority(authority);
                        if (order != null)
                        {
                            if (!order.IsPaid)
                            {
                                UpdateOrder(order.Id, verificationResponse.RefID);

                                callBack.Order = order;
                                callBack.IsSuccess = true;
                                callBack.OrderCode = order.Id.ToString();
                                callBack.RefrenceId = verificationResponse.RefID;

                                callBack.OrderDetails = db.OrderDetails
                                    .Where(c => c.OrderId == order.Id && c.IsDeleted == false).Include(c => c.Product)
                                    .ToList();
                                foreach (OrderDetail orderDetail in callBack.OrderDetails)
                                {
                                    Product product = orderDetail.Product;
                                    product.Stock = orderDetail.Product.Stock - orderDetail.Quantity;

                                    if (product.Stock <= 0)
                                    {
                                        product.IsAvailable = false;
                                    }

                                    db.SaveChanges();
                                }

                                RemoveCookie("basket-parikatan");

                                SendSms.SendCommonSmsV2("09124806404", "یک سفارش در سایت پریکاتن ثبت گردید.");

                                var user = db.Users.Find(order.UserId);
                                if (user != null)
                                    SendSms.SendThanksSmsV2(user.CellNum, user.FullName);
                            }
                            else
                            {
                                return Redirect("/");
                            }
                        }
                        else
                        {
                            callBack.IsSuccess = false;
                            callBack.RefrenceId = "سفارش پیدا نشد";
                        }
                    }
                    else
                    {
                        callBack.IsSuccess = false;
                        callBack.RefrenceId = verificationResponse.Status.ToString();
                    }
                }
                catch (Exception e)
                {
                    callBack.IsSuccess = false;
                    callBack.RefrenceId = "خطا سیستمی. لطفا با پشتیبانی سایت تماس بگیرید";
                }
            }

            return View(callBack);
        }

        //[Route("thank-you")]
        //public ActionResult FreeCallBack(Guid orderId)
        //{
        //    CallBackViewModel callBack = new CallBackViewModel();

        //    Order order = db.Orders.Find(orderId);

        //    if (order == null)
        //    {
        //        callBack.IsSuccess = false;
        //        callBack.RefrenceId = "414";
        //    }


        //    else
        //    {
        //        try
        //        {
        //            callBack.Order = order;
        //            callBack.IsSuccess = true;
        //            callBack.OrderCode = order.Id.ToString();
        //            callBack.OrderDetails = db.OrderDetails.Where(c => c.OrderId == order.Id).ToList();

        //        }
        //        catch (Exception e)
        //        {
        //            callBack.IsSuccess = false;
        //            callBack.RefrenceId = "خطا سیستمی. لطفا با پشتیبانی سایت تماس بگیرید";
        //        }
        //    }

        //    return View(callBack);
        //}





        //public void ChangeStockAfterPayment(long orderId)
        //{
        //    List<OrderDetail> orderDetails = db.OrderDetails.Where(current =>
        //        current.OrderId == orderId && current.IsDeleted == false && current.IsActive == true).ToList();

        //    foreach (OrderDetail orderDetail in orderDetails)
        //    {
        //        Product product = db.Products.FirstOrDefault(current => current.Id == orderDetail.ProductId);

        //        if (product != null)
        //        {
        //            product.Stock = product.Stock
        //                            - orderDetail.Quantity;
        //        }
        //    }
        //}

        public void UpdateOrder(long orderId, string refId)
        {
            Order order = db.Orders.Find(orderId);

            order.IsPaid = true;
            order.PaymentDate = DateTime.Now;
            order.SaleReferenceId = refId;
            order.OrderStatus = OrderStatus.InProgress;
            order.LastModifiedDate = DateTime.Now;


            db.SaveChanges();
        }
    }
}