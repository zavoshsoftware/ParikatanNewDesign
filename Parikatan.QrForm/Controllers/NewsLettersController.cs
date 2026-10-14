using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using Parikatan.QrForm.Helper;
using Parikatan.QrForm.Models;
using Parikatan.QrForm.Models.Entities;

namespace Parikatan.QrForm.Controllers
{
   [Authorize(Roles = "superadmin")]
    public class NewsLettersController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        // GET: NewsLetters
        public ActionResult Index(string nlType)
        {
            if (!string.IsNullOrEmpty(nlType))
            {
                ViewBag.Title = "فهرست ثبت نام کنندگان قدیم";

                return View(db.NewsLetters.Where(a => a.IsDeleted == true).OrderByDescending(a => a.CreationDate)
                    .ToList());
            }

            ViewBag.Title = "فهرست ثبت نام کنندگان جدید";
            return View(db.NewsLetters.Where(a => a.IsDeleted == false).OrderByDescending(a => a.CreationDate).ToList());
        }
        // GET: NewsLetters


        // GET: NewsLetters/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsLetter newsLetter = db.NewsLetters.Find(id);
            if (newsLetter == null)
            {
                return HttpNotFound();
            }
            return View(newsLetter);
        }

        // GET: NewsLetters/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NewsLetters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PhoneNumber,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] NewsLetter newsLetter)
        {
            if (ModelState.IsValid)
            {
                newsLetter.IsDeleted = false;
                newsLetter.CreationDate = DateTime.Now;

                db.NewsLetters.Add(newsLetter);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(newsLetter);
        }

        // GET: NewsLetters/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsLetter newsLetter = db.NewsLetters.Find(id);
            if (newsLetter == null)
            {
                return HttpNotFound();
            }
            return View(newsLetter);
        }

        // POST: NewsLetters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PhoneNumber,IsActive,CreationDate,LastModifiedDate,IsDeleted,DeletionDate,Description")] NewsLetter newsLetter)
        {
            if (ModelState.IsValid)
            {
                newsLetter.IsDeleted = false;
                newsLetter.LastModifiedDate = DateTime.Now;
                db.Entry(newsLetter).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(newsLetter);
        }

        // GET: NewsLetters/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NewsLetter newsLetter = db.NewsLetters.Find(id);
            if (newsLetter == null)
            {
                return HttpNotFound();
            }
            return View(newsLetter);
        }

        // POST: NewsLetters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            NewsLetter newsLetter = db.NewsLetters.Find(id);
            newsLetter.IsDeleted = true;
            newsLetter.DeletionDate = DateTime.Now;

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

        public ActionResult Export()
        {

     


            var dt = db.NewsLetters.Where(c => !c.IsDeleted).ToList().Select(c =>
                new
                {
                    phonenumber = c.PhoneNumber,
                    datetime = c.CreationDate

                }).ToList().ToDataTable();
            dt.TableName = "اکسل";

            var nl = db.NewsLetters.Where(c => c.IsDeleted == false).ToList();
            foreach (NewsLetter newsLetter in nl)
            {
                newsLetter.IsDeleted = true;
                newsLetter.DeletionDate = DateTime.Now;

            }

            db.SaveChanges();

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                wb.Worksheets.FirstOrDefault().ColumnWidth = 20;
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{dt.TableName}{DateTime.Now}.xlsx");
                }
            }



            return RedirectToAction("Index");
        }


    }
}
