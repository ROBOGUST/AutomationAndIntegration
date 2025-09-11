using AutomationAndIntegration.Data;
using AutomationAndIntegration.Models;
using OTSystem.Services;

namespace OTSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var db = new WebshopContext();
            var otService = new OTService(db);
            otService.StartMonitoring();
        }
    }
}
