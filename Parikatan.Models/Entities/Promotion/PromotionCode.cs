using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Entities;

namespace Parikatan.Models.Entities.Promotion
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
         
        public bool IsUsed { get; set; }
        public string Reciever { get; set; }
        public string  UserFullName { get; set; }
        public DateTime? UseDate { get; set; }
        public string  UserPhoneNumber { get; set; }
        public string UserService { get; set; }
    }
}
