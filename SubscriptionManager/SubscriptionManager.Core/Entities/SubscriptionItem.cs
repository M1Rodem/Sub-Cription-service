using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManager.Core.Entities
{
    public class SubscriptionItem
    {
        public Guid Id { get; set; }

        // Внешние ключи
        public Guid PlaceId { get; set; }
        public Guid SubscriptionId { get; set; }

        // Навигационные свойства
        public Place Place { get; set; } = null!;
        public Subscription Subscription { get; set; } = null!;
    }
}
