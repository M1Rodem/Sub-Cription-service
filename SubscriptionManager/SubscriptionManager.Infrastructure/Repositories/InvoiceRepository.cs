using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Core.Entities;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Infrastructure.Data;

namespace SubscriptionManager.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly ApplicationDbContext _context;

    public InvoiceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice?> GetByIdAsync(Guid id)
    {
        return await _context.Invoices
            .Include(i => i.InvoiceItems)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync()
    {
        return await _context.Invoices
            .Include(i => i.InvoiceItems)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Invoices
            .Where(i => i.UserId == userId)
            .Include(i => i.InvoiceItems)
            .ToListAsync();
    }

    public async Task<Invoice> AddAsync(Invoice invoice)
    {
        await _context.Invoices.AddAsync(invoice);
        await _context.SaveChangesAsync();
        return invoice;
    }

    public async Task<bool> UpdateAsync(Invoice invoice)
    {
        _context.Invoices.Update(invoice);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var invoice = await GetByIdAsync(id);
        if (invoice == null) return false;

        _context.Invoices.Remove(invoice);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Invoices.AnyAsync(i => i.Id == id);
    }
}