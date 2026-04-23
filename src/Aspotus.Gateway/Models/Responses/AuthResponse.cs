namespace Aspotus.Gateway.Models.Responses;

/// <summary>
/// Ответ с результатом авторизации.
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// JWT-токен доступа.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Электронная почта пользователя.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Полное имя пользователя.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Дата и время окончания срока действия токена в UTC.
    /// </summary>
    public DateTime ExpiresAtUtc { get; set; }
}