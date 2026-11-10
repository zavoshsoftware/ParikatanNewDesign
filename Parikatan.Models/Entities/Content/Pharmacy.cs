using System;
using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
    /// <summary>A point of sale (pharmacy or store) that carries Parikatan products.</summary>
    public class Pharmacy : BaseEntity
    {
        [Display(Name = "ترتیب شهر / منطقه")]
        public int RegionOrder { get; set; }

        [Display(Name = "شهر / منطقه")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [StringLength(100, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string Region { get; set; }

        [Display(Name = "نام مرکز فروش")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [StringLength(256, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string Title { get; set; }

        /// <summary>"pharmacy" or "store".</summary>
        [Display(Name = "نوع")]
        [StringLength(20)]
        public string Kind { get; set; }

        /// <summary>null = no label, "day" = روزانه, "eve" = نیمه‌وقت عصر, "h24" = شبانه‌روزی.</summary>
        [Display(Name = "ساعت فعالیت")]
        [StringLength(10)]
        public string Hours { get; set; }

        [Display(Name = "نشانی")]
        [StringLength(500, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string Address { get; set; }

        [Display(Name = "تلفن")]
        [StringLength(50, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string Phone { get; set; }

        [Display(Name = "ترتیب نمایش")]
        public int Order { get; set; }
    }
}
