using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
namespace Parikatan.Gift.Models.Entities.Promotion
{
    public class PromotionCode:BaseEntity
    {
        [Display(Name = "کد")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public string Code { get; set; }

        [Display(Name = "تاریخ انقضا")]
        public DateTime ExpireDate { get; set; }
 
        [Display(Name = "مقدار تخفیف")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public decimal Amount { get; set; }
         
        [Display(Name = "استفاده شده؟")]
        public bool IsUsed { get; set; }
        [Display(Name = "تحویل گیرنده")]
        public string Reciever { get; set; }
        [Display(Name = "نام استفاده کننده")]
        public string  UserFullName { get; set; }
        [Display(Name = "تاریخ استفاده")]
        public DateTime? UseDate { get; set; }
        [Display(Name = "تلفن استفاده کننده")]
        public string  UserPhoneNumber { get; set; }
        [Display(Name = "خدمات استفاده شده")]
        public string UserService { get; set; }

        [Display(Name = "کاربر استفاده کننده")]
        public long? UserId { get; set; }

        public virtual User User { get; set; }
    }
}
