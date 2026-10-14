using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parikatan.Web.ViewModels.Orders
{
    public class ManualOrderDetailViewModel
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
    }
}