using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Enums;

namespace Models.Entities
{
   public class InventoryDetail : BaseEntity
   {
        public long InventoryId { get; set; }
        public virtual Inventory Inventory { get; set; }
        public long? RefrenceId { get; set; }
        public int Stock { get; set; }
        public InventoryDetailType InventoryDetailType { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
