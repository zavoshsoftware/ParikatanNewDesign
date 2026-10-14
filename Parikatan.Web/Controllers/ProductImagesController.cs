using System;
using System.Collections.Generic;
using System.Data;
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
    public class ProductImagesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: ProductImages
        public ActionResult Index(long productId)
        {
            var productImages = db.ProductImages.Include(p => p.Product).Where(p => p.ProductId == productId &&
                p.IsDeleted == false).OrderByDescending(p => p.CreationDate);
            return View(productImages.ToList());
        }
         
        public ActionResult Create(long productId)
        {
            ViewBag.ProductId =productId;
            return View();
        }
 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( ProductImage productImage, HttpPostedFileBase fileupload, long productId)
        {
            if (ModelState.IsValid)
            {
                string newFilenameUrl = string.Empty;
                if (fileupload != null)
                {
                    string filename = Path.GetFileName(fileupload.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    newFilenameUrl = "/Uploads/Product/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                    productImage.ImageUrl = newFilenameUrl;
                }

                productImage.IsDeleted = false;
                productImage.CreationDate = DateTime.Now;
                productImage.ProductId = productId;

                db.ProductImages.Add(productImage);
                db.SaveChanges();
                return RedirectToAction("Index", new { productId = productImage.ProductId });
            }

            ViewBag.ProductId =  productImage.ProductId;
            return View(productImage);
        }

        // GET: ProductImages/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductImage productImage = db.ProductImages.Find(id);
            if (productImage == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = productImage.ProductId;
            return View(productImage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( ProductImage productImage,HttpPostedFileBase fileupload)
        {
            if (ModelState.IsValid)
            {
                string newFilenameUrl = string.Empty;
                if (fileupload != null)
                {
                    string filename = Path.GetFileName(fileupload.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    newFilenameUrl = "/Uploads/Product/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                    productImage.ImageUrl = newFilenameUrl;
                }

                productImage.IsDeleted = false;
                productImage.LastModifiedDate = DateTime.Now;
                db.Entry(productImage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index",new{productId=productImage.ProductId});
            }
            ViewBag.ProductId =  productImage.ProductId;
            return View(productImage);
        }

        // GET: ProductImages/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductImage productImage = db.ProductImages.Find(id);
            if (productImage == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId =  productImage.ProductId;
            return View(productImage);
        }

        // POST: ProductImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ProductImage productImage = db.ProductImages.Find(id);
            productImage.IsDeleted = true;
            productImage.DeletionDate = DateTime.Now;

            db.SaveChanges();
            return RedirectToAction("Index", new { productId = productImage.ProductId });
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
