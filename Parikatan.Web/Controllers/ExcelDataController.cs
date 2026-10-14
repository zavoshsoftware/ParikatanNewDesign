using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using Helpers;
using Models;
using ViewModels;

namespace Parikatan.Web
{
    //[Authorize(Roles = "Administrator")]
    [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
    public class ExcelDataController : Controller
    {
        private DatabaseContext db = new DatabaseContext();
        public ActionResult Import()
        {
            UploadFileViewModel uploadFile = new UploadFileViewModel();
            return View(uploadFile);
        }

        [HttpPost]
        public ActionResult Import(UploadFileViewModel UploadFile)
        {
            if (ModelState.IsValid)
            {
                if (UploadFile.ExcelFile.ContentLength > 0)
                {
                    if (UploadFile.ExcelFile.FileName.EndsWith(".xlsx") || UploadFile.ExcelFile.FileName.EndsWith(".xls"))
                    {
                        XLWorkbook Workbook;
                        try
                        {
                            Workbook = new XLWorkbook(UploadFile.ExcelFile.InputStream);
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError(String.Empty, $"Check your file. {ex.Message}");
                            return View();
                        }
                        IXLWorksheet WorkSheet = null;
                        try
                        {
                            WorkSheet = Workbook.Worksheet("Sheet1");

                        }
                        catch
                        {
                            ModelState.AddModelError(String.Empty, "sheet not found!");
                            return View();
                        }
                        WorkSheet.FirstRow().Delete();//if you want to remove ist row


                        foreach (var row in WorkSheet.RowsUsed())
                        {
                            //if (!string.IsNullOrEmpty(row.Cell(4).Value.ToString()))
                            //    BrandCheck(row.Cell(4).Value.ToString());
                        }



                        int i = 1;
                        foreach (var row in WorkSheet.RowsUsed())
                        {
                            string fulname= row.Cell(1).Value.ToString()+" "+row.Cell(2).Value.ToString();

                            //SendSms.SendAds(row.Cell(3).Value.ToString(),
                            //    fulname);

                            i++;
                        }


                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "Only .xlsx and .xls files are allowed");
                        return View();
                    }
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Not a valid file");
                    return View();
                }
            }
            return View();
        }
         
    
    }
}