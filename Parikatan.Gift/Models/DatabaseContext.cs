using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Parikatan.Gift.Models.Entities;
using Parikatan.Gift.Models.Entities.Promotion;


namespace Parikatan.Gift.Models
{
   public class DatabaseContext:DbContext
    {
        static DatabaseContext()
        {
             System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<DatabaseContext, Migrations.Configuration>());
        }

        public DbSet<User> Users { get; set; }
       
        public DbSet<PromotionCode> PromotionCodes { get; set; }

    }
}
