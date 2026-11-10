using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Models;
using Models.Entities;

namespace Parikatan.Web.Controllers
{
    [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
    public class ContactUsFormsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: ContactUsForms
        public ActionResult Index()
        {
            return View(db.ContactUsForms.Where(a=>a.IsDeleted==false).OrderByDescending(a=>a.CreationDate).ToList());
        }

        // GET: ContactUsForms/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUsForm contactUsForm = db.ContactUsForms.Find(id);
            if (contactUsForm == null)
            {
                return HttpNotFound();
            }
            return View(contactUsForm);
        }

        // GET: ContactUsForms/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ContactUsForms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ContactUsForm contactUsForm)
        {
            if (ModelState.IsValid)
            {
				contactUsForm.IsDeleted=false;
				contactUsForm.CreationDate= DateTime.Now; 
					
                db.ContactUsForms.Add(contactUsForm);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(contactUsForm);
        }

        // GET: ContactUsForms/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUsForm contactUsForm = db.ContactUsForms.Find(id);
            if (contactUsForm == null)
            {
                return HttpNotFound();
            }
            return View(contactUsForm);
        }

        // POST: ContactUsForms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ContactUsForm contactUsForm)
        {
            if (ModelState.IsValid)
            {
				contactUsForm.IsDeleted=false;
					contactUsForm.LastModifiedDate=DateTime.Now;
                db.Entry(contactUsForm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(contactUsForm);
        }

        // GET: ContactUsForms/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ContactUsForm contactUsForm = db.ContactUsForms.Find(id);
            if (contactUsForm == null)
            {
                return HttpNotFound();
            }
            return View(contactUsForm);
        }

        // POST: ContactUsForms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ContactUsForm contactUsForm = db.ContactUsForms.Find(id);
			contactUsForm.IsDeleted=true;
			contactUsForm.DeletionDate=DateTime.Now;
 
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
        [HttpPost]
        public ActionResult SubmitComment(string name, string email, string body, string phone)
        {
          
            name = (name ?? string.Empty).Trim();
            body = (body ?? string.Empty).Trim();
            phone = (phone ?? string.Empty).Trim();

            // the entity limits (name 300, message 300, ip 50) would otherwise throw on SaveChanges
            if (name.Length == 0 || body.Length == 0 || phone.Length == 0)
                return Json("invalid", JsonRequestBehavior.AllowGet);

            ContactUsForm comment = new ContactUsForm();

            comment.Name = name.Length > 300 ? name.Substring(0, 300) : name;
            comment.Message = body.Length > 300 ? body.Substring(0, 300) : body;
            comment.CreationDate = DateTime.Now;
            comment.IsDeleted = false;
            comment.IsActive = false;
            string ip = Request.UserHostAddress ?? string.Empty;
            comment.Ip = ip.Length > 50 ? ip.Substring(0, 50) : ip;
            comment.PhoneNumber = phone.Length > 50 ? phone.Substring(0, 50) : phone;

            db.ContactUsForms.Add(comment);
            db.SaveChanges();
            return Json("true", JsonRequestBehavior.AllowGet);

        }
    }
}
