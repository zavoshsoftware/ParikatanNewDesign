using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace Core.Enums
{
    public enum OrderType
    {
        [Display(Name = "سایت")]
        Site = 1,
        [Display(Name = "واتس اپ")]
        whatsapp = 2,
        [Display(Name = "با سلام")]
        basalam = 3,
        [Display(Name = "ژین بیوتی")]
        zheen = 4,
        
    }
}