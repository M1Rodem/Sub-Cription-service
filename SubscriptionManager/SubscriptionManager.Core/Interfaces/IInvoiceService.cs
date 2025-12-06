using SubscriptionManager.Shared.DTOs;

namespace SubscriptionManager.Core.Interfaces;

public interface IInvoiceService
{
    Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync();
    Task<InvoiceDto?> GetInvoiceByIdAsync(Guid id);
    Task<IEnumerable<InvoiceDto>> GetInvoicesByUserIdAsync(Guid userId);
    Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto createDto);
    Task<bool> UpdateInvoiceAsync(Guid id, UpdateInvoiceDto updateDto);
    Task<bool> DeleteInvoiceAsync(Guid id);
}