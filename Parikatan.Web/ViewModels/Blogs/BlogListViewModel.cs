using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;
using Models.Entities;

namespace ViewModels.Blogs
{
    public class BlogListViewModel:_BaseViewModel
    {
        public BlogGroup BlogGroup { get; set; }
        public List<Blog> Blogs { get; set; }
        /// <summary>Groups that have at least one published article (topic chips).</summary>
        public System.Collections.Generic.List<BlogGroup> Groups { get; set; }
    }
}