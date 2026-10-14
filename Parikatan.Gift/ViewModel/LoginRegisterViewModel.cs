using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Parikatan.Gift.ViewModel
{
    public class LoginRegisterViewModel
    {
        public string LoginCellNumber { get; set; }
        public string LoginPassword { get; set; }
 
        public string ReturnUrl { get; set; }
    }
}