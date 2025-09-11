using AutomationAndIntegration.Data;
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
                        using (var db = new WebshopContext())
                        {
                            var user = auth.Login();
                            if (user != null)
                            {
                                if (user.Role == "Admin")
                                    ShowAdminMenu(user, db);
                                else
                                    ShowUserMenu(user, db);
                            }
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

        private static void ShowAdminMenu(User admin, WebshopContext db)
        {
            var adminService = new AdminService(db);

            bool inMenu = true;
            while (inMenu)
            {
                Console.WriteLine($"\nAdminpanel - {admin.Username}");
                Console.WriteLine("1. Lista användare");
                Console.WriteLine("2. Lista produkter");
                Console.WriteLine("3. Lägg till produkt");
                Console.WriteLine("4. Uppdatera lager");
                Console.WriteLine("5. Visa alla ordrar");
                Console.WriteLine("6. Uppdatera orderstatus");
                Console.WriteLine("7. Visa orderrapport");
                Console.WriteLine("8. Visa loggar");
                Console.WriteLine("0. Logga ut");
                Console.Write("Val: ");

                string? input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        adminService.ShowUsers();
                        break;

                    case "2":
                        adminService.ShowProducts();
                        break;

                    case "3":
                        adminService.AddProduct();
                        break;

                    case "4":
                        adminService.UpdateStock();
                        break;

                    case "5":
                        adminService.ShowAllOrders();
                        break;

                    case "6":
                        adminService.UpdateOrderStatus();
                        break;

                    case "7":
                        adminService.ShowAllOrdersReport();
                        break;
                    case "8":
                        adminService.ShowLogs();
                        break;

                    case "0":
                        inMenu = false;
                        break;

                    default:
                        Console.WriteLine("Felaktigt val.");
                        break;
                }
            }
        }

        private static void ShowUserMenu(User user, WebshopContext db)
        {
            var userService = new UserService(db);

            bool inMenu = true;
            while (inMenu)
            {
                Console.WriteLine($"\nWebshop - {user.Username}");
                Console.WriteLine("1. Visa produkter");
                Console.WriteLine("2. Skapa ny order");
                Console.WriteLine("3. Mina ordrar");
                Console.WriteLine("0. Logga ut");
                Console.Write("Val: ");

                string? input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        userService.ShowProducts();
                        break;

                    case "2":
                        userService.CreateOrder(user);
                        break;

                    case "3":
                        userService.ShowMyOrders(user);
                        break;

                    case "0":
                        inMenu = false;
                        break;

                    default:
                        Console.WriteLine("Felaktigt val.");
                        break;
                }
            }
        }
    }
}