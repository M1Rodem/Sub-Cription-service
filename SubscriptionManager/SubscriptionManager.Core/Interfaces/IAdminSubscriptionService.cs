using SubscriptionManager.Shared.DTOs;

namespace SubscriptionManager.Core.Interfaces;

public interface IAdminSubscriptionService
{
    Task<IEnumerable<AdminSubscriptionDto>> GetAllSubscriptionsAdminAsync();
    Task<AdminSubscriptionDto?> GetSubscriptionByIdAdminAsync(Guid id);
    Task<bool> BlockSubscriptionAsync(Guid id, BlockSubscriptionDto blockDto);
    Task<bool> UnblockSubscriptionAsync(Guid id);
    Task<bool> ChangeSubscriptionStatusAsync(Guid id, ChangeSubscriptionStatusDto statusDto);
}