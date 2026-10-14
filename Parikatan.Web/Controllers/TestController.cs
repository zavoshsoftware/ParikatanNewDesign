using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Models;

namespace Parikatan.Web.Controllers
{
    public class TestController : Controller
    {
        private DatabaseContext db = new DatabaseContext();
        [Route("list")]
        public JsonResult List(int page=1,int item_per_page=50)
        {
            var products = db.Products
                .Where(s => s.IsActive)
                .Select(s => new
                {
                    s.Id,
                    s.Title,
                })
                .ToList();

            return Json(new CustomResponseViewModel()
            {
                item_per_page = 10,
                page_num = 1,
                pages_count = 1,
                total_items = db.Products.Where(c => c.IsDeleted == false && c.IsActive).Count(),
                products = getProducts()
            }
             , JsonRequestBehavior.AllowGet);
        }

        public List<ProductApiViewModel> getProducts()
        {
            var products = db.Products.Where(c => c.IsDeleted == false && c.IsActive).ToList();
            List<ProductApiViewModel> result = new List<ProductApiViewModel>();
            foreach (var product in products)
            {

                result.Add(new ProductApiViewModel()
                {
                    category = "پریکاتن",
                    id = product.Id.ToString(),
                    title = product.Title,
                    price = (int)(product.Amount),
                    is_available = product.IsAvailable,
                    url = "https://parikatan.shop/product-detail/"+product.Id
                });

            }

            return result;
        }
    }
}