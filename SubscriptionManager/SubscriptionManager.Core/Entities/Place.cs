using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManager.Core.Entities
{
    public class Place
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Навигационные свойства
        public ICollection<SubscriptionItem> SubscriptionItems { get; set; } = new List<SubscriptionItem>();
        public ICollection<History> Histories { get; set; } = new List<History>();
        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    }
}
