using System;
using System.Threading;
using EasyModbus;
using OTSystem.Services;

namespace OTSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var server = new OTService();
            server.Start();

            Console.WriteLine("OT-system (Modbus Server) startat på port 502...");
            Console.WriteLine("Register 0 = Lager, Register 1 = OrderId, Register 2 = Status");
            Console.WriteLine("Tryck på valfri tangent för att avsluta.");
            Console.ReadKey();
        }
    }
}
