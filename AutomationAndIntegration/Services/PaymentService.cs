using AutomationAndIntegration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AutomationAndIntegration.Services         // TODO: Implement real payment gateways
{
    public class PaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly string _klarnaUsername; // Merchant ID
        private readonly string _klarnaPassword; // Shared Secret

        public PaymentService(string klarnaUsername, string klarnaPassword)
        {
            _httpClient = new HttpClient();
            _klarnaUsername = klarnaUsername;
            _klarnaPassword = klarnaPassword;

            var byteArray = Encoding.ASCII.GetBytes($"{_klarnaUsername}:{_klarnaPassword}");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            _httpClient.BaseAddress = new Uri("https://api.playground.klarna.com/");
        }

        public async Task<bool> ProcessKlarnaPaymentAsync(Order order)
        {
            Console.WriteLine("Skickar betalning till Klarna...");

            var requestBody = new
            {
                purchase_country = "SE",
                purchase_currency = "SEK",
                locale = "sv-SE",
                order_amount = (int)(order.TotalAmount * 100),
                order_tax_amount = 0,
                order_lines = new[]
                {
                    new {
                        type = "physical",
                        name = "Warhammer-produkter",
                        quantity = 1,
                        unit_price = (int)(order.TotalAmount * 100),
                        total_amount = (int)(order.TotalAmount * 100),
                        total_tax_amount = 0
                    }
                },
                merchant_urls = new
                {
                    confirmation = "https://example.com/confirmation",
                    checkout = "https://example.com/checkout"
                }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("checkout/v3/orders", content);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Klarna-order skapad:");
                Console.WriteLine(jsonResponse);

                order.Status = "Betald (Klarna sandbox)";
                return true;
            }
            else
            {
                Console.WriteLine("Klarna-betalning misslyckades:");
                Console.WriteLine(await response.Content.ReadAsStringAsync());
                order.Status = "Betalning misslyckades";
                return false;
            }
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
