using System.Security.Cryptography;
using System.Text;

namespace SubscriptionManager.Core.Common;

public static class PkceHelper
{
    public static (string CodeVerifier, string CodeChallenge) GeneratePkceCodes()
    {
        // Генерация code_verifier (43-128 символов, [A-Z]/[a-z]/[0-9]/-/./_/~)
        var codeVerifier = GenerateCodeVerifier();

        // Генерация code_challenge (SHA256 хэш от code_verifier в base64url)
        var codeChallenge = GenerateCodeChallenge(codeVerifier);

        return (codeVerifier, codeChallenge);
    }

    private static string GenerateCodeVerifier()
    {
        const int length = 64;
        const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";

        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);

        var chars = new char[length];
        for (int i = 0; i < length; i++)
        {
            chars[i] = validChars[bytes[i] % validChars.Length];
        }

        return new string(chars);
    }

    private static string GenerateCodeChallenge(string codeVerifier)
    {
        using var sha256 = SHA256.Create();
        var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));

        // Правильное base64url encoding
        var base64 = Convert.ToBase64String(challengeBytes);

        // Заменяем символы для base64url
        var base64Url = base64
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');

        return base64Url;
    }
}