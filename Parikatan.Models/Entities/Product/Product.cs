using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Models.Entities
{
    public class Product : BaseEntity
    {
        public Product()
        {
            OrderDetails = new List<OrderDetail>();
            ProductComments = new List<ProductComment>();
            ProductFaqs = new List<ProductFaq>();
            ProductPackageItems = new List<ProductPackageItem>();
        }
        [Display(Name = "اولویت نمایش")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public int Order { get; set; }



        [Display(Name = "عنوان محصول")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        [StringLength(256, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string Title { get; set; }


        [StringLength(500, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string PageTitle { get; set; }

        [Display(Name = "تگ MetaDescription")]
        [StringLength(1000, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        [DataType(DataType.MultilineText)]
        public string PageDescription { get; set; }

        [Display(Name = "تصویر")]
        [StringLength(500, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string ImageUrl { get; set; }

        [Display(Name = "تصویر دوم")]
        [StringLength(500, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string SecondImageUrl { get; set; }

        [Display(Name = "توضیحات کوتاه")]
        [DataType(DataType.MultilineText)]
        public string Summery { get; set; }

        [Display(Name = "توضیحات")]
        [DataType(DataType.Html)]
        [AllowHtml]
        [Column(TypeName = "ntext")]
        [UIHint("RichText")]
        public string Body { get; set; }

        [Display(Name = "قیمت")]
        [Column(TypeName = "Money")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public decimal Amount { get; set; }

        [NotMapped]
        [Display(Name = "قیمت")]
        public string AmountStr
        {
            get { return Amount.ToString("n0") + " تومان"; }
        }

        [NotMapped]
        [Display(Name = "قیمت با تخفیف")]
        public string DiscountAmountStr
        {
            get
            {
                if (DiscountAmount != null)
                    return DiscountAmount.Value.ToString("n0") + " تومان";

                return string.Empty;
            }
        }

        [Display(Name = "درصد تخفیف")]
        public int? DiscountPercent { get; set; }
  

        [Display(Name = "قیمت با تخفیف")]
        public decimal? DiscountAmount { get; set; }

        [Display(Name = "تخفیف دارد؟")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public bool IsInPromotion { get; set; }

        [Display(Name = "ص اصلی؟")]
        [Required(ErrorMessage = "لطفا {0} را وارد نمایید.")]
        public bool IsInHome { get; set; }
 



        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<ProductFaq> ProductFaqs { get; set; }
        public virtual ICollection<ProductPackageItem> ProductPackageItems { get; set; }



        [Display(Name = "موجودی")]
        public int Stock { get; set; }


        [Display(Name = "بازدید")]
        public int Visit { get; set; }
        
        public virtual ICollection<ProductComment> ProductComments { get; set; }

        [Display(Name = "موجود؟")]
        public bool IsAvailable { get; set; }


        [Display(Name = "بارکد")]
        public string Barcode { get; set; }
        [Display(Name = "محصول پکیجی است؟")]
        public bool? IsPackage { get; set; }

        public ICollection<Inventory> Inventories { get; set; }

        [Display(Name = "قیمت خرید")]
        public decimal? PurchaseAmount { get; set; }

        [Display(Name = "دسته‌بندی")]
        public long? ProductCategoryId { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
        // ---- structured content for the product page (one item per line; "title|text" where noted) ----

        [Display(Name = "عنوان انگلیسی")]
        [StringLength(200, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string EnglishTitle { get; set; }

        [Display(Name = "نشان روی گالری (مثلاً کارآزمایی بالینی)")]
        [StringLength(100, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string Badge { get; set; }

        [Display(Name = "نکات کلیدی زیر عنوان (هر خط یک مورد)")]
        [DataType(DataType.MultilineText)]
        public string Highlights { get; set; }

        [Display(Name = "مشخصات (هر خط: عنوان|مقدار)")]
        [DataType(DataType.MultilineText)]
        public string Specs { get; set; }

        [Display(Name = "مزایا (هر خط: عنوان|توضیح)")]
        [DataType(DataType.MultilineText)]
        public string Benefits { get; set; }

        [Display(Name = "فناوری و مواد اثرگذار (هر خط: عنوان|توضیح)")]
        [DataType(DataType.MultilineText)]
        public string Technologies { get; set; }

        [Display(Name = "ترکیبات (هر خط یک ماده؛ مواد کلیدی با * در ابتدای خط)")]
        [DataType(DataType.MultilineText)]
        public string Ingredients { get; set; }

        [Display(Name = "مراحل مصرف (هر خط: عنوان|توضیح)")]
        [DataType(DataType.MultilineText)]
        public string UsageSteps { get; set; }

        [Display(Name = "متن تأییدیه علمی")]
        [DataType(DataType.MultilineText)]
        public string ClinicalText { get; set; }

        [Display(Name = "شماره تأییدیه")]
        [StringLength(100, ErrorMessage = "طول {0} نباید بیشتر از {1} باشد")]
        public string ClinicalNumber { get; set; }

        [Display(Name = "ارقام تأییدیه (هر خط: عدد یا مدت|توضیح)")]
        [DataType(DataType.MultilineText)]
        public string ClinicalStats { get; set; }

         
    }
}