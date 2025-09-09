using AutomationAndIntegration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationAndIntegration.Services         // TODO: Implement real payment gateways
{
    public class PaymentService
    {
        // Mocked Klarna-payment
        public void ProcessKlarnaPayment(Order order)
        {
            Console.WriteLine("(Mock) Skickar betalningsförfrågan till Klarna...");
            System.Threading.Thread.Sleep(1000);
            order.Status = "Betald";
            Console.WriteLine($"Order {order.Id} markerad som betald via Klarna.");
        }

        // Mocked Swish-payment
        public void ProcessSwishPayment(Order order)
        {
            Console.WriteLine("(Mock) Skickar betalningsförfrågan till Swish...");
            System.Threading.Thread.Sleep(1000);
            order.Status = "Betald";
            Console.WriteLine($"Order {order.Id} markerad som betald via Swish.");
        }
    }
}
