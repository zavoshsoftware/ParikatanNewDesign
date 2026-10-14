using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using Models;
using Models.Entities;

namespace Helpers
{
    public class ZarinPalHelper
    {
        private String MerchantId = WebConfigurationManager.AppSettings["MerchantId"];
 
        private DatabaseContext db = new DatabaseContext();

        public string GetMerchantId()
        {
            return MerchantId;
        }
        public string ZarinPalRedirect(Order order, decimal amount)
        {
            ZarinPal.ZarinPal zarinpal = ZarinPal.ZarinPal.Get();

            //String CallbackURL = "http://localhost:21745/callback/zp";
            String CallbackURL = WebConfigurationManager.AppSettings["callBackUrlZaringPal"];

            long Amount = Convert.ToInt64(amount);

            List<OrderDetail> orderDetails = db.OrderDetails.Where(current => current.OrderId == order.Id).ToList();
            //string productDesc = null;
            //foreach (OrderDetail orderDetail in orderDetails)
            //{
            //    productDesc = productDesc + orderDetail.Product.Code + ",";
            //}

            String description = "خرید محصول" ;

           // ZarinPal.PaymentRequest pr = new ZarinPal.PaymentRequest(MerchantId, Amount, CallbackURL, description);
            ZarinPal.PaymentRequest pr2 = new ZarinPal.PaymentRequest(MerchantId, Amount, CallbackURL, description)
            {
                Description = description,
                Amount = Amount,
                CallbackURL = CallbackURL,
                Email = "babaei.aho@gmail.com",
                MerchantID = MerchantId,
                Mobile = "09124806404"
            };

            zarinpal.DisableSandboxMode();
          // zarinpal.EnableSandboxMode();
            try
            {
                var res = zarinpal.InvokePaymentRequest(pr2);
                if (res.Status == 100)
                {
                    InsertToAuthority(order.Id, res.Authority, amount);
                    return res.PaymentURL;
                }
                else
                    return "false";
            }
            catch (Exception e)
            {
                return "zarrin";
            }
        }

        public void InsertToAuthority(long orderId, string authority, decimal amount)
        {
            ZarinpallAuthority zarinpallAuthority = new ZarinpallAuthority()
            {
                OrderId = orderId,
                Authority = authority,
                Amount = amount,
                CreationDate = DateTime.Now,
                IsDeleted = false,
                IsActive = true
            };

            db.ZarinpallAuthorities.Add(zarinpallAuthority);
            db.SaveChanges();
        }

        public long GetAmountByAuthority(string authority)
        {
            ZarinpallAuthority zarinpallAuthority =
                db.ZarinpallAuthorities.FirstOrDefault(current => current.Authority == authority);

            if (zarinpallAuthority != null)
                return Convert.ToInt64(zarinpallAuthority.Amount);

            return 0;
        }

        public Order GetOrderByAuthority(string authority)
        {
            ZarinpallAuthority zarinpallAuthority =
                db.ZarinpallAuthorities.FirstOrDefault(current => current.Authority == authority);

            if (zarinpallAuthority != null)
                return db.Orders.Find(zarinpallAuthority.OrderId);
            else
                return null;
        }
    }
}