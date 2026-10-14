using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Parikatan.QrForm.Models;
using Parikatan.QrForm.Models.Entities;

namespace Parikatan.QrForm.Controllers
{
    public class HomeController : Controller
    {
        private DatabaseContext db = new DatabaseContext();
 
        public ActionResult Index()
        {
            return HttpNotFound();
        }
        [Route("qr")]
        public ActionResult Form()
        {
            InsertToVisit();
            return View();
        }

        public void InsertToVisit()
        {
            UserVisit userVisit = new UserVisit()
            {
                IsDeleted = false,
                IsActive = true,
                CreationDate = DateTime.Now,
                Ip = Request.UserHostAddress
            };
            db.UserVisits.Add(userVisit);
            db.SaveChanges();
        }
        [HttpPost]
        public ActionResult SubmitForm(string phoneNumber)
        {
            try
            {

                NewsLetter newsLetter = new NewsLetter()
                {
                    CreationDate = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
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