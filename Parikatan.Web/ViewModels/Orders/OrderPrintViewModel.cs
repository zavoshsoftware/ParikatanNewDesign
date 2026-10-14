using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ViewModels.Orders
{
    public class OrderPrintViewModel
    {
        
        public string Address { get; set; }
        [Display(Name="کد پستی")]
        public string PostalCode { get; set; }

        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
    }
}