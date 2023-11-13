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
                    password: "admin",
                    name: "admin",
                    surname: "admin",
                    isAdmin: true
                ));
            }

            context.SaveChanges();
        }
    }
}