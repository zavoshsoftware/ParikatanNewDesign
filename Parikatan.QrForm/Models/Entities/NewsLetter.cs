using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parikatan.QrForm.Models.Entities
{
    public class NewsLetter : BaseEntity
    {
        [Display(Name = "موبایل")]
        public string PhoneNumber { get; set; }
    }
}
