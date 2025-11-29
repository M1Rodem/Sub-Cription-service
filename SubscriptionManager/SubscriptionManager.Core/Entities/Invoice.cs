using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManager.Core.Entities
{
    public class Invoice
    {
        public Guid Id { get; set; }
        public DateTime CreateAt { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }

        // Внешние ключи
        public Guid UserId { get; set; }

        // Навигационные свойства
        public User User { get; set; } = null!;
        public ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    }
}
