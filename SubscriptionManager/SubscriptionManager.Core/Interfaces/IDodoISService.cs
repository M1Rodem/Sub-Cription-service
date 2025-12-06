using SubscriptionManager.Core.Entities;
using SubscriptionManager.Shared.DTOs;

namespace SubscriptionManager.Core.Interfaces;

public interface IDodoISService
{
    // Текущий метод (для обратной совместимости)
    string GetAuthorizationUrl(string state);

    // Новый метод с PKCE
    (string AuthUrl, string CodeVerifier) GetAuthorizationUrlWithPkce(string state);

    Task<DodoISTokenResponse> ExchangeCodeForTokenAsync(string code, string codeVerifier);
    Task<DodoISAccountDto> GetUserInfoAsync(string accessToken);
    Task<LoginResponseDto> ProcessDodoISLoginAsync(string code, string codeVerifier);
    Task<bool> RefreshDodoISTokenAsync(AuthUser user);

    // Для обратной совместимости (можно удалить после обновления всех вызовов)
    Task<DodoISTokenResponse> ExchangeCodeForTokenAsync(string code);
    Task<LoginResponseDto> ProcessDodoISLoginAsync(string code);
}