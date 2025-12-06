using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SubscriptionManager.Core.Common;
using SubscriptionManager.Core.Interfaces;
using SubscriptionManager.Shared.DTOs;
using System.Security.Claims;

namespace SubscriptionManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpGet("simple-yandex-login")]
    public async Task<IActionResult> SimpleYandexLogin()
    {
        // Прямое создание пользователя без OAuth потока
        var result = await _authService.LoginOrRegisterYandexAsync(
            $"yandex_{DateTime.Now.Ticks}",
            $"user_{Guid.NewGuid().ToString()[..8]}@yandex.ru",
            "Yandex Test User");

        return Ok(new
        {
            message = "Тестовый пользователь создан",
            token = result.Token,
            redirect = "http://localhost:5001/swagger"
        });
    }

    // GET: api/auth/yandex-login - Начало OAuth потока Yandex
    [HttpGet("yandex-login")]
    public IActionResult YandexLogin()
    {
        // Используем фиксированный state для тестирования
        var state = "subscription_manager_state_2025";

        var yandexAuthUrl = $"https://oauth.yandex.ru/authorize" +
            $"?response_type=code" +
            $"&client_id=cd838bacff2b42ab9e01facc447de27d" +
            $"&redirect_uri=http://localhost:5001/api/auth/yandex-callback" +
            $"&state={state}";

        return Redirect(yandexAuthUrl);
    }

    [HttpGet("yandex-callback")]
    public async Task<IActionResult> YandexCallback([FromQuery] string code, [FromQuery] string state)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest("Код авторизации не получен");
        }

        // Простая проверка state (для тестирования)
        var expectedState = "subscription_manager_state_2025";
        if (state != expectedState)
        {
            _logger.LogWarning($"Неверный state: получен {state}, ожидался {expectedState}");
            // Пока пропускаем для тестирования
            // return BadRequest("Неверный state параметр");
        }

        try
        {
            // ВРЕМЕННО: создаём тестового пользователя
            // TODO: Реализовать реальный OAuth flow

            var testEmail = $"user_{Guid.NewGuid().ToString()[..8]}@yandex.ru";
            var testName = "Yandex User";
            var yandexId = $"yandex_{code.Substring(0, Math.Min(10, code.Length))}"; // Используем часть code как ID

            var result = await _authService.LoginOrRegisterYandexAsync(yandexId, testEmail, testName);

            return Ok(new
            {
                message = "Успешная авторизация через Yandex!",
                token = result.Token,
                userId = result.UserId,
                email = result.Email,
                name = result.Name,
                note = "Это тестовый режим. В production нужно реализовать обмен code на access_token"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке Yandex callback");
            return BadRequest($"Ошибка авторизации: {ex.Message}");
        }
    }

    // POST: api/auth/test-yandex - Тестовый эндпоинт для разработки
    [HttpPost("test-yandex")]
    public async Task<IActionResult> TestYandexLogin()
    {
        var result = await _authService.LoginOrRegisterYandexAsync(
            $"test_yandex_id_{DateTime.Now.Ticks}",
            $"test_{Guid.NewGuid().ToString()[..8]}@yandex.ru",
            "Test Yandex User");

        return Ok(result);
    }

    // GET: api/auth/me - Получить информацию о текущем пользователе
    [HttpGet("me")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized();

        var userInfo = await _authService.GetUserInfoAsync(userId);
        if (userInfo == null)
            return NotFound();

        return Ok(userInfo);
    }

    // POST: api/auth/logout
    [HttpPost("logout")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public IActionResult Logout()
    {
        return Ok(new { message = "Вы успешно вышли из системы" });
    }

    [HttpGet("dodois-login")]
    public IActionResult DodoISLogin()
    {
        var state = Guid.NewGuid().ToString();

        var dodoISService = HttpContext.RequestServices.GetRequiredService<IDodoISService>();
        var (authUrl, codeVerifier) = dodoISService.GetAuthorizationUrlWithPkce(state);

        // Сохраняем code_verifier в сессии для использования в callback
        HttpContext.Session.SetString($"DodoIS_CodeVerifier_{state}", codeVerifier);

        return Redirect(authUrl);
    }

    // GET: / - Обработчик для корневого callback от DodoIS
    [HttpGet("/")]
    public async Task<IActionResult> RootCallback([FromQuery] string? code, [FromQuery] string? state)
    {
        // Это тот же код, что и в dodois-callback, но для корневого пути
        if (string.IsNullOrEmpty(code))
        {
            // Если нет code параметра, возможно это просто заход на корень
            return Ok("Subscription Manager API is running. Use /swagger for API documentation.");
        }

        // Если есть code - обрабатываем как OAuth callback
        return await DodoISCallback(code, state);
    }

    [HttpGet("dodois-test")]
    public IActionResult DodoISTest()
    {
        try
        {
            // 1. Проверяем конфигурацию
            var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            var clientId = configuration["Authentication:DodoIS:ClientId"];
            var redirectUri = "https://localhost:5001"; // Жёстко задаём для теста
            var scopes = configuration["Authentication:DodoIS:Scopes"] ?? "openid profile email roles";

            // 2. Генерируем PKCE вручную для проверки
            var (codeVerifier, codeChallenge) = PkceHelper.GeneratePkceCodes();

            // 3. Формируем URL максимально просто
            var authUrl = $"https://auth.dodois.io/connect/authorize?" +
                         $"client_id={clientId}" +
                         $"&response_type=code" +
                         $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                         $"&scope={Uri.EscapeDataString(scopes)}" +
                         $"&code_challenge={codeChallenge}" +
                         $"&code_challenge_method=S256";

            // 4. Возвращаем всё для анализа
            return Ok(new
            {
                success = true,
                configuration = new
                {
                    clientId,
                    redirectUri,
                    scopes
                },
                pkce = new
                {
                    codeVerifier,
                    codeChallenge,
                    codeChallengeLength = codeChallenge.Length
                },
                authUrl = authUrl,

                // Альтернативный вариант URL (без state)
                authUrlSimple = $"https://auth.dodois.io/connect/authorize?" +
                              $"client_id={clientId}" +
                              $"&response_type=code" +
                              $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
                              $"&scope=openid" +
                              $"&code_challenge={codeChallenge}" +
                              $"&code_challenge_method=S256",

                instructions = new[]
                {
                "1. Скопируй 'authUrl' и открой в браузере",
                "2. Если ошибка - попробуй 'authUrlSimple' (только openid scope)",
                "3. Проверь правильность redirect_uri в настройках DodoIS",
                "4. Убедись что используется HTTPS для localhost"
            }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    [HttpGet("dodois-debug")]
    public IActionResult DodoISDebug()
    {
        try
        {
            var dodoISService = HttpContext.RequestServices.GetRequiredService<IDodoISService>();
            var (authUrl, codeVerifier) = dodoISService.GetAuthorizationUrlWithPkce("test_state_123");

            // Сохраняем code_verifier для теста
            HttpContext.Session.SetString($"DodoIS_CodeVerifier_test_state_123", codeVerifier);

            return Ok(new
            {
                success = true,
                authUrl = authUrl,
                codeVerifier = codeVerifier,
                instructions = "1. Открой ссылку в браузере\n2. Авторизуйся в DodoIS\n3. Будет redirect на https://localhost:5001/?code=...\n4. Скопируй code параметр",
                testCallbackUrl = "https://localhost:5001/?code=test_code_here&state=test_state_123"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    // Обновляем существующий метод DodoISCallback
    [HttpGet("dodois-callback")]
    public async Task<IActionResult> DodoISCallback([FromQuery] string? code, [FromQuery] string? state)
    {
        if (string.IsNullOrEmpty(code))
        {
            return BadRequest("Код авторизации не получен");
        }

        try
        {
            // Получаем code_verifier из сессии
            var codeVerifier = HttpContext.Session.GetString($"DodoIS_CodeVerifier_{state}");

            if (string.IsNullOrEmpty(codeVerifier))
            {
                return BadRequest("Сессия истекла или code_verifier не найден. Начните процесс авторизации заново.");
            }

            // Удаляем из сессии после использования
            HttpContext.Session.Remove($"DodoIS_CodeVerifier_{state}");

            var dodoISService = HttpContext.RequestServices.GetRequiredService<IDodoISService>();
            var result = await dodoISService.ProcessDodoISLoginAsync(code, codeVerifier);

            return Ok(new
            {
                message = "✅ Успешная авторизация через DodoIS!",
                token = result.Token,
                userId = result.UserId,
                email = result.Email,
                name = result.Name,
                role = result.Role,
                expires = result.Expires.ToString("yyyy-MM-dd HH:mm:ss"),
                redirectTo = "http://localhost:5001/swagger"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при обработке DodoIS callback");
            return BadRequest($"Ошибка авторизации: {ex.Message}");
        }
    }
}