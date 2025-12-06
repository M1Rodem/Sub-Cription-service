using SubscriptionManager.Shared.DTOs;

namespace SubscriptionManager.Core.Interfaces;

public interface IBffSubscriptionService
{
    Task<IEnumerable<BffSubscriptionDto>> GetBffSubscriptionsAsync(Guid userId);
    Task<BffSubscriptionDto?> GetBffSubscriptionByIdAsync(Guid id, Guid userId);
}