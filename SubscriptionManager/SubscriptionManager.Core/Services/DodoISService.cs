using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SubscriptionManager.Core.Common;
using SubscriptionManager.Core.Entities;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Shared.DTOs;
using System.Text.Json;

namespace SubscriptionManager.Core.Services;

public class DodoISService : IDodoISService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IAuthRepository _authRepository;
    private readonly IJwtService _jwtService;
    private readonly ILogger<DodoISService> _logger;

    public DodoISService(
        HttpClient httpClient,
        IConfiguration configuration,
        IAuthRepository authRepository,
        IJwtService jwtService,
        ILogger<DodoISService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _authRepository = authRepository;
        _jwtService = jwtService;
        _logger = logger;
    }

    // Старый метод (оставляем для обратной совместимости)
    public string GetAuthorizationUrl(string state)
    {
        return GetAuthorizationUrlWithPkce(state).AuthUrl;
    }

    // Новый метод с PKCE
    public (string AuthUrl, string CodeVerifier) GetAuthorizationUrlWithPkce(string? state = null)
    {
        var clientId = _configuration["Authentication:DodoIS:ClientId"]!;
        var redirectUri = GetCallbackUrl();

        // Используем scopes которые указаны в настройках DodoIS
        var scopes = _configuration["Authentication:DodoIS:Scopes"] ?? "openid profile email roles user.role:read";

        // Генерируем PKCE codes
        var (codeVerifier, codeChallenge) = PkceHelper.GeneratePkceCodes();

        // Формируем URL ПРАВИЛЬНО - без лишнего encoding
        var queryParams = new List<string>
    {
        $"client_id={clientId}",
        $"response_type=code",
        $"redirect_uri={Uri.EscapeDataString(redirectUri)}",
        $"scope={Uri.EscapeDataString(scopes)}",
        $"code_challenge={codeChallenge}", // УЖЕ в base64url формате
        $"code_challenge_method=S256"
    };

        if (!string.IsNullOrEmpty(state))
        {
            queryParams.Add($"state={state}");
        }

        var authUrl = $"{_configuration["Authentication:DodoIS:AuthorizationEndpoint"]}?{string.Join("&", queryParams)}";

        _logger.LogInformation($"DodoIS Auth URL: {authUrl}");
        _logger.LogInformation($"Redirect URI: {redirectUri}");
        _logger.LogInformation($"Scopes: {scopes}");
        _logger.LogInformation($"Code Challenge: {codeChallenge}");
        _logger.LogInformation($"Code Verifier: {codeVerifier}");

        return (authUrl, codeVerifier);
    }

    public async Task<DodoISTokenResponse> ExchangeCodeForTokenAsync(string code, string codeVerifier)
    {
        var clientId = _configuration["Authentication:DodoIS:ClientId"]!;
        var redirectUri = GetCallbackUrl();
        var tokenEndpoint = _configuration["Authentication:DodoIS:TokenEndpoint"]!;

        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("redirect_uri", redirectUri),
            new KeyValuePair<string, string>("code_verifier", codeVerifier)
            // Client Secret НЕ используется в PKCE flow!
        });

        var response = await _httpClient.PostAsync(tokenEndpoint, content);
        var responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Ошибка при обмене code на токен: {response.StatusCode} - {responseString}");
            throw new Exception($"Ошибка от DodoIS: {responseString}");
        }

        var tokenResponse = JsonSerializer.Deserialize<DodoISTokenResponse>(responseString);

        if (tokenResponse == null)
            throw new Exception("Не удалось получить токен от DodoIS");

        return tokenResponse;
    }

    public async Task<DodoISAccountDto> GetUserInfoAsync(string accessToken)
    {
        var userInfoEndpoint = _configuration["Authentication:DodoIS:UserInformationEndpoint"]!;

        _httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.GetAsync(userInfoEndpoint);
        var responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError($"Ошибка при получении информации о пользователе: {response.StatusCode} - {responseString}");
            throw new Exception($"Ошибка от DodoIS: {responseString}");
        }

        var accountInfo = JsonSerializer.Deserialize<DodoISAccountDto>(responseString);

        if (accountInfo == null)
            throw new Exception("Не удалось получить информацию о пользователе DodoIS");

        return accountInfo;
    }

    public async Task<LoginResponseDto> ProcessDodoISLoginAsync(string code, string codeVerifier)
    {
        // 1. Получаем токен от DodoIS (с использованием code_verifier)
        var tokenResponse = await ExchangeCodeForTokenAsync(code, codeVerifier);

        // 2. Получаем информацию о пользователе
        var userInfo = await GetUserInfoAsync(tokenResponse.AccessToken);

        // 3. Ищем или создаём пользователя в нашей системе
        var user = await _authRepository.GetByEmailAsync(userInfo.Email);

        if (user == null)
        {
            user = new AuthUser
            {
                Id = Guid.NewGuid(),
                Email = userInfo.Email,
                Name = userInfo.FullName,
                DodoISId = userInfo.Id,
                DodoISAccessToken = tokenResponse.AccessToken,
                DodoISRefreshToken = tokenResponse.RefreshToken,
                DodoISTokenExpires = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                Role = DetermineDodoISRole(userInfo.Role)
            };

            await _authRepository.AddAsync(user);
        }
        else
        {
            // Обновляем токены и информацию
            user.DodoISId = userInfo.Id;
            user.DodoISAccessToken = tokenResponse.AccessToken;
            user.DodoISRefreshToken = tokenResponse.RefreshToken;
            user.DodoISTokenExpires = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            user.LastLogin = DateTime.UtcNow;

            await _authRepository.UpdateAsync(user);
        }

        // 4. Генерируем наш JWT токен
        var ourToken = _jwtService.GenerateToken(user);

        return new LoginResponseDto
        {
            Token = ourToken,
            UserId = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = user.Role,
            Expires = DateTime.UtcNow.AddDays(7)
        };
    }

    private string DetermineDodoISRole(string dodoRole)
    {
        // Маппинг ролей DodoIS на наши роли
        return dodoRole.ToLower() switch
        {
            "admin" or "administrator" or "owner" => "Admin",
            "manager" or "unit_manager" => "PizzaManager",
            _ => "Customer"
        };
    }

    private string GetCallbackUrl()
    {
        var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "https://localhost:5001";
        var callbackPath = _configuration["Authentication:DodoIS:CallbackPath"] ?? "/api/auth/dodois-callback";

        return $"{baseUrl}{callbackPath}";
    }

    public async Task<bool> RefreshDodoISTokenAsync(AuthUser user)
    {
        if (string.IsNullOrEmpty(user.DodoISRefreshToken))
            return false;

        try
        {
            var clientId = _configuration["Authentication:DodoIS:ClientId"]!;
            var clientSecret = _configuration["Authentication:DodoIS:ClientSecret"]!;
            var tokenEndpoint = _configuration["Authentication:DodoIS:TokenEndpoint"]!;

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", user.DodoISRefreshToken),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret)
            });

            var response = await _httpClient.PostAsync(tokenEndpoint, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Ошибка при обновлении токена: {response.StatusCode} - {responseString}");
                return false;
            }

            var tokenResponse = JsonSerializer.Deserialize<DodoISTokenResponse>(responseString);

            if (tokenResponse == null)
                return false;

            // Обновляем токены пользователя
            user.DodoISAccessToken = tokenResponse.AccessToken;
            user.DodoISRefreshToken = tokenResponse.RefreshToken;
            user.DodoISTokenExpires = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);

            await _authRepository.UpdateAsync(user);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обновлении DodoIS токена");
            return false;
        }
    }

    // Метод для обратной совместимости (перегрузка без code_verifier)
    public async Task<DodoISTokenResponse> ExchangeCodeForTokenAsync(string code)
    {
        // Этот метод устарел - используем только с PKCE
        throw new NotImplementedException("Используйте метод с code_verifier для PKCE flow");
    }

    // Метод для обратной совместимости
    public async Task<LoginResponseDto> ProcessDodoISLoginAsync(string code)
    {
        // Этот метод устарел - используем только с PKCE
        throw new NotImplementedException("Используйте метод с code_verifier для PKCE flow");
    }
}