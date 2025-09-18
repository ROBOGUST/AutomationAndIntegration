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
            using var db = new WebshopContext();
            try
            {
                Console.WriteLine("Using DB: " + db.Database.GetDbConnection().DataSource);
                SeedData.Initialize(db);
            }
            catch (Exception ex)
            {
                Console.WriteLine("WARN: Databasinit misslyckades: " + ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

            var logger = new LoggerService(db);
            var authService = new AuthService(db, logger);
            var userService = new UserService(db);
            var adminService = new AdminService(db);
            MenuHelper.ShowMainMenu(authService);

        }
    }
}
