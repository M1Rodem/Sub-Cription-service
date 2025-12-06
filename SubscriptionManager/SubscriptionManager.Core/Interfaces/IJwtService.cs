using System.Security.Claims;
using SubscriptionManager.Core.Entities;

namespace SubscriptionManager.Core.Interfaces;

public interface IJwtService
{
    string GenerateToken(AuthUser user);
    ClaimsPrincipal? ValidateToken(string token);
}