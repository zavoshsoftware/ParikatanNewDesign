using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace Parikatan.Gift.ViewModel
{
    public class UsePromotionViewModel
    {
        [Display(Name="کد تخفیف")]
        [Required(ErrorMessage = "کد تخفیف را وارد کنید")]
        public string PromotionCode { get; set; }


        [Display(Name = "نام استفاده کننده")]
        public string UserFullName { get; set; }
        [Display(Name = "تاریخ استفاده")]
        public DateTime? UseDate { get; set; }
        [Display(Name = "تلفن استفاده کننده")]
        public string UserPhoneNumber { get; set; }
        [Display(Name = "خدمات استفاده شده")]
        public string UserService { get; set; }
    }
}