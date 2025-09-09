using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomationAndIntegration.Data;
using AutomationAndIntegration.Models;

namespace AutomationAndIntegration.Services
{
    public class UserService
    {
        private readonly WebshopContext _db;

        public UserService(WebshopContext db)
        {
            _db = db;
        }

        public void ShowProducts()
        {
            Console.WriteLine("\nTillgängliga produkter:");
            foreach (var p in _db.Products.ToList())
            {
                Console.WriteLine($"{p.Id}. {p.Name} ({p.Stock} st) - {p.Price} kr");
            }
        }

        public void CreateOrder(User user)
        {
            ShowProducts();
            Console.Write("\nAnge produkt-ID: ");
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

            Console.Write("Ange antal: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
            {
                Console.WriteLine("Felaktigt antal.");
                return;
            }

            double total = product.Price * quantity;

            var order = new Order
            {
                UserId = user.Id,
                Status = "Ej betald",
                TotalAmount = total
            };

            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                Quantity = quantity,
                Order = order
            };

            order.Items.Add(orderItem);
            _db.Orders.Add(order);
            _db.SaveChanges();

            Console.WriteLine($"Order skapad (ID: {order.Id}, Total: {total} kr). Status: {order.Status}");

            var paymentService = new PaymentService();

            Console.WriteLine("\nVälj betalningsmetod:");
            Console.WriteLine("1. Klarna");
            Console.WriteLine("2. Swish");
            Console.WriteLine("3. Avbryt (lämna som 'Ej betald')");
            Console.Write("Val: ");

            string? choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    paymentService.ProcessKlarnaPayment(order);
                    break;
                case "2":
                    paymentService.ProcessSwishPayment(order);
                    break;
                case "3":
                    Console.WriteLine("Ordern sparas som 'Ej betald'.");
                    break;
                default:
                    Console.WriteLine("Ogiltigt val, ordern sparas som 'Ej betald'.");
                    break;
            }

            _db.SaveChanges();
        }

        public void ShowMyOrders(User user)
        {
            Console.WriteLine($"\nOrdrar för {user.Username}:");
            var orders = _db.Orders
                            .Where(o => o.UserId == user.Id)
                            .OrderByDescending(o => o.CreatedAt)
                            .ToList();

            if (!orders.Any())
            {
                Console.WriteLine("Inga ordrar hittades.");
                return;
            }

            foreach (var o in orders)
            {
                Console.WriteLine($" - Order {o.Id}: {o.Status}, Total: {o.TotalAmount} kr, Skapad: {o.CreatedAt}");
            }
        }
    }
}
