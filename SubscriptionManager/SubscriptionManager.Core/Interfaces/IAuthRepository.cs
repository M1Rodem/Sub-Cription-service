using SubscriptionManager.Core.Entities;

namespace SubscriptionManager.Core.Interfaces;

public interface IAuthRepository
{
    Task<AuthUser?> GetByIdAsync(Guid id);
    Task<AuthUser?> GetByGoogleIdAsync(string googleId);
    Task<AuthUser?> GetByYandexIdAsync(string yandexId);
    Task<AuthUser?> GetByEmailAsync(string email);
    Task<AuthUser> AddAsync(AuthUser user);
    Task<bool> UpdateAsync(AuthUser user);
    Task<bool> DeleteAsync(Guid id);
}