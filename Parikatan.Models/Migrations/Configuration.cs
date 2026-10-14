namespace Models.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Models.DatabaseContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            //ContextKey = "ToubaSources.Models.DatabaseContext";
        }

        protected override void Seed(Models.DatabaseContext databaseContext)
        {
            base.Seed(databaseContext);
            if (databaseContext.Users.Count() != 0)
            {
                return;
            }

            try
            {
                DatabaseContextInitializer.Seed(databaseContext);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
