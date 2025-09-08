using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using AutomationAndIntegration.Models;

namespace AutomationAndIntegration.Data
{
    public static class SeedData
    {
        public static void Initialize(WebshopContext db)
        {
            // Admin for testing
            if (!db.Users.Any(u => u.Username == "admin"))
            {
                db.Users.Add(new User
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"),
                    Role = "Admin"
                });
            }

            // User for testing
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
                    new Product { Name = "Warhammer Space Marines", Stock = 10, Price = 399 },
                    new Product { Name = "Citadel Paint Black", Stock = 25, Price = 45 },
                    new Product { Name = "Detail Brush", Stock = 15, Price = 89 }
                );
            }

            db.SaveChanges();
        }
    }
}
