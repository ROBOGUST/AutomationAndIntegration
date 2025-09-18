using System;
using IntegrationSystem.Services;
using AutomationAndIntegration.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IntegrationSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[Integration] Integration service startad.");

            using var db = new WebshopContext();
            var order = db.Orders.FirstOrDefault(o => o.Status == "Betald");

            if (order == null)
            {
                Console.WriteLine("[Integration] Ingen 'Betald' order hittades.");
                return;
            }

            var otClient = new OTClient();
            otClient.RunOrderFlow(order.Id);
        }
    }
}
