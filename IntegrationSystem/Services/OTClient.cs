using System;
using System.Linq;
using System.Threading;
using EasyModbus;
using AutomationAndIntegration.Data;
using Microsoft.EntityFrameworkCore;

namespace IntegrationSystem.Services
{
    public class OTClient
    {
        private readonly string _ip;
        private readonly int _port;

        public OTClient(string ip = "127.0.0.1", int port = 502)
        {
            _ip = ip;
            _port = port;
        }

        public void RunOrderFlow(int orderId)
        {
            using var db = new WebshopContext();
            var order = db.Orders.Include(o => o.Items).FirstOrDefault(o => o.Id == orderId);

            if (order == null)
            {
                Console.WriteLine($"[Integration] Order {orderId} finns inte i databasen.");
                return;
            }

            var client = new ModbusClient(_ip, _port);
            client.Connect();

            Console.WriteLine("[Integration] Ansluten till OT.");
            Console.WriteLine($"[Integration] Skickar order {order.Id} ({order.Status})...");

           int statusCode = MapStatus(order.Status);

            client.WriteMultipleRegisters(1, new int[] { order.Id, statusCode });

            Console.WriteLine($"[Integration] Pollar OT för order {order.Id}...");

            int currentStatus = -1;
            bool orderComplete = false;

            string[] labels = { "Ej betald", "Betald", "Packas", "Skickad", "Klar" };

            while (!orderComplete)
            {
                Thread.Sleep(3000);

                int[] regs = client.ReadHoldingRegisters(1, 2);
                int polledOrderId = regs[0];
                int polledStatus = regs[1];

                if (polledStatus != currentStatus)
                {
                    currentStatus = polledStatus;
                    string label = polledStatus >= 0 && polledStatus < labels.Length
                        ? labels[polledStatus]
                        : $"Okänd status {polledStatus}";

                    Console.WriteLine($"[Integration] OT status för order {orderId}: {polledStatus} ({label})");

                    order.Status = label;
                    db.SaveChanges();

                    if (polledStatus == 4) orderComplete = true;
                }
            }

            Console.WriteLine($"[Integration] Order {orderId} klar!");
            client.Disconnect();
        }

        private int MapStatus(string status)
        {
            return status switch
            {
                "Ej betald" => 0,
                "Betald" => 1,
                "Mottagen" => 1,
                "Packas" => 2,
                "Skickad" => 3,
                "Klar" => 4,
                _ => 0
            };
        }
    }
}
