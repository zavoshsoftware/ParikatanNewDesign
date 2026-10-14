using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Helpers;
using Models;
using Models.Entities;

namespace Parikatan.Web.Controllers
{
    [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
    public class SiteMapGeneratorController : Controller
    {
        private DatabaseContext db = new DatabaseContext();
        private string baseUrl = System.Configuration.ConfigurationManager.AppSettings["baseUrl"];


        [Route("seo/sitemap")]
        public ActionResult Sitemap(string language)
        {
            Sitemap sm = new Sitemap();

            StaticPageSiteMap(sm);

            ProductsSiteMap(sm);
            return new XmlResult(sm);
        }



        public void ProductsSiteMap(Sitemap sm)
        {
            List<Product> products = db.Products.Where(current => current.IsDeleted == false && current.IsActive).ToList();

            foreach (Product product in products)
            {
                var encoded = HttpUtility.UrlPathEncode(baseUrl + "/product-detail/" + product.Id);
                AddToSiteMap(sm, encoded, 0.7D, Location.eChangeFrequency.monthly);
            }
        }
      

        public void StaticPageSiteMap(Sitemap sm)
        {
            AddToSiteMap(sm, baseUrl, 0.9D, Location.eChangeFrequency.weekly);

            //AddToSiteMap(sm, baseUrl+"/about", 0.5D, Location.eChangeFrequency.yearly);

            AddToSiteMap(sm, baseUrl+"/contact", 0.5D, Location.eChangeFrequency.yearly);
        }

        public void AddToSiteMap(Sitemap sm, string url, double? priority, Location.eChangeFrequency frequency)
        {
            sm.Add(new Location()
            {
                Url = url,
                LastModified = DateTime.UtcNow,
                Priority = priority,
                ChangeFrequency = frequency
            });
        }
    }
}