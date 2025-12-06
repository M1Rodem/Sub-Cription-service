using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SubscriptionManager.Core.Entities;

namespace SubscriptionManager.Core.Interfaces;

public interface ISubscriptionRepository
{
    Task<Subscription?> GetByIdAsync(Guid id);
    Task<IEnumerable<Subscription>> GetAllAsync();
    Task<IEnumerable<Subscription>> GetByUserIdAsync(Guid userId);
    Task<Subscription> AddAsync(Subscription subscription);
    Task<bool> UpdateAsync(Subscription subscription);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}