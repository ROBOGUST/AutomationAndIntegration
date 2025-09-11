using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using AutomationAndIntegration.Data;
using AutomationAndIntegration.Models;

namespace OTSystem.Services
{
    public class OTService
    {
        private readonly WebshopContext _db;
        private readonly int _restockThreshold;
        private readonly int _restockAmount;

        public OTService(WebshopContext db, int restockThreshold = 5, int restockAmount = 20)
        {
            _db = db;
            _restockThreshold = restockThreshold;
            _restockAmount = restockAmount;
        }

        public void StartMonitoring()
        {
            Console.WriteLine("OT-systemet startat. Lager övervakas...");
            while (true)
            {
                ProcessOrders();
                Thread.Sleep(5000);
            }
        }

        private void ProcessOrders()
        {
            var newOrders = _db.Orders
                               .Where(o => o.Status == "Ej betald" || o.Status.StartsWith("Väntar"))
                               .OrderBy(o => o.CreatedAt)
                               .ToList();

            foreach (var order in newOrders)
            {
                if (!CanFulfill(order))
                {
                    order.Status = "Väntar på lager (10s)";
                    _db.SaveChanges();
                    Console.WriteLine($"[OT] Order {order.Id} väntar på lager. Restock sker om 10s...");

                    Thread.Sleep(10000);

                    ForceRestock(order);
                    continue;
                }

                SimulateOrderFlow(order);
            }
        }

        private bool CanFulfill(Order order)
        {
            foreach (var item in order.Items)
            {
                var product = _db.Products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null || product.Stock < item.Quantity)
                    return false;
            }
            return true;
        }

        private void ForceRestock(Order order)
        {
            foreach (var item in order.Items)
            {
                var product = _db.Products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product != null && product.Stock < item.Quantity)
                {
                    int needed = item.Quantity - product.Stock;
                    int restock = Math.Max(_restockAmount, needed);
                    product.Stock += restock;
                    Console.WriteLine($"[OT] Produkt '{product.Name}' restockad med {restock}. Nytt lager: {product.Stock}");
                }
            }

            _db.SaveChanges();
            Console.WriteLine($"[OT] Restock klar. Order {order.Id} kan nu processas.");

            SimulateOrderFlow(order);
        }

        private void SimulateOrderFlow(Order order)
        {
            UpdateOrderStatus(order, "Order mottagen");
            Thread.Sleep(5000);

            UpdateOrderStatus(order, "Dina varor packas");
            Thread.Sleep(5000);

            foreach (var item in order.Items)
            {
                var product = _db.Products.First(p => p.Id == item.ProductId);
                product.Stock -= item.Quantity;
            }
            _db.SaveChanges();

            UpdateOrderStatus(order, "Ditt paket är nu skickat");
            Thread.Sleep(5000);

            UpdateOrderStatus(order, "Klar");
        }

        private void UpdateOrderStatus(Order order, string newStatus)
        {
            order.Status = newStatus;
            _db.SaveChanges();

            var user = _db.Users.FirstOrDefault(u => u.Id == order.UserId);
            string userMsg = user != null ? $"Till {user.Username}" : "Till okänd användare";

            Console.WriteLine($"[OT] {userMsg}: Order {order.Id} → {newStatus}");
        }
    }
}
