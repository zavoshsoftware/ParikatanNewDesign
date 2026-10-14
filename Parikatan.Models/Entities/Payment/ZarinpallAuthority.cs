using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace Models.Entities
{
    public class ZarinpallAuthority:BaseEntity
    {
        public string Authority { get; set; }
        public long OrderId { get; set; }
        public virtual Order Order { get; set; }
        public decimal Amount { get; set; }
        
    }
}