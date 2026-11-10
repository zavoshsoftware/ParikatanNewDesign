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

        /// <summary>Active categories that have at least one active product (filter chips).</summary>
        public List<ProductCategory> Categories { get; set; }

        /// <summary>UrlParam of the category preselected through ?category=, or null for "all".</summary>
        public string SelectedCategory { get; set; }
    }
}
