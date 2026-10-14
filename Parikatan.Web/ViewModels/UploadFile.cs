using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ViewModels
{
    public class UploadFileViewModel
    {
        [Required]
        public HttpPostedFileBase ExcelFile { get; set; }
    }
}