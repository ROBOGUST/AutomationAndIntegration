using AutomationAndIntegration.Data;
using AutomationAndIntegration.Helpers;
using AutomationAndIntegration.Models;
using AutomationAndIntegration.Services;
using Microsoft.EntityFrameworkCore;

namespace AutomationAndIntegration
{

    /*
        TODO: 
    Add real DB
    Add OT system to adjust stock automatically and increase stock when low
    Add real payment gateways
    Add email service to send order confirmations
    Add logging
    Add workflow engine (OT) for order processing after confirmed payment
     */

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
