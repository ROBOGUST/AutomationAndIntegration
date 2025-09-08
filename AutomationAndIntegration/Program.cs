using AutomationAndIntegration.Data;
using AutomationAndIntegration.Helpers;
using AutomationAndIntegration.Models;
using AutomationAndIntegration.Services;
using Microsoft.EntityFrameworkCore;

namespace AutomationAndIntegration
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var db = new WebshopContext())
            {
                db.Database.Migrate();
                SeedData.Initialize(db);

                var authService = new AuthService(db);

                MenuHelper.ShowMainMenu(authService);
            }
        }
    }
}
