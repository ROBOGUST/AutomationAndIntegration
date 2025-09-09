using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomationAndIntegration.Data;
using AutomationAndIntegration.Models;

namespace AutomationAndIntegration.Services
{
    public class AdminService
    {
        private readonly WebshopContext _db;

        public AdminService(WebshopContext db)
        {
            _db = db;
        }

        public void ShowUsers()
        {
            Console.WriteLine("\nAnvändare i systemet:");
            foreach (var u in _db.Users.ToList())
            {
                Console.WriteLine($"- {u.Id}: {u.Username} ({u.Role})");
            }
        }

        public void ShowProducts()
        {
            Console.WriteLine("\nProdukter i lagret:");
            foreach (var p in _db.Products.ToList())
            {
                Console.WriteLine($"- {p.Id}: {p.Name} ({p.Stock} st) - {p.Price} kr");
            }
        }

        public void AddProduct()
        {
            Console.Write("Produktnamn: ");
            string name = Console.ReadLine() ?? "";

            Console.Write("Pris: ");
            if (!double.TryParse(Console.ReadLine(), out double price))
            {
                Console.WriteLine("Felaktigt pris.");
                return;
            }

            Console.Write("Lagerantal: ");
            if (!int.TryParse(Console.ReadLine(), out int stock))
            {
                Console.WriteLine("Felaktigt antal.");
                return;
            }

            var product = new Product
            {
                Name = name,
                Price = price,
                Stock = stock
            };

            _db.Products.Add(product);
            _db.SaveChanges();

            Console.WriteLine($"Produkt {name} tillagd!");
        }

        public void UpdateStock()
        {
            ShowProducts();
            Console.Write("\nAnge produkt-ID att uppdatera: ");
            if (!int.TryParse(Console.ReadLine(), out int productId))
            {
                Console.WriteLine("Felaktigt ID.");
                return;
            }

            var product = _db.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                Console.WriteLine("Produkten finns inte.");
                return;
            }

            Console.Write("Nytt lagerantal: ");
            if (!int.TryParse(Console.ReadLine(), out int stock))
            {
                Console.WriteLine("Felaktigt antal.");
                return;
            }

            product.Stock = stock;
            _db.SaveChanges();

            Console.WriteLine($"Lager för {product.Name} uppdaterat till {stock} st.");
        }

        public void ShowAllOrders()
        {
            Console.WriteLine("\nAlla ordrar i systemet:");
            var orders = _db.Orders.OrderByDescending(o => o.CreatedAt).ToList();

            if (!orders.Any())
            {
                Console.WriteLine("Inga ordrar finns.");
                return;
            }

            foreach (var o in orders)
            {
                Console.WriteLine($"Order {o.Id}: {o.Status}, Total: {o.TotalAmount} kr, UserId: {o.UserId}, Skapad: {o.CreatedAt}");
            }
        }

        public void UpdateOrderStatus()
        {
            ShowAllOrders();
            Console.Write("\nAnge order-ID att uppdatera: ");
            if (!int.TryParse(Console.ReadLine(), out int orderId))
            {
                Console.WriteLine("Felaktigt ID.");
                return;
            }

            var order = _db.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                Console.WriteLine("Ordern finns inte.");
                return;
            }

            Console.WriteLine("\nVälj ny status:");
            Console.WriteLine("1. Ej betald");
            Console.WriteLine("2. Betald");
            Console.WriteLine("3. Skickad");
            Console.WriteLine("4. Klar");
            Console.Write("Val: ");

            string? choice = Console.ReadLine();
            string newStatus = "";

            switch (choice)
            {
                case "1":
                    newStatus = "Ej betald";
                    break;
                case "2":
                    newStatus = "Betald";
                    break;
                case "3":
                    newStatus = "Skickad";
                    break;
                case "4":
                    newStatus = "Klar";
                    break;
                default:
                    Console.WriteLine("Ogiltigt val, status ändras inte.");
                    return;
            }

            order.Status = newStatus;
            _db.SaveChanges();

            Console.WriteLine($"Order {order.Id} uppdaterad till '{newStatus}'.");
        }
    }
}
