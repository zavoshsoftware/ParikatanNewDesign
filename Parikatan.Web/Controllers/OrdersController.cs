

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Core.Enums;
//using Clotho.Core.Enums;
using Helpers;
using Kendo.Mvc.Extensions;
using Models;
using Models.Entities;
using Parikatan.Web.ViewModels.Orders;
using ViewModels.Basket;
using ViewModels.Orders;

//using Newtonsoft.Json.Linq;
//using ViewModels;
//using ViewModels.Orders;
//using ZarinPal;

namespace Parikatan.Web.Controllers
{
    public class OrdersController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
        public async Task<ActionResult> Index(Guid? id, string orderType)
        {
            ViewBag.OrderType = orderType;


            //  bool isPos = orderType != null;

            if (id != null)
            {
                Guid sendStatus = new Guid("d719d03e-3f85-4590-a451-b44ead16c162");
                if (id == new Guid("11869F4D-D6D1-434C-A2F1-7945227CD3BB"))
                {

                    var orders = await db.Orders.AsNoTracking().Where(o =>
                            (o.OrderStatus == OrderStatus.InProgress) &&
                            o.IsDeleted == false)
                        .OrderByDescending(o => o.CreationDate)
                        .Include(o => o.User).ToListAsync();
                    ViewBag.CurrentOrder = "سفارشات جاری";
                    return View(orders.ToList());
                }
                else
                {
                    var orders = await db.Orders.AsNoTracking().Where(o =>
                            (o.OrderStatus == OrderStatus.Delivered || o.OrderStatus == OrderStatus.Sent) && o.IsDeleted == false)
                        .OrderByDescending(o => o.CreationDate)
                        .Include(o => o.User).ToListAsync();
                    if (id == new Guid("68490C34-A666-44DA-8D9F-7B6DEE2E4DE0"))
                    {
                        ViewBag.CurrentOrder = "سفارشات تحویل داده شده";
                        orders = orders.Where(o => (o.OrderStatus == OrderStatus.Delivered || o.OrderStatus == OrderStatus.Sent)).ToList();
                    }
                    else
                        ViewBag.CurrentOrder = "سفارشات لغو شده";
                    return View(orders.ToList());
                }
            }
            else
            {

                var orders = await db.Orders.AsNoTracking().Where(o => o.IsDeleted == false)
                     .OrderByDescending(o => o.CreationDate)
                     .Include(o => o.User).ToListAsync();
                //if (isPos)
                //    ViewBag.CurrentOrder = "همه سفارشات فروشگاه";
                //else
                //    ViewBag.CurrentOrder = "همه سفارشات";
                return View(orders.ToList());
            }
        }


        [Authorize(Roles = "Marketing")]
        public async Task<ActionResult> IndexMarketing()
        {
            

            var identity = (System.Security.Claims.ClaimsIdentity)User.Identity;

            string name = identity.FindFirst(System.Security.Claims.ClaimTypes.Name).Value;

            long userId = Int64.Parse(name);

            //  bool isPos = orderType != null;

            var user = db.Users.FirstOrDefault(c => c.Id == userId);

            long? discountCodeId = null;


            discountCodeId = user.DiscountCodeId;
            

            if (discountCodeId != null)
            {

                var orders = await db.Orders.AsNoTracking().Where(o =>
                        (o.OrderStatus == OrderStatus.Delivered || o.OrderStatus == OrderStatus.Sent ||
                         o.OrderStatus == OrderStatus.InProgress) && o.IsDeleted == false&&o.DiscountCodeId==discountCodeId)
                    .OrderByDescending(o => o.CreationDate)
                    .Include(o => o.User).ToListAsync();
                return View(orders.ToList());

            }
            else
            {
                return View(new List<Order>());
            }




        }

        [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            List<OrderDetail> orderDetails = await db.OrderDetails.Include(current => current.Product)
               .Where(current => current.OrderId == order.Id && current.IsDeleted == false).ToListAsync();

            bool isChange = false;


            if (isChange)
                db.SaveChanges();

            OrderDetailViewModel orderDetailViewModel = new OrderDetailViewModel()
            {
                Order = order,
                OrderDetails = orderDetails
            };



            return View(orderDetailViewModel);
        }

