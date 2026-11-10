using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
    public class ProductCategory : BaseEntity
    {
        public ProductCategory()
        {
            Products = new List<Product>();
        }

        [Display(Name = "اولویت نمایش")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public int Order { get; set; }

        [Display(Name = "عنوان دسته")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [StringLength(256, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string Title { get; set; }

        [Display(Name = "عنوان کوتاه (نوار دسته‌ها)")]
        [StringLength(100, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string ShortTitle { get; set; }

        [Display(Name = "پارامتر url")]
        [StringLength(256, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string UrlParam { get; set; }

        [Display(Name = "تصویر")]
        [StringLength(500, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string ImageUrl { get; set; }

        [Display(Name = "نمایش در کاشی‌های صفحه اصلی؟")]
        public bool IsInHome { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
