using AutomationAndIntegration.Models;
using AutomationAndIntegration.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationAndIntegration.Helpers
{
    public static class MenuHelper
    {
        public static void ShowMainMenu(AuthService auth)
        {
            bool running = true;
            while (running)
            {
                Console.WriteLine("\n=== WEBBSHOP MENU ===");
                Console.WriteLine("1. Logga in");
                Console.WriteLine("2. Registrera konto");
                Console.WriteLine("0. Avsluta");
                Console.Write("Val: ");

                string? input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        var user = auth.Login();
                        if (user != null)
                        {
                            if (user.Role == "Admin")
                                ShowAdminMenu(user);
                            else
                                ShowUserMenu(user);
                        }
                        break;

                    case "2":
                        auth.Register();
                        break;

                    case "0":
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Felaktigt val.");
                        break;
                }
            }
        }

        private static void ShowAdminMenu(User admin)
        {
            Console.WriteLine($"\nAdminpanel ({admin.Username})");
            Console.WriteLine("Här kan vi senare bygga admin-funktioner.");
        }

        private static void ShowUserMenu(User user)
        {
            Console.WriteLine($"\nWebshop ({user.Username})");
            Console.WriteLine("Här kan vi senare bygga beställningsflöde.");
        }
    }
}
