using CSS_MagacinControl_App.Models.DboModels;
using System;
using System.Linq;

namespace CSS_MagacinControl_App.Migrations.DbInitialize
{
    public class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            if (!context.Users.Any())
            {
                context.Users.Add(new UserDbo(
                    id: Guid.NewGuid(),
                    username: "admin",
                    password: "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
                    name: "admin",
                    surname: "admin",
                    isAdmin: true
                ));
            }

            context.SaveChanges();
        }
    }
}