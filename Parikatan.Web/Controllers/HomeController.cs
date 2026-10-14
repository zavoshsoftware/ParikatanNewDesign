using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Helpers;
using Models;
using ViewModels.Home;

namespace Parikatan.Web.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        private DatabaseContext db = new DatabaseContext();
        public ActionResult Index()
        {
            HomeViewModel result = new HomeViewModel();
            result.Blogs = db.Blogs.Where(c => c.IsDeleted == false && c.IsActive)
                .OrderByDescending(c => c.CreationDate).Take(3).ToList();
            return View(result);
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