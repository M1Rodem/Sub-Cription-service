using SubscriptionManager.Core.Entities;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Shared.DTOs;

namespace SubscriptionManager.Core.Services;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IJwtService _jwtService;

    public AuthService(IAuthRepository authRepository, IJwtService jwtService)
    {
        _authRepository = authRepository;
        _jwtService = jwtService;
    }

    public async Task<LoginResponseDto> LoginOrRegisterGoogleAsync(string googleId, string email, string name)
    {
        var user = await _authRepository.GetByGoogleIdAsync(googleId);

        if (user == null)
        {
            // Регистрируем нового пользователя
            user = new AuthUser
            {
                Id = Guid.NewGuid(),
                GoogleId = googleId,
                Email = email,
                Name = name,
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                Role = DetermineUserRole(email) // Первый пользователь - админ?
            };

            await _authRepository.AddAsync(user);
        }
        else
        {
            // Обновляем время последнего входа
            user.LastLogin = DateTime.UtcNow;
            await _authRepository.UpdateAsync(user);
        }

        var token = _jwtService.GenerateToken(user);

        return new LoginResponseDto
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = user.Role,
            Expires = DateTime.UtcNow.AddDays(7)
        };
    }

    public async Task<LoginResponseDto> LoginOrRegisterYandexAsync(string yandexId, string email, string name)
    {
        var user = await _authRepository.GetByYandexIdAsync(yandexId);

        if (user == null)
        {
            user = new AuthUser
            {
                Id = Guid.NewGuid(),
                YandexId = yandexId,
                Email = email,
                Name = name,
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                Role = DetermineUserRole(email)
            };

            await _authRepository.AddAsync(user);
        }
        else
        {
            user.LastLogin = DateTime.UtcNow;
            await _authRepository.UpdateAsync(user);
        }

        var token = _jwtService.GenerateToken(user);

        return new LoginResponseDto
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = user.Role,
            Expires = DateTime.UtcNow.AddDays(7)
        };
    }

    public async Task<UserInfoDto?> GetUserInfoAsync(Guid userId)
    {
        var user = await _authRepository.GetByIdAsync(userId);
        if (user == null) return null;

        return new UserInfoDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = user.Role
        };
    }

    private string DetermineUserRole(string email)
    {
        // Первый пользователь в системе становится админом
        // TODO: Реализовать логику определения роли
        return "Customer";
    }

    public async Task<LoginResponseDto> LoginOrRegisterDodoISAsync(string dodoISId, string email, string name, string? unitId = null)
    {
        var user = await _authRepository.GetByEmailAsync(email);

        if (user == null)
        {
            user = new AuthUser
            {
                Id = Guid.NewGuid(),
                Email = email,
                Name = name,
                DodoISId = dodoISId,
                DodoISUnitId = unitId,
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                Role = "PizzaManager"
            };

            await _authRepository.AddAsync(user);
        }
        else
        {
            user.DodoISId = dodoISId;
            user.DodoISUnitId = unitId;
            user.LastLogin = DateTime.UtcNow;
            await _authRepository.UpdateAsync(user);
        }

        var token = _jwtService.GenerateToken(user);

        return new LoginResponseDto
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = user.Role,
            Expires = DateTime.UtcNow.AddDays(7)
        };
    }
}