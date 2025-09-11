using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomationAndIntegration.Data;
using AutomationAndIntegration.Models;

namespace AutomationAndIntegration.Services
{
    public class LoggerService
    {
        private readonly WebshopContext _db;

        public LoggerService(WebshopContext db)
        {
            _db = db;
        }

        public void Log(string eventType, string message, string? username = null)
        {
            var log = new AuditLog
            {
                EventType = eventType,
                Message = message,
                Username = username
            };

            _db.AuditLogs.Add(log);
            _db.SaveChanges();

            Console.WriteLine($"[LOG] {eventType} - {message} ({username})");
        }
    }
}
