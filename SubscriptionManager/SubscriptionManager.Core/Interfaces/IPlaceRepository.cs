using SubscriptionManager.Core.Entities;

namespace SubscriptionManager.Core.Interfaces;

public interface IPlaceRepository
{
    Task<Place?> GetByIdAsync(Guid id);
    Task<IEnumerable<Place>> GetAllAsync();
    Task<Place> AddAsync(Place place);
    Task<bool> UpdateAsync(Place place);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}