using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;
using Models.Entities;

namespace ViewModels.Products
{
    public class ProductDetailViewModel : _BaseViewModel
    {
        public Product Product { get; set; }
        public List<Product> RelatedProducts { get; set; }
        public List<ProductImage> ProductImages { get; set; }

        public List<ProductComment> ProductComments { get; set; }

        public List<ProductFaq> Faqs { get; set; }

        /// <summary>Products contained in this package (resolved from ProductPackageItem.ChildId).</summary>
        public List<Product> PackageProducts { get; set; }
    }
}
