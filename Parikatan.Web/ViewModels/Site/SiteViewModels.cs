using System.Collections.Generic;
using Models.Entities;

namespace ViewModels.Site
{
    /// <summary>Data for the shared header (mega menu / mobile drawer) and footer.</summary>
    public class SiteMenuViewModel
    {
        public List<Product> Products { get; set; }
    }
}
