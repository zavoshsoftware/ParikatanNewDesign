using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ViewModels;

namespace Helpers
{
    public static class PageContentHelper
    {
        public static string SiteContentName(this HtmlHelper helper, string key)
        {
            DatabaseContext db = new DatabaseContext();
            var content = db.TextItems.SingleOrDefault(x => x.Name.Equals(key));
            if (content == null)
            {
                return key;
            }
            else
            {
                return content.Name;
            }

        }
        public static string SiteContentTitle(this HtmlHelper helper, string key)
        {
            DatabaseContext db = new DatabaseContext();
            var content = db.TextItems.SingleOrDefault(x => x.Name.Equals(key));
            if (content == null)
            {
                return key;
            }
            else
            {
                return content.Title;
            }

        }
        public static string SiteContentSummery(this HtmlHelper helper, string key)
        {
            DatabaseContext db = new DatabaseContext();
            var content = db.TextItems.SingleOrDefault(x => x.Name.Equals(key));
            if (content == null)
            {
                return key;
            }
            else
            {
                return content.Summery;
            }

        }

         
        public static string SiteContentImage(this HtmlHelper helper, string key)
        {
            DatabaseContext db = new DatabaseContext();
            var content = db.TextItems.SingleOrDefault(x => x.Name.Equals(key));
            if (content == null)
            {
                return key;
            }
            else
            {
                return content.ImageUrl;
            }

        }
        public static string SiteContentBody(this HtmlHelper helper, string key)
        {
            DatabaseContext db = new DatabaseContext();
            var content = db.TextItems.SingleOrDefault(x => x.Name.Equals(key));
            if (content == null)
            {
                return key;
            }
            else
            {
                return content.Body;
            }

        }
        public static string SiteContentUrlLink(this HtmlHelper helper, string key)
        {
            DatabaseContext db = new DatabaseContext();
            var content = db.TextItems.SingleOrDefault(x => x.Name.Equals(key));
            if (content == null)
            {
                return key;
            }
            else
            {
                return content.LinkUrl;
            }

        }
        public static string SiteContentUrlLinkTitle(this HtmlHelper helper, string key)
        {
            DatabaseContext db = new DatabaseContext();
            var content = db.TextItems.SingleOrDefault(x => x.Name.Equals(key));
            if (content == null)
            {
                return key;
            }
            else
            {
                return content.LinkTitle;
            }

        }
    }
}