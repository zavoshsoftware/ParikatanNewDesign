using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class OrderDashboardViewModel
    {
        public int TotalOrderQty { get; set; }
        public decimal TotamOrderAmount { get; set; }
        public int TotalOnProgressOrderQty { get; set; }
        public int TotalFinalOrderQty { get; set; }
        public decimal TotalPurchaseAmount { get; set; }
        public decimal? TotalDiscountAmount { get; set; }
        public int NewCustomersCount { get; set; }
        public int OldCustomersCount { get; set; }
        public int TotalCustomersCount { get; set; }
        public List<OrderByCityViewModel> OrderByProvince { get; set; }
        public decimal TotalCosts { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public int TotalProductCount { get; set; }
        public decimal? TotalShipmentAmountIncome { get; set; }
        public decimal? TotalShipmentAmountPaid { get; set; }

    }

    public class OrderByCityViewModel
    {
        public long ProvinceId { get; set; }
        [Display(Name ="استان")]
        public string ProvinceTitle { get; set; }
        [Display(Name ="تعداد سفارشات ثبت شده")]
        public int TotalOrderQtyByProvince { get; set; }
        [Display(Name ="جمع ریالی سفارشات")]
        public decimal TotamOrderAmountByProvince { get; set; }
        [Display(Name ="جمع ریالی سفارشات")]
        public string TotamOrderAmountByProvinceStr { get { return TotamOrderAmountByProvince.ToString("n0"); } }
    }

    public class OrderData
    {
        public List<OrderItem> OrderItems { get; set; }
        public decimal TotalShipmentAmountIncome { get; set; }
        public decimal TotalShipmentAmountPaid { get; set; }
        public decimal? TotalDiscountAmount { get; set; }
        public decimal TotalPurchaseAmount { get; set; }
        public int TotalProductCount { get; set; }

    }
    public class OrderItem
    {
        public long ProductId { get; set; }
        public string ProductTitle { get; set; }
        public int Quantity { get; set; }
    }
}