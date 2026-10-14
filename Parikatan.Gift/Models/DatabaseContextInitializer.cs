using System;
 

namespace Parikatan.Gift.Models.Entities
{
    internal static class DatabaseContextInitializer
    {
        static DatabaseContextInitializer()
        {
        }

        internal static void Seed(DatabaseContext databaseContext)
        {
            InsertBaseUser(databaseContext);
            databaseContext.SaveChanges();
        }

        internal static void InsertBaseUser(DatabaseContext databaseContext)
        {
            User user = new User()
            {
                SecurityRole = "SuperAdmin",
                CellNum = "09124806404",
                Password = "zavosh327056",
                CreationDate = DateTime.Now,
                IsActive = true,
                IsDeleted = false,
                FullName = "admin",
            };

            databaseContext.Users.Add(user);
        }
    }
}
