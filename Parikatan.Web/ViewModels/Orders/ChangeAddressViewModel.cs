using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ViewModels.Orders
{
    public class ChangeAddressViewModel
    {
        public long Id { get; set; }
        [Display(Name="استان")]
        public long ProvinceId { get; set; }
        [Display(Name="شهر")]
        public long CityId { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name="آدرس")]
        public string Address { get; set; }
        [Display(Name="کد پستی")]
        public string PostalCode { get; set; }
    }
}