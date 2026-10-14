using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Parikatan.QrForm.Models.Entities;


namespace Parikatan.QrForm.Models
{
   public class DatabaseContext:DbContext
    {
        static DatabaseContext()
        {
             System.Data.Entity.Database.SetInitializer(new MigrateDatabaseToLatestVersion<DatabaseContext, Migrations.Configuration>());
        }

        public DbSet<User> Users { get; set; }
       
        public DbSet<NewsLetter> NewsLetters { get; set; }
        public DbSet<UserVisit> UserVisits { get; set; }

    }
}
