using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class OrderDetail : BaseEntity
    {
        public long OrderId { get; set; }

        public long ProductId { get; set; }

        [Display(Name = "تعداد")]
        public int Quantity { get; set; }

        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }

        [Column(TypeName = "Money")]
        public decimal RowAmount { get; set; }
        
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }

        [Column(TypeName = "Money")]
        public decimal? PurchaseAmount { get; set; }
        [Column(TypeName = "Money")]
        public decimal? PurchaseRowAmount { get; set; }


        [NotMapped]
        public string AmountStr { get { return Amount.ToString("N0"); } }

        [NotMapped]
        public string RowAmountStr { get { return RowAmount.ToString("N0"); } }
    }
}
