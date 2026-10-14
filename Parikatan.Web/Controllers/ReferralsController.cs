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

    public class ReferralsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: Referrals
        public ActionResult Index()
        {
            return View(db.Referrals.Where(a=>a.IsDeleted==false).OrderByDescending(a=>a.CreationDate).ToList());
        }

        // GET: Referrals/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Referral referral = db.Referrals.Find(id);
            if (referral == null)
            {
                return HttpNotFound();
            }
            return View(referral);
        }

        // GET: Referrals/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Referrals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Referral referral)
        {
            if (ModelState.IsValid)
            {
				referral.IsDeleted=false;
				referral.CreationDate= DateTime.Now;
                
					
                db.Referrals.Add(referral);
                db.SaveChanges();
                var domain = string.Format("{0}://{1}", Request.Url.Scheme, Request.Url.Authority);
                referral.ReferralLink = $"{domain}/Referral/{referral.Id}";
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(referral);
        }

        // GET: Referrals/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Referral referral = db.Referrals.Find(id);
            if (referral == null)
            {
                return HttpNotFound();
            }
            return View(referral);
        }

        // POST: Referrals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Referral referral)
        {
            if (ModelState.IsValid)
            {
				referral.IsDeleted=false;
					referral.LastModifiedDate=DateTime.Now;
                db.Entry(referral).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(referral);
        }

        // GET: Referrals/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Referral referral = db.Referrals.Find(id);
            if (referral == null)
            {
                return HttpNotFound();
            }
            return View(referral);
        }

        // POST: Referrals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Referral referral = db.Referrals.Find(id);
			referral.IsDeleted=true;
			referral.DeletionDate=DateTime.Now;
 
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
        [AllowAnonymous]
        [Route("referral/{link}")]
        public ActionResult Referral(string link)
        {
            string path = Request.Url.AbsolutePath;

            if (path.StartsWith("/Referral/", StringComparison.OrdinalIgnoreCase))
            {
                Session["Referrer"] = path;
                return Redirect("/");
            }

            return Redirect("/");
        }
    }
}
