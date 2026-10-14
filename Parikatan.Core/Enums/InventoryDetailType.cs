using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Core.Enums
{
    public enum InventoryDetailType
    {
        [Display(Name = "فروش محصول")]
        Sale = 1,
        [Display(Name = "خرید محصول")]
        Buy = 2,
        
    }
}