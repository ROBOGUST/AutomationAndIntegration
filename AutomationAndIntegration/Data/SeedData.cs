using AutomationAndIntegration.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationAndIntegration.Data
{
    public static class SeedData
    {
        public static void Initialize(WebshopContext db)
        {
            db.Database.Migrate();

            if (!db.Users.Any(u => u.Username == "admin"))
            {
                db.Users.Add(new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"),
                    Role = "Admin"
                });
            }

            if (!db.Users.Any(u => u.Username == "user"))
            {
                db.Users.Add(new User
                {
                    Username = "user",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("user"),
                    Role = "User"
                });
            }

            if (!db.Products.Any())
            {
                db.Products.AddRange(
                    new Product { Name = "Warhammer Space Marines", Stock = 10, Price = 699 },
                    new Product { Name = "Warhammer Chaos Marines", Stock = 10, Price = 699 },
                    new Product { Name = "Citadel Paint Spray", Stock = 25, Price = 199 },
                    new Product { Name = "Citadel Paint Set", Stock = 10, Price = 499 },
                    new Product { Name = "Detail Brush", Stock = 15, Price = 89 }
                );
            }

            db.SaveChanges();
        }
    }
}
