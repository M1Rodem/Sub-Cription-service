using SubscriptionManager.Shared.DTOs;

public interface IAuthService
{
    // Существующие методы
    Task<LoginResponseDto> LoginOrRegisterGoogleAsync(string googleId, string email, string name);
    Task<LoginResponseDto> LoginOrRegisterYandexAsync(string yandexId, string email, string name);
    Task<LoginResponseDto> LoginOrRegisterDodoISAsync(string dodoISId, string email, string name, string? unitId = null);
    Task<UserInfoDto?> GetUserInfoAsync(Guid userId);
}