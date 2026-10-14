using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Models;
using Models.Entities;
using ViewModels.Products;

namespace Parikatan.Web.Controllers
{
    public class ProductsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

           [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
        public ActionResult Index(long? parentId)
        {
            var products = db.Products.Where(p => p.IsDeleted == false).OrderByDescending(p => p.CreationDate);
            return View(products.ToList());
        }

      

           [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
        public ActionResult Create(long? parentId)
        {
            return View();
        }

           [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product,HttpPostedFileBase fileupload, HttpPostedFileBase fileUploadSecond)
        {
           

            if (ModelState.IsValid)
            {
                #region Upload and resize image if needed
                string newFilenameUrl = string.Empty;
                if (fileupload != null)
                {
                    string filename = Path.GetFileName(fileupload.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    newFilenameUrl = "/Uploads/Product/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                    product.ImageUrl = newFilenameUrl;
                }

                if (fileUploadSecond != null)
                {
                    string filename = Path.GetFileName(fileUploadSecond.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    newFilenameUrl = "/Uploads/Product/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileUploadSecond.SaveAs(physicalFilename);
                    product.SecondImageUrl = newFilenameUrl;
                }
                #endregion

                product.IsDeleted = false;
                product.CreationDate = DateTime.Now;
                product.Visit = 0;
               
                db.Products.Add(product);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(product);
        }

           [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            //ViewBag.Tags = GetTagCheckboxList(null);
            return View(product);
        }

           [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase fileupload, HttpPostedFileBase fileUploadSecond, List<TagCheckboxList> Tags)
        {
            if (ModelState.IsValid)
            {
                #region Upload and resize image if needed
                string newFilenameUrl = string.Empty;
                if (fileupload != null)
                {
                    string filename = Path.GetFileName(fileupload.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    newFilenameUrl = "/Uploads/Product/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                    product.ImageUrl = newFilenameUrl;
                }

                if (fileUploadSecond != null)
                {
                    string filename = Path.GetFileName(fileUploadSecond.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    newFilenameUrl = "/Uploads/Product/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileUploadSecond.SaveAs(physicalFilename);
                    product.SecondImageUrl = newFilenameUrl;
                }
                #endregion

                 
                product.IsDeleted = false;
                product.LastModifiedDate = DateTime.Now;
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                db.SaveChanges();
                return RedirectToAction("Index" );
            }
     
            return View(product);
        }

           [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

           [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Product product = db.Products.Find(id);
            product.IsDeleted = true;
            product.DeletionDate = DateTime.Now;

            db.SaveChanges();
            return RedirectToAction("Index" );
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private readonly int _productPagination = Convert.ToInt32(WebConfigurationManager.AppSettings["productPaginationSize"]);
        private readonly string _baseUrl = WebConfigurationManager.AppSettings["baseUrl"];


        [AllowAnonymous]
        [Route("product-detail/{code:int}")]
        public ActionResult Details(int code)
        {
            Product product = db.Products.FirstOrDefault(c => c.Id == code && c.IsDeleted == false);

            if (product == null)
            {
                return HttpNotFound();
            }

            //ViewBag.isSizable = 0;
            //if (product.ProductGroup.IsSizable)
            //    ViewBag.isSizable = 1;


            ProductDetailViewModel result = new ProductDetailViewModel();

            result.Product = product;

            result.ProductComments = db.ProductComments
                .Where(c => c.ProductId == product.Id && c.IsDeleted == false && c.IsActive)
                .OrderByDescending(c => c.CreationDate).ToList();

            result.ProductImages = db.ProductImages
                .Where(c => c.ProductId == product.Id && c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).ToList();

            result.RelatedProducts = db.Products.Where(c => c.IsDeleted == false && c.IsActive && c.Id != product.Id)
                .Take(3).ToList();
            //{
            //    Product = product,
            //    ProductComments = db.ProductComments.Where(c => c.ProductId == product.Id && c.IsDeleted == false && c.IsActive).OrderByDescending(c => c.CreationDate).ToList(),
            //    ProductImages = db.ProductImages.Where(c => c.ProductId == product.Id && c.IsDeleted == false && c.IsActive).ToList(),
            //    ProductFeatures = db.ProductFeatures.Where(c => c.ProductId == product.Id && c.IsDeleted == false && c.IsActive).ToList(),
            //    RelatedProducts = db.Products.Where(c => c.ProductGroupId == product.ProductGroupId &&
            //    c.Id != product.Id && c.ParentId == null && c.IsDeleted == false && c.IsActive).OrderByDescending(x => x.Stock).Take(10).ToList(),
            //    Colors = GetProductColors(childProducts),
            //    Sizes = GetProductSizes(childProducts)
            //};

            ViewBag.Canonical = "https://parikatan.shop/product-detail/" + product.Id;
         
            if (!string.IsNullOrEmpty(product.PageTitle))
                ViewBag.Title = product.PageTitle;
            else
                ViewBag.Title = product.Title;
            return View(result);
        }

        [AllowAnonymous]
        [Route("product")]
        public ActionResult List()
        {
       
            ProductListViewModel result = new ProductListViewModel();

            result.Products = db.Products.Where(c=>c.IsDeleted==false&&c.IsActive).OrderByDescending(c=>c.IsAvailable).ThenByDescending(c=>c.Order).ToList();

           

            ViewBag.Canonical = "https://parikatan.shop/product/";
         
          
                ViewBag.Title ="خرید محصولات پریکاتن";
        
            return View(result);
        }

        [AllowAnonymous]
        [Route("product-detail-ads/{code:int}")]
        public ActionResult DetailsAds(int code)
        {
            Product product = db.Products.FirstOrDefault(c => c.Id == code && c.IsDeleted == false);

            if (product == null)
            {
                return HttpNotFound();
            }

            //ViewBag.isSizable = 0;
            //if (product.ProductGroup.IsSizable)
            //    ViewBag.isSizable = 1;


            ProductDetailViewModel result = new ProductDetailViewModel();

            result.Product = product;

            result.ProductComments = db.ProductComments
                .Where(c => c.ProductId == product.Id && c.IsDeleted == false && c.IsActive)
                .OrderByDescending(c => c.CreationDate).ToList();

            result.ProductImages = db.ProductImages
                .Where(c => c.ProductId == product.Id && c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).ToList();


            //{
            //    Product = product,
            //    ProductComments = db.ProductComments.Where(c => c.ProductId == product.Id && c.IsDeleted == false && c.IsActive).OrderByDescending(c => c.CreationDate).ToList(),
            //    ProductImages = db.ProductImages.Where(c => c.ProductId == product.Id && c.IsDeleted == false && c.IsActive).ToList(),
            //    ProductFeatures = db.ProductFeatures.Where(c => c.ProductId == product.Id && c.IsDeleted == false && c.IsActive).ToList(),
            //    RelatedProducts = db.Products.Where(c => c.ProductGroupId == product.ProductGroupId &&
            //    c.Id != product.Id && c.ParentId == null && c.IsDeleted == false && c.IsActive).OrderByDescending(x => x.Stock).Take(10).ToList(),
            //    Colors = GetProductColors(childProducts),
            //    Sizes = GetProductSizes(childProducts)
            //};

            ViewBag.Canonical = "https://parikatan.shop/product-detail/" + product.Id;
         
            if (!string.IsNullOrEmpty(product.PageTitle))
                ViewBag.Title = product.PageTitle;
            else
                ViewBag.Title = product.Title;
            return View(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult NewsLetterPhoneNumberPost(string phoneNumber)
        {
            try
            {

                NewsLetter newsLetter = new NewsLetter()
                {
                    IsDeleted = false,
                    CreationDate = DateTime.Now,
                    PhoneNumber = phoneNumber
                };

                db.NewsLetters.Add(newsLetter);
                db.SaveChanges();

                return Json("true", JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json("false", JsonRequestBehavior.AllowGet);
            }
        }


    }
}
