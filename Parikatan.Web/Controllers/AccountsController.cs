using Models;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Helpers;
//using ViewModels;
//using Microsoft.AspNet.Identity;
//using Microsoft.Owin.Security;
using System.Threading.Tasks;
using Core.Enums;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
//using Clotho.Core.Enums;
using Models.Entities;
using ViewModels;


namespace Parikatan.Web.Controllers
{
    public class AccountsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        [HttpGet]
        [Route("login")]
        public async Task<ActionResult> OtpLogin(string returnUrl)
        {
            ForgetPasswordViewModel model = new ForgetPasswordViewModel() { ReturnUrl = returnUrl };
            ViewBag.ReturnUrl = returnUrl;

            return View(model);
        }
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> OtpLogin(ForgetPasswordViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (model.UserCellNumber != null)
                {
                    model.UserCellNumber = PersianToEnglish(model.UserCellNumber);

                    bool isValidMobile = Regex.IsMatch(model.UserCellNumber,
                        @"(^(09|9)[0123456789][0123456789]\d{7}$)|(^(09|9)[0123456789][0123456789]\d{7}$)",
                        RegexOptions.IgnoreCase);

                    if (isValidMobile)
                    {

                        var user = db.Users.FirstOrDefault(x => x.CellNum == model.UserCellNumber && x.SecurityRole == SecurityRole.Customer && x.IsDeleted == false);

                        if (user != null)
                        {
                            user.Password = RandomCode();
                            user.LastModifiedDate = DateTime.Now;

                            db.SaveChanges();

                            SendSms.SendOtp(user.CellNum, user.Password);

                            return RedirectToAction("Activate", new { returnUrl = returnUrl, code = user.Id });
                        }
                        else
                        {
                            if (model.UserCellNumber.Length < 11)
                            {

                                TempData["WrongMobile"] = "شماره موبایل وارد شده صحیح نمی باشد";
                                return View(model);
                            }
                            User oUser = new User()
                            {

                                CellNum = model.UserCellNumber,
                                Password = RandomCode(),
                                IsDeleted = false,
                                IsActive = true,
                                CreationDate = DateTime.Now,
                                SecurityRole = SecurityRole.Customer
                            };

                            db.Users.Add(oUser);
                            db.SaveChanges();

                            SendSms.SendOtp(oUser.CellNum, oUser.Password);

                            return RedirectToAction("Activate", new { returnUrl = returnUrl, code = oUser.Id });
                        }
                    }
                    else
                    {
                        TempData["WrongMobile"] = "شماره موبایل وارد شده صحیح نمی باشد";
                        return View(model);
                    }
                }

            }
            else
            {
                TempData["WrongMobile"] = "شماره موبایل خود را وارد نمایید";
                return View(model);
            }
            return View();
        }


        [HttpGet]
        [Route("activate")]
        public async Task<ActionResult> Activate(int code, string returnUrl)
        {
            var user = db.Users.Where(c => c.Id == code).Select(c => new { c.CellNum, c.Password }).FirstOrDefault();
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.code = code;

            if (user != null)
            {
                ActivateAccountViewModel activateViewModel = new ActivateAccountViewModel()
                {
                    CellNumber = user.CellNum,
                };
                return View(activateViewModel);
            }

            return RedirectToAction("Login");
        }

        [Route("Activate")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Activate(int code, ActivateAccountViewModel activateViewModel, string returnUrl)
        {
            if (!string.IsNullOrEmpty(activateViewModel.ActivationCode))
            {
                string activecode = PersianToEnglish(activateViewModel.ActivationCode);

                User user = db.Users.FirstOrDefault(c =>
                    c.Id == code && c.Password == activecode && c.IsDeleted == false);


                if (user != null)
                {
                    if (!user.IsActive)
                    {
                        user.IsActive = true;
                        user.LastModifiedDate = DateTime.Now;

                        db.SaveChanges();
                    }

                    LoginTask(user);

                    if (returnUrl != null)
                        return Redirect(returnUrl);

                    return RedirectToAction("index", "home");
                }

                TempData["WrongActivationCode"] = "کد فعالسازی وارد شده صحیح نمی باشد.";
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.code = code;

                return View(activateViewModel);
            }


            TempData["WrongActivationCode"] = "کد فعالسازی را وارد کنید.";
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.code = code;

            return View(activateViewModel);
        }


        public string RandomCode()
        {
            Random generator = new Random();
            String r = generator.Next(0, 100000).ToString("D5");
            return (r);
        }


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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginRegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                User oUser = db.Users
                    .FirstOrDefault(a =>
                        a.CellNum == model.LoginCellNumber
                        && a.Password == model.LoginPassword
                        && a.IsActive
                        && a.IsDeleted == false);

                if (oUser != null)
                {
                    LoginTask(oUser);
                    return RedirectToLocal(model.ReturnUrl, oUser.SecurityRole);
                }
                else
                {
                    // invalid username or password
                    TempData["WrongPass"] = "شماره موبایل و یا کلمه عبور وارد شده صحیح نمی باشد.";
                }
            }

            LoginRegisterViewModel result = new LoginRegisterViewModel();
            result.ReturnUrl = model.ReturnUrl;
            return RedirectToAction("LoginRegister");
        }


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

        private ActionResult RedirectToLocal(string returnUrl, string role)
        {

            if (role.ToLower().Contains("admin"))
                return RedirectToAction("index", "Products");


            if (!string.IsNullOrEmpty(returnUrl))
            {
                if (returnUrl == "checkout")
                    return Redirect("/checkout");
                return Redirect(returnUrl);
            }
            if (role.ToLower().Contains("customer"))
                return RedirectToAction("Index", "Home");


            return Redirect("/");
        }
        public ActionResult LogOff()
        {
            if (User.Identity.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.SignOut();
            }
            return Redirect("/");
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
                    //return RedirectToLocal(returnUrl, oUser.Role.Name); // auth succeed 

                    if(oUser.SecurityRole==SecurityRole.Marketing)
                        return Redirect("/orders/indexmarketing"); // auth succeed 

                    return Redirect("/blogs"); // auth succeed 
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