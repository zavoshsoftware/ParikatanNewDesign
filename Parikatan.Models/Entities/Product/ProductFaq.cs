using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace Models.Entities
{
    public class ProductFaq : BaseEntity
    {
         
        public int Order { get; set; }
        public string Question { get; set; }

        public string Response { get; set; }
        
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }

     
    }
}