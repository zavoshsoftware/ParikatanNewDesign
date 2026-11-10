using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Models;
using Models.Entities;

namespace Parikatan.Web.Controllers
{
    public class PharmaciesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // ---------------- public page ----------------

        [AllowAnonymous]
        [Route("pharmacies")]
        public ActionResult Index()
        {
            var list = db.Pharmacies.Where(p => p.IsDeleted == false && p.IsActive)
                .OrderBy(p => p.RegionOrder).ThenBy(p => p.Order).ThenBy(p => p.Title).ToList();

            ViewBag.Title = "داروخانه‌های عرضه‌کننده‌ی پریکاتن";
            ViewBag.Description = "فهرست داروخانه‌ها و فروشگاه‌هایی که محصولات پریکاتن را در شهرهای مختلف عرضه می‌کنند.";
            ViewBag.Canonical = "https://parikatan.shop/pharmacies";
            ViewBag.NavActive = "pharmacies";
            return View(list);
        }

        // ---------------- admin ----------------

        [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
        public ActionResult Manage()
        {
            return View(db.Pharmacies.Where(p => p.IsDeleted == false)
                .OrderBy(p => p.RegionOrder).ThenBy(p => p.Order).ToList());
        }

        [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
        public ActionResult Create()
        {
            int next = db.Pharmacies.Any() ? db.Pharmacies.Max(p => p.Order) + 1 : 1;
            return View(new Pharmacy { IsActive = true, Kind = "pharmacy", Order = next });
        }

        [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Pharmacy pharmacy)
        {
            if (ModelState.IsValid)
            {
                Normalize(pharmacy);
                pharmacy.IsDeleted = false;
                pharmacy.CreationDate = DateTime.Now;
                db.Pharmacies.Add(pharmacy);
                db.SaveChanges();
                return RedirectToAction("Manage");
            }
            return View(pharmacy);
        }

        [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
        public ActionResult Edit(long? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Pharmacy pharmacy = db.Pharmacies.Find(id);
            if (pharmacy == null)
                return HttpNotFound();
            return View(pharmacy);
        }

        [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Pharmacy pharmacy)
        {
            if (ModelState.IsValid)
            {
                Normalize(pharmacy);
                pharmacy.IsDeleted = false;
                pharmacy.LastModifiedDate = DateTime.Now;
                db.Entry(pharmacy).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Manage");
            }
            return View(pharmacy);
        }

        [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
        public ActionResult Delete(long? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Pharmacy pharmacy = db.Pharmacies.Find(id);
            if (pharmacy == null)
                return HttpNotFound();
            return View(pharmacy);
        }

        [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Pharmacy pharmacy = db.Pharmacies.Find(id);
            pharmacy.IsDeleted = true;
            pharmacy.DeletionDate = DateTime.Now;
            db.SaveChanges();
            return RedirectToAction("Manage");
        }

        /// <summary>Rows of the same city share one RegionOrder, so a new row takes it from an existing row.</summary>
        private void Normalize(Pharmacy p)
        {
            p.Region = (p.Region ?? string.Empty).Trim();
            if (string.IsNullOrEmpty(p.Hours))
                p.Hours = null;
            if (p.Kind != "store")
                p.Kind = "pharmacy";

            var sameRegion = db.Pharmacies.FirstOrDefault(x => x.Region == p.Region && x.IsDeleted == false && x.Id != p.Id);
            if (sameRegion != null)
                p.RegionOrder = sameRegion.RegionOrder;
            else if (p.RegionOrder <= 0)
                p.RegionOrder = (db.Pharmacies.Any() ? db.Pharmacies.Max(x => x.RegionOrder) : 0) + 1;
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
