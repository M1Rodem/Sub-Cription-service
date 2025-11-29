using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManager.Core.Entities
{
    public class InvoiceItem
    {
        public Guid Id { get; set; }
        public decimal Price { get; set; }

        // Внешние ключи
        public Guid ApplicationId { get; set; }
        public Guid PlaceId { get; set; }
        public Guid InvoiceId { get; set; }

        // Навигационные свойства
        public Application Application { get; set; } = null!;
        public Place Place { get; set; } = null!;
        public Invoice Invoice { get; set; } = null!;
    }
}
