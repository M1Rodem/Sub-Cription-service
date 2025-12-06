using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Core.Entities;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Infrastructure.Data;

namespace SubscriptionManager.Infrastructure.Repositories;

public class HistoryRepository : IHistoryRepository
{
    private readonly ApplicationDbContext _context;

    public HistoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<History?> GetActiveBySubscriptionAndPlaceAsync(Guid subscriptionId, Guid placeId)
    {
        return await _context.Histories
            .Where(h => h.SubscriptionId == subscriptionId &&
                       h.PlaceId == placeId &&
                       h.End == null)
            .FirstOrDefaultAsync();
    }

    public async Task<Dictionary<Guid, List<History>>> GetPeriodsBySubscriptionAsync(Guid subscriptionId)
    {
        var histories = await _context.Histories
            .Where(h => h.SubscriptionId == subscriptionId)
            .OrderBy(h => h.PlaceId)
            .ThenBy(h => h.Start)
            .ToListAsync();

        var result = new Dictionary<Guid, List<History>>();
        foreach (var history in histories)
        {
            if (!result.ContainsKey(history.PlaceId))
                result[history.PlaceId] = new List<History>();

            result[history.PlaceId].Add(history);
        }

        return result;
    }

    public async Task<IEnumerable<History>> GetBySubscriptionAndPlaceAsync(Guid subscriptionId, Guid placeId)
    {
        return await _context.Histories
            .Where(h => h.SubscriptionId == subscriptionId && h.PlaceId == placeId)
            .OrderBy(h => h.Start)
            .ToListAsync();
    }

    public async Task<History> AddAsync(History history)
    {
        try
        {
            await _context.Histories.AddAsync(history);
            await _context.SaveChangesAsync();
            return history;
        }
        catch (DbUpdateException ex)
        {
            // Логируем детальную ошибку
            Console.WriteLine($"Error adding history: {ex.InnerException?.Message}");
            throw;
        }
    }

    public async Task<bool> UpdateAsync(History history)
    {
        _context.Histories.Update(history);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

}