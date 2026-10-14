using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
   public class PaymentType:BaseEntity
    {
        public PaymentType()
        {
            Orders = new List<Order>();
        }
        public string Title { get; set; }
        public string GatewayCode { get; set; }
        public string ImageUrl { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