        [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]

        public ActionResult Create()
        {
            ViewBag.CityId = new SelectList(db.Cities, "Id", "Title");
            ViewBag.DiscountCodeId = new SelectList(db.DiscountCodes, "Id", "Code");
            ViewBag.UserId = new SelectList(db.Users, "Id", "Password");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.



        [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Order order = db.Orders.Find(id);
            order.IsDeleted = true;
            order.DeletionDate = DateTime.Now;

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



        [AllowAnonymous]
        public ActionResult ChangeOrderStatus(string code, string statusId)
        {

            //Guid id = new Guid(statusId);
            int orderCode = Convert.ToInt32(code);
            Order order = db.Orders.FirstOrDefault(c => c.Id == orderCode);

            if (order != null)
            {
                order.OrderStatus = (OrderStatus)Enum.Parse(typeof(OrderStatus), statusId);
                order.LastModifiedDate = DateTime.Now;

                if (order.OrderStatus == OrderStatus.Sent)
                {
                    var orderDetails = db.OrderDetails.Where(c => c.OrderId == order.Id);
                    foreach (var orderDetail in orderDetails)
                    {
                        if (orderDetail.Product.IsPackage == false)
                        {
                            UpdateInventory(orderDetail.ProductId,
                                orderDetail.Quantity,
                                order.Id,
                                order.DeliverFullName,
                                order.OrderDateStr);
                        }
                        else
                        {
                            var productPackageList = db.ProductPackageItemss
                                .Where(c => c.ProductId == orderDetail.ProductId).ToList();


                            foreach (var productPackageItem in productPackageList)
                            {
                                UpdateInventory(productPackageItem.ChildId,
                                    orderDetail.Quantity,
                                    order.Id,
                                    order.DeliverFullName,
                                    order.OrderDateStr);
                            }
                        }
                    }
                }
                db.SaveChanges();

            }
            return Json("true", JsonRequestBehavior.AllowGet);
        }

        public void UpdateInventory(long productId, int orderDetailQty, long orderId, string orderFullName, string orderDate)
        {
            var inventory = db.Inventories.FirstOrDefault(c => c.ProductId == productId);
            if (inventory != null)
            {
                inventory.Stock -= orderDetailQty;

                InventoryDetail inventoryDetail = new InventoryDetail()
                {
                    InventoryId = inventory.Id,
                    CreationDate = DateTime.Now,
                    InventoryDetailType = InventoryDetailType.Sale,
                    IsActive = true,
                    IsDeleted = false,
                    RefrenceId = orderId,
                    Stock = -orderDetailQty,
                    TransactionDate = DateTime.Now,
                    Description = "سفارش " + orderFullName + " در تاریخ: " +
                                  orderDate + " با شماره: " + orderId
                };
                db.InventoryDetails.Add(inventoryDetail);

            }
        }

        public void ChangeStockAfterCansele(Order order, Guid newstatusId)
        {
            if (newstatusId == new Guid("99c42ad7-ae29-46c5-bae2-00d2da31ddcd"))
            {
                List<OrderDetail> orderDetails = db.OrderDetails.Where(c => c.OrderId == order.Id).ToList();

                foreach (OrderDetail orderDetail in orderDetails)
                {
                    orderDetail.Product.Stock = orderDetail.Product.Stock + orderDetail.Quantity;
                    if (orderDetail.Product.Stock > 0)
                        orderDetail.Product.IsAvailable = true;
                }
            }
        }

        //public void ChangeCanseledStock(Order order, Guid newstatusId)
        //{
        //    if (order.OrderStatusId == new Guid("99c42ad7-ae29-46c5-bae2-00d2da31ddcd") &&
        //        (newstatusId == new Guid("7DBF85F4-7835-4D21-8269-26695D0C7E0F") || newstatusId == new Guid("68490C34-A666-44DA-8D9F-7B6DEE2E4DE0") ||
        //         newstatusId == new Guid("EC934A7E-0061-4B09-BD44-CA5120CF6200") ||
        //         newstatusId == new Guid("11869f4d-d6d1-434c-a2f1-7945227cd3bb")))
        //    {
        //        List<OrderDetail> orderDetails = db.OrderDetails.Where(c => c.OrderId == order.Id).ToList();

        //        foreach (OrderDetail orderDetail in orderDetails)
        //        {
        //            orderDetail.Product.Stock = orderDetail.Product.Stock - orderDetail.Quantity;
        //            if (orderDetail.Product.Stock <= 0)
        //                orderDetail.Product.IsAvailable = false;
        //        }
        //    }
        //}

        [Authorize(Roles = "SuperAdmin")]
        public ActionResult ChangeOrderPaymentStatus(string code, string statusId, string paymentDesc)
        {

            bool isPay = Convert.ToBoolean(statusId);
            int orderCode = Convert.ToInt32(code);
            Order order = db.Orders.FirstOrDefault(c => c.Id == orderCode);

            if (order != null)
            {
                order.IsPaid = isPay;
                order.LastModifiedDate = DateTime.Now;
                db.SaveChanges();

            }
            return Json("true", JsonRequestBehavior.AllowGet);
        }



        //[AllowAnonymous]
        //public ActionResult ConfirmOrderSmsToUser(string code)
        //{
        //    string confirmOrderSms = "";

        //    int orderCode = Convert.ToInt32(code);
        //    Order order = db.Orders.FirstOrDefault(c => c.Code == orderCode);

        //    if (order != null)
        //    {
        //        confirmOrderSms = order.User.FullName + " عزیز، سفارش کد " + code +
        //                          "در وب سایت چرم نیکلاس، با موفقیت تایید گردید.";
        //        SendSms.SendCommonSms(order.User.CellNum, confirmOrderSms);
        //        UserMessage userMessage = new UserMessage();
        //        userMessage.MessageText = confirmOrderSms;
        //        userMessage.UserId = ToGuid(order.UserId);
        //        userMessage.CreationDate = DateTime.Now;
        //        userMessage.IsDeleted = false;
        //        userMessage.Id = Guid.NewGuid();
        //        db.UserMessages.Add(userMessage);
        //        db.SaveChanges();
        //    }
        //    return Json("true", JsonRequestBehavior.AllowGet);
        //}

        //[AllowAnonymous]
        //public ActionResult SendProductSmsToUser(string code)
        //{

        //    string sendProductSms = "";
        //    int orderCode = Convert.ToInt32(code);
        //    Order order = db.Orders.FirstOrDefault(c => c.Code == orderCode);

        //    if (order != null)
        //    {
        //        sendProductSms = order.User.FullName + " عزیز، سفارش کد " + code +
        //                         "در وب سایت چرم نیکلاس، ارسال گردید.";
        //        SendSms.SendCommonSms(order.User.CellNum, sendProductSms);
        //        UserMessage userMessage = new UserMessage();
        //        userMessage.MessageText = sendProductSms;
        //        userMessage.UserId = ToGuid(order.UserId);
        //        userMessage.CreationDate = DateTime.Now;
        //        userMessage.IsDeleted = false;
        //        userMessage.Id = Guid.NewGuid();
        //        db.UserMessages.Add(userMessage);
        //        db.SaveChanges();
        //    }
        //    return Json("true", JsonRequestBehavior.AllowGet);
        //}

        public static Guid ToGuid(Guid? source)
        {
            return source ?? Guid.Empty;
        }

        [AllowAnonymous]
        public ActionResult SubmitOrderDesc(string code, string desc)
        {


            int orderCode = Convert.ToInt32(code);
            Order order = db.Orders.FirstOrDefault(c => c.Id == orderCode);

            if (order != null)
            {
                order.Description = desc;
                order.LastModifiedDate = DateTime.Now;
                db.SaveChanges();

            }
            return Json("true", JsonRequestBehavior.AllowGet);
        }

        //public string SerOrderStatus()
        //{
        //    Guid id = new Guid("7dbf85f4-7835-4d21-8269-26695d0c7e0f");
        //    List<Order> orders = db.Orders.Where(c => c.OrderStatusId == id).ToList();

        //    foreach (Order order in orders)
        //    {
        //        order.OrderStatusId = new Guid("11869f4d-d6d1-434c-a2f1-7945227cd3bb");

        //    }

        //    db.SaveChanges();
        //    return string.Empty;
        //}


        public async Task<ActionResult> GetUserOrder(long id)
        {
            var userOrders = await db.Orders.Where(x => x.UserId == id).ToListAsync();
            return View(userOrders);
        }

        [AllowAnonymous]
        [Route("co/{id:long}")]
        public ActionResult CustomerOrderRedirect(long id, string factorType)
        {
            Order order = db.Orders.Find(id);
            if (order != null)
            {
                if (factorType == "co")
                    return RedirectToAction("CustomerOrder",
                        new { code = order.Id, idThreeDigit = "123" });
                else
                    return RedirectToAction("CustomerOrderWithoutAddress",
                    new { code = order.Id, idThreeDigit = "123" });

            }
            return HttpNotFound();

        }

        [AllowAnonymous]
        [Route("co/{idThreeDigit}/{code:int}")]
        public ActionResult CustomerOrder(int code, string idThreeDigit)
        {

            Order order = db.Orders.Include(c => c.City).Include(c => c.User).FirstOrDefault(c => c.Id == code);
            if (order == null)
            {
                return HttpNotFound();
            }

            if (order.CityId != null)
            {
                ViewBag.ProvinceTitle = db.Provinces.Find(order.City.ProvinceId)?.Title;
            }
            //string id = order.Id.ToString().Substring(0, 3);

            //if (id != idThreeDigit)
            //{
            //    return HttpNotFound();
            //}


            ViewBag.PaymentAmountStr = order.PaymentAmountStr;
            ViewBag.RemainAmountStr = order.RemainAmountStr;



            List<OrderDetail> orderDetails = db.OrderDetails.Include(current => current.Product)
                .Where(current => current.OrderId == order.Id && current.IsDeleted == false).ToList();

            OrderDetailViewModel orderDetailViewModel = new OrderDetailViewModel()
            {
                Order = order,
                OrderDetails = orderDetails
            };

            return View(orderDetailViewModel);
        }


        [AllowAnonymous]
        [Route("cow/{idThreeDigit}/{code:int}")]
        public ActionResult CustomerOrderWithoutAddress(int code, string idThreeDigit)
        {

            Order order = db.Orders.Include(c => c.City).Include(c => c.User).FirstOrDefault(c => c.Id == code);
            if (order == null)
            {
                return HttpNotFound();
            }

            if (order.CityId != null)
            {
                ViewBag.ProvinceTitle = db.Provinces.Find(order.City.ProvinceId)?.Title;
            }
            string id = order.Id.ToString().Substring(0, 3);

            if (id != idThreeDigit)
            {
                return HttpNotFound();
            }


            ViewBag.PaymentAmountStr = order.PaymentAmountStr;
            ViewBag.RemainAmountStr = order.RemainAmountStr;



            List<OrderDetail> orderDetails = db.OrderDetails.Include(current => current.Product)
                .Where(current => current.OrderId == order.Id && current.IsDeleted == false).ToList();

            OrderDetailViewModel orderDetailViewModel = new OrderDetailViewModel()
            {
                Order = order,
                OrderDetails = orderDetails
            };

            return View(orderDetailViewModel);
        }

        public ActionResult ChangeAddress(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.Title = "تغییر آدرس سفارش کد " + order.Id;
            ChangeAddressViewModel orderAddress = new ChangeAddressViewModel()
            {
                Id = order.Id,
                Address = order.Address,
                PostalCode = order.PostalCode,

            };
            ViewBag.CityId = new SelectList(db.Cities.Where(c => c.ProvinceId == order.City.ProvinceId).OrderBy(c => c.Title), "Id", "Title", order.CityId);
            ViewBag.ProvinceId = new SelectList(db.Provinces.OrderBy(c => c.Title), "Id", "Title", order.City.ProvinceId);
            return View(orderAddress);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult ChangeAddress(ChangeAddressViewModel orderAddress)
        {
            if (ModelState.IsValid)
            {
                Order order = db.Orders.Find(orderAddress.Id);

                order.Address = orderAddress.Address;
                order.CityId = orderAddress.CityId;
                order.PostalCode = orderAddress.PostalCode;
                order.LastModifiedDate = DateTime.Now;

                order.Description = "تغییر آدرس توسط ادمین";

                //User user = db.Users.Find(order.UserId);
                //City city = db.Cities.Find(orderAddress.CityId);
                //if (user != null)
                //{
                //    user.Address = orderAddress.Address;
                //    user.CityId = orderAddress.CityId;

                //    if (city != null)
                //        user.ProvinceId = city.ProvinceId;

                //    user.PostalCode = orderAddress.PostalCode;
                //    user.LastModifiedDate = DateTime.Now;
                //}
                db.SaveChanges();
                return RedirectToAction("Details", new { id = orderAddress.Id });
            }
            ViewBag.CityId = new SelectList(db.Cities.Where(c => c.ProvinceId == orderAddress.ProvinceId).OrderBy(c => c.Title), "Id", "Title", orderAddress.CityId);
            ViewBag.ProvinceId = new SelectList(db.Provinces.OrderBy(c => c.Title), "Id", "Title", orderAddress.ProvinceId);

            return View(orderAddress);
        }

        public ActionResult ManualOrder()
        {
            ManualOrderViewModel result = new ManualOrderViewModel()
            {

                Products = new SelectList(db.Products.Where(c => c.IsDeleted == false).ToList(), "Id", "Title"),
                Cities = new SelectList(db.Cities.OrderBy(c => c.Title).ToList(), "Id", "Title")
            };

            return View(result);
        }
        [HttpPost]
        public ActionResult ManualOrder(ManualOrderViewModel input)
        {
            User user = db.Users.FirstOrDefault(c =>
                c.CellNum == input.PhoneNumber && c.IsDeleted == false && c.SecurityRole == SecurityRole.Customer);

            if (user == null)
            {
                user = new User()
                {
                    IsDeleted = false,
                    IsActive = true,
                    CreationDate = DateTime.Now,
                    CellNum = input.PhoneNumber,
                    FullName = input.FullName,
                    SecurityRole = SecurityRole.Customer,
                    Password = "123456",
                };

                db.Users.Add(user);
            }

            Order order = CreateOrder(GetProductInBasket(input.SelectedProduct), input.ShipmentAmount, input.DiscountCodeId);
            order.Address = input.Address;
            order.PostalCode = input.PostalCode;
            order.CityId = (input.SelectedCity);
            order.DeliverFullName = input.FullName;
            order.DeliverCellNumber = input.PhoneNumber;
            order.IsPaid = true;
            order.OrderStatus = OrderStatus.InProgress;
            order.SaleReferenceId = "manual";
            order.PaymentDate = DateTime.Now;
            order.OrderType = input.OrderType;
            order.UserId = user.Id;
            order.PaymentTypeId = 2;
            db.SaveChanges();
            ManualOrderViewModel result = new ManualOrderViewModel()
            {

                Products = new SelectList(db.Products.Where(c => c.IsDeleted == false).ToList(), "Id", "Title"),
                Cities = new SelectList(db.Cities.OrderBy(c => c.Title).ToList(), "Id", "Title")
            };

            return View(result);
        }

        public Order CreateOrder(List<ProductInCart> products, decimal shipmentAmount, long? discountCodeId)
        {
            decimal discountAmount = 0;
            decimal subtotal = GetSubtotal(products);
            if (discountCodeId != null)
            {
                DiscountCode discount = db.DiscountCodes.FirstOrDefault(c => c.Id == discountCodeId.Value);

                DiscountHelper helper = new DiscountHelper();
                discountAmount = helper.CalculateDiscountAmount(discount, subtotal);
            }

            Order order = new Order();

            order.IsActive = true;
            order.IsDeleted = false;
            order.IsPaid = false;
            order.CreationDate = DateTime.Now;
            order.LastModifiedDate = DateTime.Now;
            order.OrderStatus = OrderStatus.WaitForPayment;
            order.SubTotal = subtotal;
            order.DiscountAmount = discountAmount;
            order.DiscountCodeId = discountCodeId;
            order.ShippingAmount = shipmentAmount;

            order.TotalAmount = Convert.ToDecimal(order.SubTotal + order.ShippingAmount - order.DiscountAmount);
            order.PaymentAmount = order.TotalAmount;


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

            return order;
        }

        public List<ProductInCart> GetProductInBasket(string inputProducts)
        {
            List<ProductInCart> productInCarts = new List<ProductInCart>();

            string[] basketItems = inputProducts.Split('-');


            for (int i = 0; i < basketItems.Length; i++)
            {
                if (!string.IsNullOrEmpty(basketItems[i]))
                {
                    if (basketItems[i] != "0")
                    {
                        int productCode = Convert.ToInt32(basketItems[i]);

                        Product product =
                            db.Products.FirstOrDefault(current =>
                                current.IsDeleted == false && current.Id == productCode);
                        if (product != null)
                        {
                            var currentProduct = productInCarts.FirstOrDefault(c => c.Product.Id == product.Id);

                            if (currentProduct != null)
                            {
                                currentProduct.Quantity = currentProduct.Quantity + 1;
                            }
                            else
                            {
                                productInCarts.Add(new ProductInCart()
                                {
                                    Product = product,
                                    Quantity = 1,

                                });
                            }

                        }
                    }

                }

            }


            return productInCarts;
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


        [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
        public async Task<ActionResult> NoPaymentList()
        {


            //  bool isPos = orderType != null;

            DateTime yesterday = DateTime.Now.AddDays(-1);


            //var orders = await db.Orders.AsNoTracking().Where(o =>
            //        (o.OrderStatus == OrderStatus.WaitForPayment) &&
            //        o.IsDeleted == false && o.CreationDate< yesterday)
            //    .OrderByDescending(o => o.CreationDate)
            //    .Include(o => o.User).ToListAsync();

            var orders = await db.Orders
                .AsNoTracking()
                .Where(o =>
                    o.OrderStatus == OrderStatus.WaitForPayment &&
                    o.IsDeleted == false &&
                    o.CreationDate < yesterday)
                .OrderByDescending(o => o.CreationDate)
                .Include(o => o.User)
                .GroupBy(o => o.User.CellNum) // گروه‌بندی بر اساس شماره تلفن
                .Select(g => g.FirstOrDefault()) // انتخاب اولین رکورد از هر گروه
                .OrderByDescending(c => c.CreationDate).ToListAsync();

            foreach (var order in orders.ToList())
            {
                if (db.Orders.Any(c => c.IsPaid && c.UserId == order.UserId))
                    orders.Remove(order);
            }
            ViewBag.CurrentOrder = "سفارشات در انتظار پرداخت";
            return View(orders.ToList());
        }


        [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
        public async Task<ActionResult> PrintOrderList()
        {
            List<OrderPrintViewModel> result = new List<OrderPrintViewModel>();
            var orders = db.Orders.Where(c => c.OrderStatus == OrderStatus.InProgress && c.IsDeleted == false);


            foreach (var order in orders)
            {


                if (order != null)
                {
                    if (order.CityId != null)
                    {
                        result.Add(new OrderPrintViewModel()
                        {
                            Address = order.City.Title + " - " + order.Address,
                            FullName = order.DeliverFullName,
                            PhoneNumber = order.DeliverCellNumber,
                            PostalCode = order.PostalCode
                        });
                    }
                }

            }

            return View(result);
        }

    }
}
