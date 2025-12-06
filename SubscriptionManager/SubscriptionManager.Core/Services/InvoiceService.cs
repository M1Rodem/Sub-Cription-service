using SubscriptionManager.Core.Entities;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Shared.DTOs;

namespace SubscriptionManager.Core.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceService(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync()
    {
        var invoices = await _invoiceRepository.GetAllAsync();
        return invoices.Select(MapToDto);
    }

    public async Task<InvoiceDto?> GetInvoiceByIdAsync(Guid id)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        return invoice != null ? MapToDto(invoice) : null;
    }

    public async Task<IEnumerable<InvoiceDto>> GetInvoicesByUserIdAsync(Guid userId)
    {
        var invoices = await _invoiceRepository.GetByUserIdAsync(userId);
        return invoices.Select(MapToDto);
    }

    public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceDto createDto)
    {
        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            UserId = createDto.UserId,
            CreateAt = DateTime.UtcNow,
            Year = createDto.Year,
            Month = createDto.Month,
            InvoiceItems = createDto.InvoiceItems.Select(item => new InvoiceItem
            {
                Id = Guid.NewGuid(),
                ApplicationId = item.ApplicationId,
                PlaceId = item.PlaceId,
                Price = item.Price
            }).ToList()
        };

        var created = await _invoiceRepository.AddAsync(invoice);
        return MapToDto(created);
    }

    public async Task<bool> UpdateInvoiceAsync(Guid id, UpdateInvoiceDto updateDto)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id);
        if (invoice == null) return false;

        if (updateDto.InvoiceItems != null)
        {
            invoice.InvoiceItems.Clear();
            foreach (var itemDto in updateDto.InvoiceItems)
            {
                invoice.InvoiceItems.Add(new InvoiceItem
                {
                    Id = Guid.NewGuid(),
                    ApplicationId = itemDto.ApplicationId,
                    PlaceId = itemDto.PlaceId,
                    Price = itemDto.Price
                });
            }
        }

        return await _invoiceRepository.UpdateAsync(invoice);
    }

    public async Task<bool> DeleteInvoiceAsync(Guid id)
    {
        return await _invoiceRepository.DeleteAsync(id);
    }

    private InvoiceDto MapToDto(Invoice invoice)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            UserId = invoice.UserId,
            CreateAt = invoice.CreateAt,
            Year = invoice.Year,
            Month = invoice.Month,
            InvoiceItems = invoice.InvoiceItems.Select(item => new InvoiceItemDto
            {
                Id = item.Id,
                ApplicationId = item.ApplicationId,
                PlaceId = item.PlaceId,
                Price = item.Price
            }).ToList()
        };
    }
}