using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationAndIntegration.Models
{
    public class IntegrationLog
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public string Note { get; set; } = "";
    }
}
