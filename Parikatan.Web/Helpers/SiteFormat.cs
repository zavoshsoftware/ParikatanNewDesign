using System;
using System.Configuration;
using System.Text.RegularExpressions;
using Models.Entities;

namespace Helpers
{
    /// <summary>Small formatting helpers shared by the public-site views.</summary>
    public static class SiteFormat
    {
        private static readonly string BaseUrl = (ConfigurationManager.AppSettings["baseUrl"] ?? string.Empty).TrimEnd('/');

        /// <summary>Versioned url for a static file (cache-busts on change): ~/Assets/site/site.css -> /Assets/site/site.css?v=ticks</summary>
        public static string Asset(string virtualPath)
        {
            string url = System.Web.VirtualPathUtility.ToAbsolute(virtualPath);
            try
            {
                string physical = System.Web.Hosting.HostingEnvironment.MapPath(virtualPath);
                if (physical != null && System.IO.File.Exists(physical))
                    return url + "?v=" + System.IO.File.GetLastWriteTimeUtc(physical).Ticks;
            }
            catch (Exception) { }
            return url;
        }

        /// <summary>
        /// Gives every h2 of an article body an id (for the table of contents) and returns the
        /// (id, text) list. Existing ids are kept.
        /// </summary>
        public static string AddHeadingIds(string html, out System.Collections.Generic.List<string[]> toc)
        {
            var list = new System.Collections.Generic.List<string[]>();
            if (string.IsNullOrEmpty(html))
            {
                toc = list;
                return html;
            }

            int n = 0;
            string result = Regex.Replace(html, @"<h2(\s[^>]*)?>(.*?)</h2>", m =>
            {
                n++;
                string attrs = m.Groups[1].Value;
                string inner = m.Groups[2].Value;
                string text = PlainText(inner, 200);
                var idMatch = Regex.Match(attrs, @"\sid\s*=\s*""([^""]+)""");
                string id = idMatch.Success ? idMatch.Groups[1].Value : "s" + n;
                if (!idMatch.Success)
                    attrs += " id=\"" + id + "\"";
                if (text.Length > 0)
                    list.Add(new[] { id, text });
                return "<h2" + attrs + ">" + inner + "</h2>";
            }, RegexOptions.Singleline | RegexOptions.IgnoreCase);

            toc = list;
            return result;
        }

        /// <summary>Non-empty trimmed lines of a multi-line admin field.</summary>
        public static System.Collections.Generic.List<string> Lines(string text)
        {
            var result = new System.Collections.Generic.List<string>();
            if (string.IsNullOrWhiteSpace(text))
                return result;

            foreach (var line in text.Replace("\r", "").Split('\n'))
            {
                var t = line.Trim();
                if (t.Length > 0)
                    result.Add(t);
            }
            return result;
        }

        /// <summary>Lines of "title|text" pairs (text may be empty).</summary>
        public static System.Collections.Generic.List<string[]> Pairs(string text)
        {
            var result = new System.Collections.Generic.List<string[]>();
            foreach (var line in Lines(text))
            {
                int i = line.IndexOf('|');
                result.Add(i < 0
                    ? new[] { line, string.Empty }
                    : new[] { line.Substring(0, i).Trim(), line.Substring(i + 1).Trim() });
            }
            return result;
        }

        public static string Digits(object value)
        {
            if (value == null)
                return string.Empty;

            var chars = value.ToString().ToCharArray();
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] >= '0' && chars[i] <= '9')
                    chars[i] = (char)('۰' + (chars[i] - '0'));
            }
            return new string(chars);
        }

        /// <summary>1680000 -> ۱٬۶۸۰٬۰۰۰ تومان</summary>
        public static string Price(decimal amount)
        {
            return Digits(Math.Round(amount).ToString("n0", System.Globalization.CultureInfo.InvariantCulture).Replace(",", "٬")) + " تومان";
        }

        public static decimal FinalAmount(Product product)
        {
            return product.IsInPromotion && product.DiscountAmount.HasValue ? product.DiscountAmount.Value : product.Amount;
        }

        public static bool CanBuy(Product product)
        {
            return product.IsAvailable && product.Stock > 0;
        }

        /// <summary>Uploaded images are stored as relative urls; the shop serves them from baseUrl (same as the older views).</summary>
        public static string Img(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return string.Empty;
            if (url.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return url;
            return BaseUrl + (url.StartsWith("/") ? url : "/" + url);
        }

        public static string PlainText(string html, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(html))
                return string.Empty;

            string text = Regex.Replace(html, "<.*?>", " ");
            text = System.Net.WebUtility.HtmlDecode(text);
            text = Regex.Replace(text, @"\s+", " ").Trim();
            if (text.Length <= maxLength)
                return text;

            int cut = text.LastIndexOf(' ', maxLength);
            return text.Substring(0, cut > maxLength / 2 ? cut : maxLength).TrimEnd() + "…";
        }

        /// <summary>Rough reading time, ~180 words per minute, at least one minute.</summary>
        public static int ReadMinutes(string html)
        {
            string text = PlainText(html, int.MaxValue);
            int words = string.IsNullOrEmpty(text) ? 0 : text.Split(' ').Length;
            return Math.Max(1, (int)Math.Ceiling(words / 180.0));
        }
    }
}
