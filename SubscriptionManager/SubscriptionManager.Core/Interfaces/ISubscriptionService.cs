using SubscriptionManager.Shared.DTOs;

namespace SubscriptionManager.Core.Interfaces;

public interface ISubscriptionService
{
    // Существующие методы
    Task<IEnumerable<SubscriptionDto>> GetAllSubscriptionsAsync();
    Task<SubscriptionDto?> GetSubscriptionByIdAsync(Guid id);
    Task<SubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionDto createDto, Guid userId);
    Task<bool> UpdateSubscriptionAsync(Guid id, UpdateSubscriptionDto updateDto);
    Task<bool> DeleteSubscriptionAsync(Guid id);

    // Новые методы
    Task<bool> AddPlacesToSubscriptionAsync(Guid subscriptionId, AddPlacesToSubscriptionDto addDto);
    Task<bool> RemovePlaceFromSubscriptionAsync(Guid subscriptionId, RemovePlaceFromSubscriptionDto removeDto);
    Task<bool> RestoreSubscriptionAsync(Guid subscriptionId, RestoreSubscriptionDto restoreDto);
}