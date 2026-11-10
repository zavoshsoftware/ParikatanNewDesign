using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Models;
using Models.Entities;

namespace Parikatan.Web.Controllers
{
    [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
    public class ProductCategoriesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public ActionResult Index()
        {
            return View(db.ProductCategories.Where(a => a.IsDeleted == false).OrderBy(a => a.Order).ToList());
        }

        public ActionResult Create()
        {
            return View(new ProductCategory { IsActive = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductCategory category, HttpPostedFileBase fileupload)
        {
            if (ModelState.IsValid)
            {
                category.ImageUrl = SaveImage(fileupload) ?? category.ImageUrl;
                category.IsDeleted = false;
                category.CreationDate = DateTime.Now;

                db.ProductCategories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(category);
        }

        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategory category = db.ProductCategories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductCategory category, HttpPostedFileBase fileupload)
        {
            if (ModelState.IsValid)
            {
                category.ImageUrl = SaveImage(fileupload) ?? category.ImageUrl;
                category.IsDeleted = false;
                category.LastModifiedDate = DateTime.Now;
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductCategory category = db.ProductCategories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ProductCategory category = db.ProductCategories.Find(id);
            category.IsDeleted = true;
            category.DeletionDate = DateTime.Now;

            foreach (var product in db.Products.Where(p => p.ProductCategoryId == id))
            {
                product.ProductCategoryId = null;
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private string SaveImage(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
                return null;

            string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty) + Path.GetExtension(file.FileName);
            string url = "/Uploads/ProductCategory/" + newFilename;
            string physical = Server.MapPath(url);
            Directory.CreateDirectory(Path.GetDirectoryName(physical));
            file.SaveAs(physical);
            return url;
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
