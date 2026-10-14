using System;
using System.Collections.Generic;
using Models;
using Models.Entities; 

namespace ViewModels.Orders
{
    public class OrderDetailViewModel : _BaseViewModel
    {
        public Order Order { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

     


    }

   
}