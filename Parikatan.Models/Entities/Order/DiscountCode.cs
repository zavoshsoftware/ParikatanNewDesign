using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
   public class DiscountCode:BaseEntity
    {
        public DiscountCode()
        {
            Orders=new List<Order>();
            Users = new List<User>();
        }
        [Display(Name = "کد تخفیف")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [StringLength(10, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string Code { get; set; }
        [Display(Name = "تاریخ انقضا")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public DateTime ExpireDate { get; set; }
        [Display(Name = "تخفیف درصدی است؟")]
        public bool IsPercent { get; set; }
        [Display(Name = "مقدار تخفیف")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")] 
        public decimal Amount { get; set; }

        [Display(Name = "چند بار مصرف است؟")]
        public bool IsMultiUsing { get; set; }
        [Display(Name ="نام کاربری")]
        public long? UserId { get; set; }
        public User User { get; set; }

        //[Display(Name="بن کارت می باشد؟")]
        //public bool IsPhysicalCard { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

        [Display(Name="مخصوص سفارش اول است؟")]
        public bool IsForFirstOrder { get; set; }

        //public long? OrderId { get; set; }
        public bool IsUsed { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
