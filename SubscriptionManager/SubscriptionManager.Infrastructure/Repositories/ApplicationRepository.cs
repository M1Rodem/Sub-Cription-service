using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Core.Entities;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Infrastructure.Data;

namespace SubscriptionManager.Infrastructure.Repositories;

public class ApplicationRepository : IApplicationRepository
{
    private readonly ApplicationDbContext _context;

    public ApplicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Application?> GetByIdAsync(Guid id)
    {
        return await _context.Applications.FindAsync(id);
    }

    public async Task<IEnumerable<Application>> GetAllAsync()
    {
        return await _context.Applications.ToListAsync();
    }

    public async Task<Application> AddAsync(Application application)
    {
        await _context.Applications.AddAsync(application);
        await _context.SaveChangesAsync();
        return application;
    }

    public async Task<bool> UpdateAsync(Application application)
    {
        _context.Applications.Update(application);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var application = await GetByIdAsync(id);
        if (application == null) return false;

        _context.Applications.Remove(application);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Applications.AnyAsync(a => a.Id == id);
    }
}