using SubscriptionManager.Core.Entities;

namespace SubscriptionManager.Core.Interfaces;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id);
    Task<IEnumerable<Invoice>> GetAllAsync();
    Task<IEnumerable<Invoice>> GetByUserIdAsync(Guid userId);
    Task<Invoice> AddAsync(Invoice invoice);
    Task<bool> UpdateAsync(Invoice invoice);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}