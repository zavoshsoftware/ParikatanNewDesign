using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Core.Enums;
using Models.Entities;

namespace Parikatan.Web.ViewModels.Orders
{
    public class ManualOrderViewModel
    {
        //public List<City> Cities { get; set; }
        public string SelectedProduct { get; set; }
        public long SelectedCity { get; set; }
        public string PostalCode { get; set; }
        public string Address { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public long ProductId { get; set; }
        public SelectList Products { get; set; }
        public SelectList Cities { get; set; }
        public OrderType OrderType { get; set; }
        public decimal ShipmentAmount { get; set; }
        public long? DiscountCodeId { get; set; }
    }
}