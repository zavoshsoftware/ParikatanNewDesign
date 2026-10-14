using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Models;
using Models.Entities;

namespace Parikatan.Web.Controllers
{
    [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
    public class ProductPackageItemsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: ProductPackageItems
        public ActionResult Index()
        {
            var productPackageItemss = db.ProductPackageItemss.Include(p => p.Product).Where(p=>p.IsDeleted==false).OrderByDescending(p=>p.CreationDate);
            return View(productPackageItemss.ToList());
        }

        // GET: ProductPackageItems/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductPackageItem productPackageItem = db.ProductPackageItemss.Find(id);
            if (productPackageItem == null)
            {
                return HttpNotFound();
            }
            return View(productPackageItem);
        }

        // GET: ProductPackageItems/Create
        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Title");
            return View();
        }

        // POST: ProductPackageItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProductId,ChildId,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] ProductPackageItem productPackageItem)
        {
            if (ModelState.IsValid)
            {
				productPackageItem.IsDeleted=false;
				productPackageItem.CreationDate= DateTime.Now; 
					
                db.ProductPackageItemss.Add(productPackageItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(db.Products, "Id", "Title", productPackageItem.ProductId);
            return View(productPackageItem);
        }

        // GET: ProductPackageItems/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductPackageItem productPackageItem = db.ProductPackageItemss.Find(id);
            if (productPackageItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Title", productPackageItem.ProductId);
            return View(productPackageItem);
        }

        // POST: ProductPackageItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProductId,ChildId,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] ProductPackageItem productPackageItem)
        {
            if (ModelState.IsValid)
            {
				productPackageItem.IsDeleted=false;
					productPackageItem.LastModifiedDate=DateTime.Now;
                db.Entry(productPackageItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Title", productPackageItem.ProductId);
            return View(productPackageItem);
        }

        // GET: ProductPackageItems/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductPackageItem productPackageItem = db.ProductPackageItemss.Find(id);
            if (productPackageItem == null)
            {
                return HttpNotFound();
            }
            return View(productPackageItem);
        }

        // POST: ProductPackageItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ProductPackageItem productPackageItem = db.ProductPackageItemss.Find(id);
			productPackageItem.IsDeleted=true;
			productPackageItem.DeletionDate=DateTime.Now;
 
            db.SaveChanges();
            return RedirectToAction("Index");
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
