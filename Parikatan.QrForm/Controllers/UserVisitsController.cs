using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Parikatan.QrForm.Models;
using Parikatan.QrForm.Models.Entities;

namespace Parikatan.QrForm.Controllers
{
   [Authorize(Roles = "superadmin")]
    public class UserVisitsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: UserVisits
        public ActionResult Index()
        {
            return View(db.UserVisits.Where(a=>a.IsDeleted==false).OrderByDescending(a=>a.CreationDate).ToList());
        }

   //     // GET: UserVisits/Details/5
   //     public ActionResult Details(long? id)
   //     {
   //         if (id == null)
   //         {
   //             return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
   //         }
   //         UserVisit userVisit = db.UserVisits.Find(id);
   //         if (userVisit == null)
   //         {
   //             return HttpNotFound();
   //         }
   //         return View(userVisit);
   //     }

   //     // GET: UserVisits/Create
   //     public ActionResult Create()
   //     {
   //         return View();
   //     }

   //     // POST: UserVisits/Create
   //     // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
   //     // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
   //     [HttpPost]
   //     [ValidateAntiForgeryToken]
   //     public ActionResult Create([Bind(Include = "Id,Ip,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] UserVisit userVisit)
   //     {
   //         if (ModelState.IsValid)
   //         {
			//	userVisit.IsDeleted=false;
			//	userVisit.CreationDate= DateTime.Now; 
					
   //             db.UserVisits.Add(userVisit);
   //             db.SaveChanges();
   //             return RedirectToAction("Index");
   //         }

   //         return View(userVisit);
   //     }

   //     // GET: UserVisits/Edit/5
   //     public ActionResult Edit(long? id)
   //     {
   //         if (id == null)
   //         {
   //             return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
   //         }
   //         UserVisit userVisit = db.UserVisits.Find(id);
   //         if (userVisit == null)
   //         {
   //             return HttpNotFound();
   //         }
   //         return View(userVisit);
   //     }

   //     // POST: UserVisits/Edit/5
   //     // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
   //     // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
   //     [HttpPost]
   //     [ValidateAntiForgeryToken]
   //     public ActionResult Edit([Bind(Include = "Id,Ip,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] UserVisit userVisit)
   //     {
   //         if (ModelState.IsValid)
   //         {
			//	userVisit.IsDeleted=false;
			//		userVisit.LastModifiedDate=DateTime.Now;
   //             db.Entry(userVisit).State = EntityState.Modified;
   //             db.SaveChanges();
   //             return RedirectToAction("Index");
   //         }
   //         return View(userVisit);
   //     }

   //     // GET: UserVisits/Delete/5
   //     public ActionResult Delete(long? id)
   //     {
   //         if (id == null)
   //         {
   //             return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
   //         }
   //         UserVisit userVisit = db.UserVisits.Find(id);
   //         if (userVisit == null)
   //         {
   //             return HttpNotFound();
   //         }
   //         return View(userVisit);
   //     }

   //     // POST: UserVisits/Delete/5
   //     [HttpPost, ActionName("Delete")]
   //     [ValidateAntiForgeryToken]
   //     public ActionResult DeleteConfirmed(long id)
   //     {
   //         UserVisit userVisit = db.UserVisits.Find(id);
			//userVisit.IsDeleted=true;
			//userVisit.DeletionDate=DateTime.Now;
 
   //         db.SaveChanges();
   //         return RedirectToAction("Index");
   //     }

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
