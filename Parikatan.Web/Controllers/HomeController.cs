using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Helpers;
using Models;
using System.Data.Entity;
using ViewModels.Home;
using Models.Entities;

namespace Parikatan.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        private DatabaseContext db = new DatabaseContext();
        public ActionResult Index()
        {
            HomeViewModel result = new HomeViewModel();

            var activeProducts = db.Products
                .Include(p => p.ProductCategory)
                .Where(p => p.IsDeleted == false && p.IsActive);

            result.ActiveProductCount = activeProducts.Count();

            // best-sellers: products flagged "show on home"; fall back to the first active products
            result.Products = activeProducts.Where(p => p.IsInHome)
                .OrderByDescending(p => p.IsAvailable).ThenByDescending(p => p.Order).Take(10).ToList();
            if (result.Products.Count == 0)
            {
                result.Products = activeProducts
                    .OrderByDescending(p => p.IsAvailable).ThenByDescending(p => p.Order).Take(10).ToList();
            }

            result.Categories = db.ProductCategories
                .Where(c => c.IsDeleted == false && c.IsActive)
                .OrderBy(c => c.Order).ToList();

            result.Ingredients = db.TextItems
                .Where(t => t.IsDeleted == false && t.IsActive && t.TextItemType.TypeName == "ingredient")
                .OrderBy(t => t.Id)
                .ToList()
                .Select(t => new IngredientItem { Title = t.Title, Text = t.Summery, Url = t.LinkUrl })
                .ToList();
            if (result.Ingredients.Count == 0)
            {
                result.Ingredients = DefaultIngredients();
            }

            result.Blogs = db.Blogs.Include(b => b.BlogGroup)
                .Where(c => c.IsDeleted == false && c.IsActive)
                .OrderByDescending(c => c.CreationDate).Take(3).ToList();

            ViewBag.Title = "فروشگاه اینترنتی پریکاتن - خرید کرم پریکاتن اصل";
            ViewBag.Description = "خرید کرم پریکاتن اصل شامل کرم ضد چروک صورت پریکاتن و کرم دور چشم پریکاتن و سرم روشن کننده پریکاتن اصل از فروشگاه اینترنتی رسمی شرکت پریکاتن";
            ViewBag.Canonical = "https://parikatan.shop/";
            ViewBag.NavActive = "home";
            return View(result);
        }

        // Used until ingredient items (TextItemType.TypeName = "ingredient") are entered in the admin panel.
        private static List<IngredientItem> DefaultIngredients()
        {
            return new List<IngredientItem>
            {
                new IngredientItem { Title = "بیوپلاسما FA", Text = "از تخریب کلاژن جلوگیری می‌کند و به سنتز دوباره‌ی آن کمک می‌کند." },
                new IngredientItem { Title = "فیتوپلانکتون", Text = "منبع پروتئین، ویتامین و آنتی‌اکسیدان با جذب بیش از ۹۰٪." },
                new IngredientItem { Title = "پالمیتویل گلایسین", Text = "افزایش میکروسیرکولاسیون و بهبود توزیع کلاژن در لایه‌ی درم." },
                new IngredientItem { Title = "روغن آووکادو", Text = "مرطوب‌کننده و آنتی‌اکسیدان در برابر رادیکال‌های آزاد." },
                new IngredientItem { Title = "فناوری TIMECODE", Text = "آبرسانی سریع و هیدراتاسیون ۴ برابری پوست." },
                new IngredientItem { Title = "ترانگزامیک اسید", Text = "مهار ساخت ملانین برای کاهش لک و تیرگی پوست." }
            };
        }

        [HttpPost]
        public ActionResult Subscribe(string email)
        {
            email = (email ?? string.Empty).Trim();
            if (email.Length == 0 || email.Length > 200 || !email.Contains("@") || !email.Contains("."))
            {
                return Json(new { ok = false, message = "ایمیل واردشده معتبر نیست." });
            }

            if (!db.NewsLetters.Any(n => n.Email == email && n.IsDeleted == false))
            {
                db.NewsLetters.Add(new NewsLetter
                {
                    Email = email,
                    IsActive = true,
                    IsDeleted = false,
                    CreationDate = DateTime.Now
                });
                db.SaveChanges();
            }

            return Json(new { ok = true, message = "عضویت شما در خبرنامه ثبت شد." });
        }

        [HttpPost]
        public ActionResult Dealership(string name, string phone, string city, string notes)
        {
            name = (name ?? string.Empty).Trim();
            phone = (phone ?? string.Empty).Trim();
            if (name.Length == 0 || phone.Length < 7)
            {
                return Json(new { ok = false, message = "نام و شماره تماس را وارد کنید." });
            }

            string message = "درخواست نمایندگی | شهر/استان: " + (city ?? string.Empty).Trim() + " | " + (notes ?? string.Empty).Trim();
            if (message.Length > 300)
                message = message.Substring(0, 300);

            db.ContactUsForms.Add(new ContactUsForm
            {
                Name = name.Length > 300 ? name.Substring(0, 300) : name,
                PhoneNumber = phone,
                Message = message,
                Ip = Request.UserHostAddress ?? string.Empty,
                IsActive = true,
                IsDeleted = false,
                CreationDate = DateTime.Now
            });
            db.SaveChanges();

            return Json(new { ok = true, message = "درخواست شما ثبت شد؛ همکاران ما با شما تماس می‌گیرند." });
        }
        [Route("about")]
        public ActionResult About()
        {
            ViewBag.Title = "درباره ما | پریکاتن";
            ViewBag.Description = "درباره‌ی شرکت پویا ورزان تجارت سپاهان و برند پریکاتن: تولید محصولات مراقبت از پوست با فرمولاسیون علمی، مواد اولیه‌ی باکیفیت و مطالعات بالینی.";
            ViewBag.Canonical = "https://parikatan.shop/about";
            ViewBag.NavActive = "about";
            return View();
        }

        [Route("contact")]
        public ActionResult Contact()
        {
            return View();
        }
        [Route("buy")]
        public ActionResult Buy()
        {
            return View();
        }
        [Route("how")]
        public ActionResult How()
        {
            SendSms.SendCommonSmsV2("", "");
            return View();
        }

        static string GenerateUniqueCode(HashSet<string> existingCodes)
        {
            Random random = new Random();
            string code;
            do
            {
                code = GenerateRandomCode(random);
            } while (existingCodes.Contains(code)); // اگر کد تکراری بود دوباره تولید می‌کند

            existingCodes.Add(code); // اضافه کردن کد جدید به لیست کدهای موجود
            return code;
        }

        // متدی برای تولید یک رشته 8 رقمی تصادفی
        static string GenerateRandomCode(Random random)
        {
            char[] chars = new char[8];
            for (int i = 0; i < 6; i++)
            {
                chars[i] = (char)('0' + random.Next(1, 10)); // تولید یک رقم تصادفی بین 0 تا 9
            }
            return new string(chars);
        }

        public ActionResult Main()
        {
            HashSet<string> existingCodes = new HashSet<string>();
            List<string> result = new List<string>();
            // تولید 5 کد یونیک برای مثال
            for (int i = 0; i < 1000; i++)
            {
                string uniqueCode = GenerateUniqueCode(existingCodes);
                Console.WriteLine($"Unique Code {i + 1}: {uniqueCode}");
                result.Add(uniqueCode);
            }

            return View(result);
        }

    }
}