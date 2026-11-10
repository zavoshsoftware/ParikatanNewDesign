using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Models.Entities;

namespace Models
{
   public class DatabaseContext:DbContext
    {
        static DatabaseContext()
        {
             System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<DatabaseContext, Migrations.Configuration>());
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<BlogGroup> BlogGroups { get; set; }
        public DbSet<BlogComment> BlogComments { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<TextItem> TextItems { get; set; }
        public DbSet<ContactUsForm> ContactUsForms { get; set; }
        public DbSet<TextItemType> TextItemTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductFaq> ProductFaqs { get; set; }
        public DbSet<DiscountCode> DiscountCodes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ProductComment> ProductComments { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Province> Provinces { get; set; }
        public DbSet<ZarinpallAuthority> ZarinpallAuthorities { get; set; }
        public DbSet<PaymentType> PaymentTypes { get; set; }
        public DbSet<NewsLetter> NewsLetters { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryDetail> InventoryDetails { get; set; }
        public DbSet<ProductPackageItem> ProductPackageItemss { get; set; }
        public DbSet<Cost> Costs { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Pharmacy> Pharmacies { get; set; }

        public System.Data.Entity.DbSet<Models.Entities.Referral> Referrals { get; set; }
    }
}
