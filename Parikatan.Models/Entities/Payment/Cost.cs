using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;

namespace Models.Entities
{
   public class Cost:BaseEntity
    {
        public decimal Amount { get; set; }
        public CostType CostType { get; set; }
        public string Reciever { get; set; }
        public DateTime PayDate { get; set; }
    }
}
