using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
   public class Inventory : BaseEntity
   {
        public long ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int Stock { get; set; }
    }
}
