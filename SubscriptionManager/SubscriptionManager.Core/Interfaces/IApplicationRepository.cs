using SubscriptionManager.Core.Entities;

namespace SubscriptionManager.Core.Interfaces;

public interface IApplicationRepository
{
    Task<Application?> GetByIdAsync(Guid id);
    Task<IEnumerable<Application>> GetAllAsync();
    Task<Application> AddAsync(Application application);
    Task<bool> UpdateAsync(Application application);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}