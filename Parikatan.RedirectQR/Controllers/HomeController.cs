using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
 

namespace Parikatan.RedirectQR.Controllers
{
    public class HomeController : Controller
    {
         
 
        public ActionResult Index()
        {
            return RedirectPermanent("https://pirikaten.com/qr");

        }
        [Route("qr")]
        public ActionResult Form()
        {
            return RedirectPermanent("https://pirikaten.com/qr");
 
        }

        [Route("agency")]
        public ActionResult AgencyForm()
        {
            return RedirectPermanent("https://parikatan.com/receive-representative-points/");
 
        }

      
    }
}