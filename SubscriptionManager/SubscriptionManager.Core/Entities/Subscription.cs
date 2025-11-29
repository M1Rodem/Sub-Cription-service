using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManager.Core.Entities
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int Status { get; set; } // 0-Активная, 1-Отмененная, 2-Заблокированная

        // Внешние ключи
        public Guid UserId { get; set; }
        public Guid ApplicationId { get; set; }

        // Навигационные свойства
        public User User { get; set; } = null!;
        public Application Application { get; set; } = null!;
        public ICollection<SubscriptionItem> SubscriptionItems { get; set; } = new List<SubscriptionItem>();
        public ICollection<History> Histories { get; set; } = new List<History>();
    }
}
