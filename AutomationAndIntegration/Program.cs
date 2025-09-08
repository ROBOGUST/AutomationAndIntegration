using AutomationAndIntegration.Data;
using AutomationAndIntegration.Models;
using Microsoft.EntityFrameworkCore;

namespace AutomationAndIntegration
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var db = new WebshopContext())
            {
                db.Database.Migrate(); // Create DB if it does not exist
                SeedData.Initialize(db);

                Console.WriteLine("Databasen är initierad!");
                Console.WriteLine("Users i systemet:");
                foreach (var user in db.Users)
                {
                    Console.WriteLine($" - {user.Username} ({user.Role})");
                }

                Console.WriteLine("\nProdukter i lager:");
                foreach (var product in db.Products)
                {
                    Console.WriteLine($" - {product.Name} ({product.Stock} st) {product.Price} kr");
                }

                // TODO: login & register
            }


        }
    }
}
