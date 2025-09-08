using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AutomationAndIntegration.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public string Status { get; set; } = "Ej betald";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public double TotalAmount { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }
}
