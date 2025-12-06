using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Core.Entities;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Infrastructure.Data;

namespace SubscriptionManager.Infrastructure.Repositories;

public class PlaceRepository : IPlaceRepository
{
    private readonly ApplicationDbContext _context;

    public PlaceRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Place?> GetByIdAsync(Guid id)
    {
        return await _context.Places.FindAsync(id);
    }

    public async Task<IEnumerable<Place>> GetAllAsync()
    {
        return await _context.Places.ToListAsync();
    }

    public async Task<Place> AddAsync(Place place)
    {
        await _context.Places.AddAsync(place);
        await _context.SaveChangesAsync();
        return place;
    }

    public async Task<bool> UpdateAsync(Place place)
    {
        _context.Places.Update(place);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var place = await GetByIdAsync(id);
        if (place == null) return false;

        _context.Places.Remove(place);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Places.AnyAsync(p => p.Id == id);
    }
}