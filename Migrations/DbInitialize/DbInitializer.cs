using CSS_MagacinControl_App.Models.DboModels;
using Microsoft.EntityFrameworkCore;
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

        public async static void DeleteUnusedBarcodeIdentRelations(AppDbContext context)
        {
            await context.Database.ExecuteSqlAsync($@"
                delete _css_IdentBarKod
                where SifraIdenta not in
                (SELECT distinct kod.SifraIdenta
                  FROM _css_IdentBarKod kod, _css_RobaZaPakovanje_item stavka, _css_RobaZaPakovanje_hd zaglavlje
                  WHERE kod.SifraIdenta = stavka.SifraIdenta
                  AND stavka.BrojFakture = zaglavlje.BrojFakture 
                  AND zaglavlje.StatusFakture = 'U radu')
            ");
        }
    }
}