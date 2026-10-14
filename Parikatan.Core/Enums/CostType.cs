using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Core.Enums
{
    public enum CostType
    {
        [Display(Name = "هزینه ارسال")]
        Shipment = 1,
        [Display(Name = "تامین کننده")]
        Supplier = 2,
        [Display(Name = "وسایل جانبی")]
        Material = 3,
        [Display(Name = "تبلیغات")]
        Ads = 4,
        
    }
}