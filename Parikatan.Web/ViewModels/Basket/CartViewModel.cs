using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;
using Models.Entities;

namespace ViewModels.Basket
{
    public class CartViewModel : _BaseViewModel
    {
        public List<ProductInCart> Products { get; set; }
        public string SubTotal { get; set; }
        public string Total { get; set; }
        public string DiscountAmount { get; set; }
        public string ShipmentAmount { get; set; }
        public decimal SubTotalValue { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal ShipmentValue { get; set; }
        public decimal TotalValue { get; set; }
        public int ItemCount { get; set; }
        public List<Product> RelatedProducts { get; set; }
    }

    public class ProductInCart
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public string RowAmount
        {
            get
            {
                if (Product.IsInPromotion && Product.DiscountAmount != null)
                    return (Product.DiscountAmount.Value * Quantity).ToString("n0") + " تومان";

                return (Product.Amount * Quantity).ToString("n0") + " تومان";
            }
        }

   public string  Amount
        {
            get
            {
                if (Product.IsInPromotion && Product.DiscountAmount != null)
                    return (Product.DiscountAmount.Value).ToString("n0") + " تومان";

                return (Product.Amount).ToString("n0") + " تومان";
            }
        }


    }
}