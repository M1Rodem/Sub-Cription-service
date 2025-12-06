using Microsoft.EntityFrameworkCore;
using SubscriptionManager.Core.Entities;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Infrastructure.Data;

namespace SubscriptionManager.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly ApplicationDbContext _context;

    public AuthRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AuthUser?> GetByIdAsync(Guid id)
    {
        return await _context.AuthUsers.FindAsync(id);
    }

    public async Task<AuthUser?> GetByGoogleIdAsync(string googleId)
    {
        return await _context.AuthUsers
            .FirstOrDefaultAsync(u => u.GoogleId == googleId);
    }

    public async Task<AuthUser?> GetByYandexIdAsync(string yandexId)
    {
        return await _context.AuthUsers
            .FirstOrDefaultAsync(u => u.YandexId == yandexId);
    }

    public async Task<AuthUser?> GetByEmailAsync(string email)
    {
        return await _context.AuthUsers
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<AuthUser> AddAsync(AuthUser user)
    {
        await _context.AuthUsers.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> UpdateAsync(AuthUser user)
    {
        _context.AuthUsers.Update(user);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await GetByIdAsync(id);
        if (user == null) return false;

        _context.AuthUsers.Remove(user);
        var result = await _context.SaveChangesAsync();
        return result > 0;
    }
}