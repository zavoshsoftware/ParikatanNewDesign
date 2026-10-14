using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Core.Enums
{
    public enum OrderStatus
    {
        [Display(Name = "در انتظار پرداخت")]
        WaitForPayment = 1,
        [Display(Name = "در حال پردازش")]
        InProgress = 2,
        [Display(Name = "ارسال شده")]
        Sent = 3,
        [Display(Name = "تحویل شده")]
        Delivered = 4
    }
}