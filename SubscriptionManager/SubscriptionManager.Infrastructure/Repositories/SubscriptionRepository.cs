using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Core.Entities;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Infrastructure.Data;

namespace SubscriptionManager.Infrastructure.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly ApplicationDbContext _context;

    public SubscriptionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Subscription?> GetByIdAsync(Guid id)
    {
        return await _context.Subscriptions
            .Include(s => s.SubscriptionItems)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Subscription>> GetAllAsync()
    {
        return await _context.Subscriptions
            .Include(s => s.SubscriptionItems)
            .ToListAsync();
    }

    public async Task<IEnumerable<Subscription>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Subscriptions
            .Where(s => s.UserId == userId)
            .Include(s => s.SubscriptionItems)
            .ToListAsync();
    }

    public async Task<Subscription> AddAsync(Subscription subscription)
    {
        await _context.Subscriptions.AddAsync(subscription);
        await _context.SaveChangesAsync();
        return subscription;
    }

    public async Task<bool> UpdateAsync(Subscription subscription)
    {
        _context.Subscriptions.Update(subscription);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var subscription = await GetByIdAsync(id);
        if (subscription == null) return false;

        _context.Subscriptions.Remove(subscription);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Subscriptions.AnyAsync(s => s.Id == id);
    }
}