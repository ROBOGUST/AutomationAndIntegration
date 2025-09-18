using System;
using System.Linq;
using System.Threading;
using EasyModbus;
using AutomationAndIntegration.Data;
using AutomationAndIntegration.Models;

namespace IntegrationSystem.Services
{
    public class OrderIntegrationProcessor
    {
        private readonly string _ip;
        private readonly int _port;
        private ModbusClient? _client;

        public OrderIntegrationProcessor(string ip, int port)
        {
            _ip = ip; _port = port;
        }

        public void Run()
        {
            Console.WriteLine("[Integration] Integration service startad.");
            EnsureConnected();

            while (true)
            {
                try
                {
                    using var db = new WebshopContext();
                    var toSend = db.Orders.Where(o => o.Status == "Betald").OrderBy(o => o.CreatedAt).ToList();

                    foreach (var order in toSend)
                    {
                        if (db.IntegrationLogs.Any(l => l.OrderId == order.Id)) continue;

                        Console.WriteLine($"[Integration] Skickar order {order.Id} till OT...");
                        _client!.WriteSingleRegister(1, order.Id);

                        Thread.Sleep(200);
                        int[] back = _client.ReadHoldingRegisters(1, 2);
                        Console.WriteLine($"[Integration] Läs tillbaka från OT: orderId={back[0]}, status={back[1]}");

                        db.IntegrationLogs.Add(new IntegrationLog { OrderId = order.Id, Note = "Sent to OT" });
                        order.Status = "Skickas av OT";
                        db.SaveChanges();

                        PollOTForCompletion(order.Id, db, timeoutSeconds: 300);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Integration] Fel i huvudslinga: " + ex.Message);
                    _client = null;
                    EnsureConnected();
                }

                Thread.Sleep(3000);
            }
        }

        private void EnsureConnected()
        {
            if (_client != null && _client.Connected) return;
            _client = new ModbusClient(_ip, _port);
            _client.Connect();
            Console.WriteLine("[Integration] Ansluten till OT.");
        }

        private void PollOTForCompletion(int orderId, WebshopContext db, int timeoutSeconds = 120)
        {
            Console.WriteLine($"[Integration] Pollar OT för order {orderId}...");
            var sw = System.Diagnostics.Stopwatch.StartNew();
            while (sw.Elapsed.TotalSeconds < timeoutSeconds)
            {
                Thread.Sleep(2000);
                try
                {
                    int[] regs = _client!.ReadHoldingRegisters(2, 1);
                    int status = regs[0];
                    string label = status switch
                    {
                        0 => "Ej betald/Idle",
                        1 => "Mottagen",
                        2 => "Packas",
                        3 => "Skickad",
                        4 => "Klar",
                        _ => "Okänd"
                    };

                    Console.WriteLine($"[Integration] OT status för order {orderId}: {status} ({label})");

                    if (status == 2) UpdateOrderStatus(db, orderId, "Dina varor packas");
                    else if (status == 3) UpdateOrderStatus(db, orderId, "Ditt paket är nu skickat");
                    else if (status == 4)
                    {
                        UpdateOrderStatus(db, orderId, "Klar");
                        Console.WriteLine($"[Integration] Order {orderId} färdigbehandlad av OT.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[Integration] Poll-fel: " + ex.Message);
                    _client = null;
                    EnsureConnected();
                    return;
                }
            }

            Console.WriteLine($"[Integration] Timeout polling för order {orderId} efter {timeoutSeconds}s.");
        }

        private void UpdateOrderStatus(WebshopContext db, int orderId, string newStatus)
        {
            var order = db.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null) return;
            order.Status = newStatus;
            db.SaveChanges();
            Console.WriteLine($"[Integration] Order {orderId} uppdaterad i IT: {newStatus}");
        }
    }
}
