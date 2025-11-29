using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManager.Core.Entities
{
    public class History
    {
        public Guid Id { get; set; }
        public DateOnly Start { get; set; }
        public DateOnly? End { get; set; }

        // Внешние ключи
        public Guid PlaceId { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid SubscriptionId { get; set; }

        // Навигационные свойства
        public Place Place { get; set; } = null!;
        public Application Application { get; set; } = null!;
        public Subscription Subscription { get; set; } = null!;
    }
}
