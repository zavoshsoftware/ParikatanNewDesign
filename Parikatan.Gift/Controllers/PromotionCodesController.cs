using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Parikatan.Gift.Enums;
using Parikatan.Gift.Models;
using Parikatan.Gift.Models.Entities.Promotion;
using Parikatan.Gift.ViewModel;

namespace Parikatan.Gift.Controllers
{
    public class PromotionCodesController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: PromotionCodes
        public ActionResult Index()
        {
            var promotionCodes = db.PromotionCodes.Include(p => p.User).Where(p=>p.IsDeleted==false).OrderByDescending(p=>p.CreationDate);
            return View(promotionCodes.ToList());
        }

        // GET: PromotionCodes/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PromotionCode promotionCode = db.PromotionCodes.Find(id);
            if (promotionCode == null)
            {
                return HttpNotFound();
            }
            return View(promotionCode);
        }

        // GET: PromotionCodes/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.Users, "Id", "FullName");
            return View();
        }

        // POST: PromotionCodes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( PromotionCode promotionCode)
        {
            if (ModelState.IsValid)
            {
				promotionCode.IsDeleted=false;
				promotionCode.CreationDate= DateTime.Now; 
					
                db.PromotionCodes.Add(promotionCode);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.Users, "Id", "FullName", promotionCode.UserId);
            return View(promotionCode);
        }

        // GET: PromotionCodes/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PromotionCode promotionCode = db.PromotionCodes.Find(id);
            if (promotionCode == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users.Where(c=>c.SecurityRole==SecurityRole.User), "Id", "FullName", promotionCode.UserId);
            return View(promotionCode);
        }

        // POST: PromotionCodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Code,ExpireDate,Amount,IsUsed,Reciever,UserFullName,UseDate,UserPhoneNumber,UserService,UserId,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] PromotionCode promotionCode)
        {
            if (ModelState.IsValid)
            {
				promotionCode.IsDeleted=false;
					promotionCode.LastModifiedDate=DateTime.Now;
                db.Entry(promotionCode).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "FullName", promotionCode.UserId);
            return View(promotionCode);
        }

        // GET: PromotionCodes/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PromotionCode promotionCode = db.PromotionCodes.Find(id);
            if (promotionCode == null)
            {
                return HttpNotFound();
            }
            return View(promotionCode);
        }

        // POST: PromotionCodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            PromotionCode promotionCode = db.PromotionCodes.Find(id);
			promotionCode.IsDeleted=true;
			promotionCode.DeletionDate=DateTime.Now;
 
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

        public ActionResult Use()
        {
            
            return View();
        }

        // POST: PromotionCodes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Use(UsePromotionViewModel result)
        {
            if (ModelState.IsValid)
            {
                var promotionCode = db.PromotionCodes.Where(c =>
                        c.Code == result.PromotionCode && c.IsDeleted == false && c.IsActive && c.IsUsed == false)
                    .FirstOrDefault();

                if (promotionCode != null)
                {
                    TempData["success"] = "true";

                    promotionCode.IsUsed = true;
                    promotionCode.UseDate=DateTime.Now;
                    promotionCode.UserFullName = result.UserFullName;
                    promotionCode.UserPhoneNumber = result.UserPhoneNumber;
                    promotionCode.UserService = result.UserService;

                    promotionCode.LastModifiedDate=DateTime.Now;
                    
                    db.SaveChanges();

                }
                else
                {
                    TempData["danger"] = "true";

                }
                return View(result);
            }

            return View(result);
        }

    }
}
