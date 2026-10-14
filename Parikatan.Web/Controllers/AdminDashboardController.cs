
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Enums;
using Models;
using Models.Entities;
using ViewModels;

namespace Parikatan.Web.Controllers
{
    [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
    public class AdminDashboardController : Controller
    {

        private DatabaseContext db = new DatabaseContext();

        public ActionResult OrderList(string startDate, string endDate)
        {
            if (startDate == null)
                startDate = DateTime.Today.ToShortDateString();
            if (endDate == null)
                endDate = DateTime.Today.AddDays(1).ToShortDateString();

            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;

            //Guid canceledOrderId = new Guid("99C42AD7-AE29-46C5-BAE2-00D2DA31DDCD");
            //Guid pendingOrderId = new Guid("11869F4D-D6D1-434C-A2F1-7945227CD3BB");

            List<Order> orders = GetOrders(startDate, endDate)
                .ToList();

            //Guid statusOnprogress = new Guid("7DBF85F4-7835-4D21-8269-26695D0C7E0F");
            //Guid statusFinal = new Guid("D719D03E-3F85-4590-A451-B44EAD16C162");
            // db.Database.Log = Console.Write;
            var startdate = DateTime.Parse(startDate);
            var enddate = DateTime.Parse(endDate);

            var totalUsersCount = orders.Select(x => x.UserId).Distinct().Count();
            //    var newUsersCount = orders.Where(x => userIdsInPeriod.Contains((long)x.UserId) && x.IsDeleted==false).Count();



            DateTime startDateCost = Convert.ToDateTime(startDate);
            DateTime endDateCost = Convert.ToDateTime(endDate);




            var cost = db.Costs
                .Where(c =>c.IsDeleted==false&& c.PayDate >= startDateCost.Date && c.PayDate <= endDateCost.Date)
                .AsEnumerable()  // تبدیل کوئری به داده‌های قابل پردازش در حافظه
                .Sum(i => i.Amount);

            OrderData orderData = GetOrderData(orders);

            OrderDashboardViewModel result = new OrderDashboardViewModel()
            {
                TotalOrderQty = orders.Count,
                TotamOrderAmount = orders.Sum(c => c.TotalAmount),
                OrderByProvince = GetOrderByProvince(orders),
                TotalOnProgressOrderQty = orders.Count(c => c.OrderStatus == OrderStatus.InProgress),
                TotalFinalOrderQty = orders.Count(c => c.OrderStatus == OrderStatus.Sent || c.OrderStatus == OrderStatus.Delivered),
                TotalDiscountAmount = orderData.TotalDiscountAmount,
                TotalCustomersCount = totalUsersCount,
                TotalCosts = cost,
                TotalPurchaseAmount = orderData.TotalPurchaseAmount,
                OrderItems = orderData.OrderItems,
                TotalShipmentAmountIncome = orderData.TotalShipmentAmountIncome,
                TotalProductCount = orderData.TotalProductCount,
                TotalShipmentAmountPaid = db.Costs
                    .Where(c => c.IsDeleted == false && c.PayDate >= startDateCost.Date && c.PayDate <= endDateCost.Date
                    &&c.CostType==CostType.Shipment)
                    .AsEnumerable()  // تبدیل کوئری به داده‌های قابل پردازش در حافظه
                    .Sum(i => i.Amount)
        };

            return View(result);
        }

        //public List<OrderItem> GetOrderItems(List<Order> orders)
        //{
        //    List<OrderItem> result = new List<OrderItem>();

        //    foreach (var order in orders)
        //    {
        //        foreach (var orderDetail in order.OrderDetails)
        //        {
        //            OrderItem orderItem = result.FirstOrDefault(c => c.ProductId == orderDetail.ProductId);
        //            if(orderItem!=null)
        //            {
        //                orderItem.Quantity = orderItem.Quantity + orderDetail.Quantity;
        //            }
        //            else
        //            {
        //                result.Add(new OrderItem()
        //                {
        //                    Quantity = orderDetail.Quantity,
        //                    ProductId = orderDetail.ProductId,
        //                    ProductTitle = orderDetail.Product.Title
        //                });
        //            }
        //        }
        //    }

        //    return result.OrderByDescending(c=>c.Quantity).ToList();
        //}
        public OrderData GetOrderData(List<Order> orders)
        {

            OrderData result = new OrderData();
            List<OrderItem> orderItemResult = new List<OrderItem>();
            decimal discountResult = 0;
            decimal shipmentResult = 0;
            decimal purchaseResult = 0;

            foreach (var order in orders)
            {
                foreach (var orderDetail in order.OrderDetails)
                {
                    OrderItem orderItem = orderItemResult.FirstOrDefault(c => c.ProductId == orderDetail.ProductId);
                    if(orderItem!=null)
                    {
                        orderItem.Quantity = orderItem.Quantity + orderDetail.Quantity;
                    }
                    else
                    {
                        orderItemResult.Add(new OrderItem()
                        {
                            Quantity = orderDetail.Quantity,
                            ProductId = orderDetail.ProductId,
                            ProductTitle = orderDetail.Product.Title
                        });
                    }

                    if (orderDetail.PurchaseAmount != null)
                        purchaseResult = (decimal)(purchaseResult + (orderDetail.PurchaseRowAmount));
                }

                if (order.DiscountAmount != null)
                    discountResult = discountResult + order.DiscountAmount.Value;

                if (order.ShippingAmount != null)
                    shipmentResult = shipmentResult + order.ShippingAmount.Value;

               
            }


            result.OrderItems = orderItemResult.OrderByDescending(c => c.Quantity).ToList();
            result.TotalDiscountAmount = discountResult;
            result.TotalShipmentAmountIncome = shipmentResult;
            result.TotalPurchaseAmount = purchaseResult;
            result.TotalProductCount = orderItemResult.Sum(c => c.Quantity);

            return result;
        }



      
    
        public List<OrderByCityViewModel> GetOrderByProvince(List<Order> orders)
        {
            List<OrderByCityViewModel> result = new List<OrderByCityViewModel>();

            List<Province> provinces = db.Provinces.Where(c => c.IsDeleted == false).OrderBy(c => c.Title).ToList();

            foreach (var province in provinces)
            {

                result.Add(new OrderByCityViewModel()
                {
                    ProvinceId = province.Id,
                    ProvinceTitle = province.Title,
                    TotalOrderQtyByProvince = orders.Where(c => c.City.ProvinceId == province.Id).Count(),
                    TotamOrderAmountByProvince = orders.Where(c => c.City.ProvinceId == province.Id).Sum(c => c.TotalAmount),
                });
            }

            return result;
        }

        public List<Order> GetOrders(string start, string end)
        {
            DateTime startDate = Convert.ToDateTime(start);
            DateTime endDate = Convert.ToDateTime(end);

            //List<Order> orders = db.Orders.AsNoTracking()
            //    .Where(current => current.IsDeleted == false)
            //  .OrderByDescending(o => o.CreationDate).ToList();
            var orderlist = db.Orders.AsNoTracking().Where(current =>
                 current.IsDeleted == false && current.IsPaid &&
                 (current.CreationDate >= startDate.Date && current.CreationDate <= endDate.Date));

            return orderlist.ToList();
        }

        public List<Order> ReturnFilteredOrders(List<Order> orders, string start, string end)
        {
            List<Order> orderlist = new List<Order>();
            if (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end))
            {

                DateTime startDate = Convert.ToDateTime(start);
                DateTime endDate = Convert.ToDateTime(end);
                ViewBag.startDate = startDate.ToShortDateString();
                ViewBag.endDate = endDate.ToShortDateString();
                orderlist = orders.Where(current => (current.CreationDate >= startDate.Date && current.CreationDate <= endDate.Date)).ToList();

            }

            return orderlist;
        }

    }
}