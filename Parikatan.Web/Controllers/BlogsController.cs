using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Models;
using Models.Entities;
using ViewModels.Blogs;

namespace Parikatan.Web.Controllers
{

    [Authorize(Roles = "SuperAdmin,UltraSuperAdmin")]
    public class BlogsController : Controller
    {
        private DatabaseContext db = new DatabaseContext();

        public ActionResult Index()
        {
            var blogs = db.Blogs.Include(b => b.BlogGroup).Where(b => b.IsDeleted == false).OrderByDescending(b => b.CreationDate);
            return View(blogs.ToList());
        }


        public ActionResult Create()
        {
            ViewBag.BlogGroupId = new SelectList(db.BlogGroups, "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Blog blog, HttpPostedFileBase fileupload)
        {
            if (ModelState.IsValid)
            {
                #region Upload and resize image if needed
                string newFilenameUrl = string.Empty;
                if (fileupload != null)
                {
                    string filename = Path.GetFileName(fileupload.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    newFilenameUrl = "/Uploads/Blog/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                    blog.ImageUrl = newFilenameUrl;
                }


                #endregion

                blog.Visit = 0;
                blog.IsDeleted = false;
                blog.CreationDate = DateTime.Now;
                blog.UrlParam = GetUrlParam(blog.Title);


                db.Blogs.Add(blog);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BlogGroupId = new SelectList(db.BlogGroups, "Id", "Title", blog.BlogGroupId);
            return View(blog);
        }

        public string GetUrlParam(string title)
        {
            title = ReplaceCharachter(title, '@');
            title = ReplaceCharachter(title, '#');
            title = ReplaceCharachter(title, '$');
            title = ReplaceCharachter(title, '&');
            title = ReplaceCharachter(title, '^');
            title = ReplaceCharachter(title, '/');
            title = ReplaceCharachter(title, ']');
            title = ReplaceCharachter(title, '[');
            title = ReplaceCharachter(title, '%');
            title = ReplaceCharachter(title, '?');
            title = ReplaceCharachter(title, '؟');
            title = ReplaceCharachter(title, '!');

            return title.Replace(' ', '-');
        }

        public string ReplaceCharachter(string title, char charachter)
        {
            if (title.Contains(charachter))
                return title.Replace(charachter, '-');
            return title;
        }

        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            ViewBag.BlogGroupId = new SelectList(db.BlogGroups, "Id", "Title", blog.BlogGroupId);
            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Blog blog, HttpPostedFileBase fileupload)
        {
            if (ModelState.IsValid)
            {
                #region Upload and resize image if needed
                string newFilenameUrl = string.Empty;
                if (fileupload != null)
                {
                    string filename = Path.GetFileName(fileupload.FileName);
                    string newFilename = Guid.NewGuid().ToString().Replace("-", string.Empty)
                                         + Path.GetExtension(filename);

                    newFilenameUrl = "/Uploads/Blog/" + newFilename;
                    string physicalFilename = Server.MapPath(newFilenameUrl);
                    fileupload.SaveAs(physicalFilename);
                    blog.ImageUrl = newFilenameUrl;
                }


                #endregion
                blog.IsDeleted = false;
                blog.LastModifiedDate = DateTime.Now;
                db.Entry(blog).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BlogGroupId = new SelectList(db.BlogGroups, "Id", "Title", blog.BlogGroupId);
            return View(blog);
        }

        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Blog blog = db.Blogs.Find(id);
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Blog blog = db.Blogs.Find(id);
            blog.IsDeleted = true;
            blog.DeletionDate = DateTime.Now;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [Route("blog")]
        [AllowAnonymous]
        public ActionResult List()
        {
            List<Blog> blogs = db.Blogs.Include(c => c.BlogGroup).Where(c => c.IsDeleted == false && c.IsActive)
                .OrderByDescending(c => c.CreationDate).ToList();

            BlogListViewModel blogList = new BlogListViewModel()
            {
                Blogs = blogs,
                Groups = GroupsOf(blogs)
            };

            ViewBag.Title = "مجله پریکاتن | راهنمای مراقبت از پوست";
            ViewBag.Description = "مقاله‌های مجله پریکاتن: راهنمای مراقبت از پوست، شناخت مواد مؤثر و انتخاب درست محصول.";
            ViewBag.Canonical = "https://parikatan.shop/blog";
            ViewBag.NavActive = "blog";
            return View(blogList);
        }

        private static List<BlogGroup> GroupsOf(List<Blog> blogs)
        {
            return blogs.Where(b => b.BlogGroup != null && b.BlogGroup.IsActive && !b.BlogGroup.IsDeleted)
                .Select(b => b.BlogGroup).Distinct().OrderBy(g => g.Title).ToList();
        }


        [Route("blog/{urlParam}")]
        [AllowAnonymous]
        public ActionResult ListByGroup(string urlParam)
        {
            BlogGroup blogGroup = db.BlogGroups.FirstOrDefault(c => c.UrlParam == urlParam && c.IsActive);

            if (blogGroup == null)
                return Redirect("/blog");

            List<Blog> blogs = db.Blogs.Include(c => c.BlogGroup)
                .Where(c => c.BlogGroupId == blogGroup.Id && c.IsDeleted == false && c.IsActive)
                .OrderByDescending(c => c.CreationDate).ToList();

            var all = db.Blogs.Include(c => c.BlogGroup).Where(c => c.IsDeleted == false && c.IsActive).ToList();

            BlogListViewModel blogList = new BlogListViewModel()
            {
                Blogs = blogs,
                BlogGroup = blogGroup,
                Groups = GroupsOf(all)
            };

            ViewBag.Title = blogGroup.Title + " | مجله پریکاتن";
            ViewBag.Description = blogGroup.Summery;
            ViewBag.Canonical = "https://parikatan.shop/blog/" + blogGroup.UrlParam;
            ViewBag.NavActive = "blog";
            return View("List", blogList);
        }


        [Route("blog/post/{urlParam}")]
        [AllowAnonymous]
        public ActionResult Details(string urlParam)
        {

            Blog blog = db.Blogs.Include(c => c.BlogGroup)
                .FirstOrDefault(c => c.UrlParam == urlParam && c.IsActive && c.IsDeleted == false);
            if (blog == null)
            {
                return Redirect("/blog");
            }

            blog.Visit++;
            db.SaveChanges();



            BlogDetailViewModel detail = new BlogDetailViewModel()
            {
                Blog = blog,

                BlogComments = db.BlogComments.Where(c => c.BlogId == blog.Id && c.IsActive && c.IsDeleted == false).ToList(),

                SidebarBlogGroups = db.BlogGroups.Where(c => c.IsActive && c.IsDeleted == false).ToList(),

                RelatedBlogs = db.Blogs.Include(c => c.BlogGroup)
                    .Where(c => c.IsActive && c.IsDeleted == false && c.Id != blog.Id)
                    .OrderByDescending(c => c.BlogGroupId == blog.BlogGroupId)
                    .ThenByDescending(c => c.CreationDate).Take(3).ToList(),




            };

            if (string.IsNullOrEmpty(blog.PageTitle))
                ViewBag.Title = blog.Title + " | پریکاتن";
            else
                ViewBag.Title = blog.PageTitle;

            ViewBag.Description = !string.IsNullOrEmpty(blog.MetaDescription) ? blog.MetaDescription : blog.Summery;
            ViewBag.Canonical = "https://parikatan.shop/blog/post/" + blog.UrlParam;
            ViewBag.NavActive = "blog";

            return View(detail);
        }


        //[Route("blogs")]
        //[AllowAnonymous]
        //public ActionResult List()
        //{
        //    return View();
        //}


        //[Route("blogs/details")]
        //[AllowAnonymous]
        //public ActionResult Details()
        //{
        //    return View();
        //}
    }
}
