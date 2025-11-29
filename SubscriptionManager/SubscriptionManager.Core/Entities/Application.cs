using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManager.Core.Entities
{
    public class Application
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }

        // Навигационные свойства
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
        public ICollection<History> Histories { get; set; } = new List<History>();
        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    }
}
