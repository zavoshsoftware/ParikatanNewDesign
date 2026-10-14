using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// مدل برگشتی سفارشی شده در برنامه
/// </summary>
public class CustomResponseViewModel
{
    public List<ProductApiViewModel> products { get; set; }
    public int total_items { get; set; }
    public int pages_count { get; set; }
    public int item_per_page { get; set; }
    public int page_num { get; set; }
}


/// <summary>
/// مدل نمایشی پیغام ها در مدل برگشتی سفارشی
/// </summary>
public class ProductApiViewModel
{
    public string title { get; set; }
    public string id { get; set; }
    public int price { get; set; }
    public int? old_price { get; set; }
    public string category { get; set; }
    public string color { get; set; }
    public string guarantee { get; set; }
    public bool is_available  { get; set; }
    public string url { get; set; }
}