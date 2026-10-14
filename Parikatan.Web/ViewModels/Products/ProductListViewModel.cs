
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;
using Models.Entities;

namespace ViewModels.Products
{
    public class ProductListViewModel : _BaseViewModel
    {
        public List<Product> Products { get; set; }
        public int TotalProductCount { get; set; }
    }   
 
}