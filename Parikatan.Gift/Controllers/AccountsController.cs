
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

//using ViewModels;
//using Microsoft.AspNet.Identity;
//using Microsoft.Owin.Security;
using System.Threading.Tasks;

using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Parikatan.Gift.Models;
using Parikatan.Gift.Models.Entities;
using Parikatan.Gift.ViewModel;


//using Clotho.Core.Enums;



namespace Parikatan.Gift.Controllers
{
    public class AccountsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();



        public string PersianToEnglish(string persianStr)
        {

            Dictionary<string, string> LettersDictionary = new Dictionary<string, string>
            {
                ["۰"] = "0",
                ["۱"] = "1",
                ["۲"] = "2",
                ["۳"] = "3",
                ["۴"] = "4",
                ["۵"] = "5",
                ["۶"] = "6",
                ["۷"] = "7",
                ["۸"] = "8",
                ["۹"] = "9"
            };
            return LettersDictionary.Aggregate(persianStr, (current, item) =>
                current.Replace(item.Key, item.Value));
        }



        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Login(LoginRegisterViewModel model)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        User oUser = db.Users
        //            .FirstOrDefault(a =>
        //                a.CellNum == model.LoginCellNumber
        //                && a.Password == model.LoginPassword
        //                && a.IsActive
        //                && a.IsDeleted == false);

        //        if (oUser != null)
        //        {
        //            LoginTask(oUser);
        //            return RedirectToLocal(model.ReturnUrl, oUser.SecurityRole);
        //        }
        //        else
        //        {
        //            // invalid username or password
        //            TempData["WrongPass"] = "شماره موبایل و یا کلمه عبور وارد شده صحیح نمی باشد.";
        //        }
        //    }

        //    LoginRegisterViewModel result = new LoginRegisterViewModel();
        //    result.ReturnUrl = model.ReturnUrl;
        //    return RedirectToAction("LoginRegister");
        //}


        public void LoginTask(User oUser)
        {
            string sirName = oUser.CellNum;
            if (oUser.FullName != null)
                sirName = oUser.FullName;

            var ident = new ClaimsIdentity(
            new[] { 
                    // adding following 2 claim just for supporting default antiforgery provider
                    new Claim(ClaimTypes.NameIdentifier, oUser.CellNum),
                    new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),

                    new Claim(ClaimTypes.Name,oUser.Id.ToString()),

                    // optionally you could add roles if any
                    new Claim(ClaimTypes.Role, oUser.SecurityRole),

                    new Claim(ClaimTypes.Surname, sirName),

            },
            DefaultAuthenticationTypes.ApplicationCookie);

            HttpContext.GetOwinContext().Authentication.SignIn(
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(600),

                },
                ident);
        }

        private ActionResult RedirectToLocal(string role)
        {

            if (role.ToLower().Contains("admin"))
                return RedirectToAction("index", "PromotionCodes");
            return RedirectToAction("Use", "PromotionCodes");




            return Redirect("/");
        }
        public ActionResult LogOff()
        {
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.SignOut();
            }
            return Redirect("/panellogin");
        }




        [Route("PanelLogin")]
        public ActionResult Login(string ReturnUrl = "")
        {

            ViewBag.Message = "";
            ViewBag.ReturnUrl = ReturnUrl;

            LoginPageViewModel login = new LoginPageViewModel()
            {
                //MenuGalleryGroups = baseViewModelHelper.GetMenuGalleryGroups(),
                //MenuItem = baseViewModelHelper.GetMenuItems(),
            };
            return View(login);

        }

        [Route("PanelLogin")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginPageViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                User oUser = db.Users.FirstOrDefault(a => a.CellNum == model.Username && a.Password == model.Password);

                if (oUser != null)
                {
                    var ident = new ClaimsIdentity(
                      new[] { 
              // adding following 2 claim just for supporting default antiforgery provider
              new Claim(ClaimTypes.NameIdentifier, oUser.Id.ToString()),
              new Claim("http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider", "ASP.NET Identity", "http://www.w3.org/2001/XMLSchema#string"),
              new Claim(ClaimTypes.Name,oUser.Id.ToString()),

              // optionally you could add roles if any
               new Claim(ClaimTypes.Role, oUser.SecurityRole),

                      },
                      DefaultAuthenticationTypes.ApplicationCookie);

                    HttpContext.GetOwinContext().Authentication.SignIn(
                       new AuthenticationProperties { IsPersistent = true }, ident);
                    return RedirectToLocal( oUser.SecurityRole); // auth succeed 
                }
                else
                {
                    // invalid username or password
                    TempData["WrongPass"] = "نام کاربری و یا کلمه عبور وارد شده صحیح نمی باشد.";
                }
            }


            return View(model);
        }




    }
}