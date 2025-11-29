using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManager.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        // Навигационные свойства
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
