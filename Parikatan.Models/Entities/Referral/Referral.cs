using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class Referral : BaseEntity
    {
        [Display(Name = "نام مسئول تبلیغات")]
        public string ReferralName { get; set; }
        [Display(Name = "لینک تبلیغ")]
        public string ReferralLink { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
