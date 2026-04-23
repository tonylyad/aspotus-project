namespace Aspotus.Gateway.Models.Settings;

/// <summary>
/// Настройки JWT-токена.
/// </summary>
public class JwtOptions
{
    /// <summary>
    /// Имя секции в конфигурации приложения.
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// Издатель токена.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Получатель токена.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Секретный ключ для подписи токена.
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Время жизни токена в минутах.
    /// </summary>
    public int ExpiresInMinutes { get; set; }
}