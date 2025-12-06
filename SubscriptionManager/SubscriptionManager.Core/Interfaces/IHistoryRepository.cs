using SubscriptionManager.Core.Entities;

namespace SubscriptionManager.Core.Interfaces;

public interface IHistoryRepository
{
    Task<History?> GetActiveBySubscriptionAndPlaceAsync(Guid subscriptionId, Guid placeId);
    Task<IEnumerable<History>> GetBySubscriptionAndPlaceAsync(Guid subscriptionId, Guid placeId); // ДОБАВЛЯЕМ ЭТОТ МЕТОД
    Task<Dictionary<Guid, List<History>>> GetPeriodsBySubscriptionAsync(Guid subscriptionId);
    Task<History> AddAsync(History history);
    Task<bool> UpdateAsync(History history);
}