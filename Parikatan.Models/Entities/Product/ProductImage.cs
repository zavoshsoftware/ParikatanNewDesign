using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Models.Entities
{
    public class ProductImage : BaseEntity
    {


        [Display(Name = "اولویت نمایش")]
        public int Order { get; set; }
        [Display(Name = "عنوان")]
        public string Title { get; set; }

        [Display(Name = "تصویر")]
        [MaxLength(200)]
        public string ImageUrl { get; set; }

        public long ProductId { get; set; }
        public Product Product { get; set; }
 
         
    }
}