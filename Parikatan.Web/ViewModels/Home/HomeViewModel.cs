using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models.Entities;

namespace ViewModels.Home
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            Products = new List<Product>();
            Categories = new List<ProductCategory>();
            Ingredients = new List<IngredientItem>();
            Blogs = new List<Blog>();
        }

        /// <summary>Products shown in the best-sellers carousel.</summary>
        public List<Product> Products { get; set; }

        /// <summary>Categories for the quick-nav strip and (those flagged IsInHome) the tiles.</summary>
        public List<ProductCategory> Categories { get; set; }

        public List<IngredientItem> Ingredients { get; set; }

        public List<Blog> Blogs { get; set; }

        public int ActiveProductCount { get; set; }
    }

    public class IngredientItem
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
    }
}
