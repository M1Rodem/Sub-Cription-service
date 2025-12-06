using System;
using System.Collections.Generic;
using System.Text;

namespace SubscriptionManager.Shared
{
    public class DateTimeProvider
    {
        private DateTime? _dateTime = null;
        public DateTime UtcNow => _dateTime ?? DateTime.UtcNow;

        public void Set(DateTime dateTime)
        {
            _dateTime = dateTime;
        }

        public void Reset()
        {
            _dateTime = null;
        }
    }
}
