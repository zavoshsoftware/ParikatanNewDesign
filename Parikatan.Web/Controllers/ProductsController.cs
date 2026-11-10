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
            FillCategories(null);
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

            FillCategories(product.ProductCategoryId);
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
            FillCategories(product.ProductCategoryId);
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
     
            FillCategories(product.ProductCategoryId);
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

        private void FillCategories(long? selected)
        {
            var categories = db.ProductCategories.Where(c => c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).ToList();
            ViewBag.Categories = new SelectList(categories, "Id", "Title", selected);
        }

        private readonly int _productPagination = Convert.ToInt32(WebConfigurationManager.AppSettings["productPaginationSize"]);
        private readonly string _baseUrl = WebConfigurationManager.AppSettings["baseUrl"];


        [AllowAnonymous]
        [Route("product-detail/{code:int}")]
        public ActionResult Details(int code)
        {
            Product product = db.Products.Include(c => c.ProductCategory)
                .FirstOrDefault(c => c.Id == code && c.IsDeleted == false);

            if (product == null)
            {
                return HttpNotFound();
            }

            ProductDetailViewModel result = new ProductDetailViewModel();

            result.Faqs = db.ProductFaqs
                .Where(f => f.ProductId == product.Id && f.IsDeleted == false && f.IsActive)
                .OrderBy(f => f.Order).ToList();

            var childIds = db.ProductPackageItemss
                .Where(i => i.ProductId == product.Id && i.IsDeleted == false)
                .Select(i => i.ChildId).ToList();
            result.PackageProducts = childIds.Count == 0
                ? new List<Product>()
                : db.Products.Where(p => childIds.Contains(p.Id) && p.IsDeleted == false).ToList();

            result.Product = product;

            result.ProductComments = db.ProductComments
                .Where(c => c.ProductId == product.Id && c.IsDeleted == false && c.IsActive)
                .OrderByDescending(c => c.CreationDate).ToList();

            result.ProductImages = db.ProductImages
                .Where(c => c.ProductId == product.Id && c.IsDeleted == false && c.IsActive).OrderBy(c => c.Order).ToList();

            // related: same category first, then the rest, ordered like the shop list
            result.RelatedProducts = db.Products.Include(c => c.ProductCategory)
                .Where(c => c.IsDeleted == false && c.IsActive && c.Id != product.Id)
                .OrderByDescending(c => c.ProductCategoryId == product.ProductCategoryId && product.ProductCategoryId != null)
                .ThenByDescending(c => c.IsAvailable).ThenByDescending(c => c.Order)
                .Take(4).ToList();
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

            ViewBag.Description = (product.PageDescription ?? product.Summery ?? string.Empty).Trim();
            ViewBag.NavActive = "products";
            return View(result);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("product-detail/{code:int}/comment")]
        public ActionResult AddComment(int code, string name, string message)
        {
            name = (name ?? string.Empty).Trim();
            message = (message ?? string.Empty).Trim();

            if (name.Length == 0 || name.Length > 200)
                return Json(new { ok = false, message = "لطفاً نام خود را وارد کنید." });
            if (message.Length < 10 || message.Length > 1000)
                return Json(new { ok = false, message = "متن نظر باید بین ۱۰ تا ۱۰۰۰ نویسه باشد." });

            if (!db.Products.Any(p => p.Id == code && p.IsDeleted == false))
                return Json(new { ok = false, message = "محصول یافت نشد." });

            db.ProductComments.Add(new ProductComment
            {
                ProductId = code,
                Name = name,
                Message = message,
                IsActive = false,          // shown after an admin approves it
                IsDeleted = false,
                CreationDate = DateTime.Now
            });
            db.SaveChanges();

            return Json(new { ok = true, message = "نظر شما ثبت شد و پس از بررسی نمایش داده می‌شود." });
        }

        [AllowAnonymous]
        [Route("product")]
        public ActionResult List(string category = null)
        {

            ProductListViewModel result = new ProductListViewModel();

            // filtering by category happens in the browser; ?category= only preselects a chip
            result.Products = db.Products.Include(c => c.ProductCategory)
                .Where(c => c.IsDeleted == false && c.IsActive)
                .OrderByDescending(c => c.IsAvailable).ThenByDescending(c => c.Order).ToList();

            result.Categories = result.Products
                .Where(p => p.ProductCategory != null && p.ProductCategory.IsActive && !p.ProductCategory.IsDeleted)
                .Select(p => p.ProductCategory).Distinct()
                .OrderBy(c => c.Order).ToList();

            result.SelectedCategory = string.IsNullOrWhiteSpace(category) ? null : category;
            result.TotalProductCount = result.Products.Count;

           

            ViewBag.Canonical = "https://parikatan.shop/product/";
         
          
                ViewBag.Title ="خرید محصولات پریکاتن";
            ViewBag.Description = "فروشگاه رسمی پریکاتن: کرم ضد چروک، کرم دور چشم، سرم روشن‌کننده و سایر محصولات با فرمولاسیون علمی. فیلتر بر اساس نیاز پوست و خرید آنلاین.";
            ViewBag.NavActive = "products";
        
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
