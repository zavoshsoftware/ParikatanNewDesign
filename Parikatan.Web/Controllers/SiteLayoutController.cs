using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Models;
using ViewModels.Site;

namespace Parikatan.Web.Controllers
{
    /// <summary>Child actions that feed the shared public-site layout (header menu and footer).</summary>
    public class SiteLayoutController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        private SiteMenuViewModel BuildMenu()
        {
            return new SiteMenuViewModel
            {
                Products = db.Products
                    .Include(p => p.ProductCategory)
                    .Where(p => p.IsDeleted == false && p.IsActive)
                    .OrderByDescending(p => p.IsAvailable)
                    .ThenByDescending(p => p.Order)
                    .Take(8)
                    .ToList()
            };
        }

        [ChildActionOnly]
        public ActionResult Header()
        {
            return PartialView(BuildMenu());
        }

        [ChildActionOnly]
        [OutputCache(Duration = 300)]
        public ActionResult Footer()
        {
            return PartialView(BuildMenu());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
