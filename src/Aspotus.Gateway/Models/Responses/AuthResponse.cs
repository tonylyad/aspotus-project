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
    /// Логин пользователя.
    /// </summary>
    public string Login { get; set; } = string.Empty;

    /// <summary>
    /// Электронная почта пользователя.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Полное имя пользователя.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Номер телефона пользователя.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Роли пользователя.
    /// </summary>
    public List<string> Roles { get; set; } = new();

    /// <summary>
    /// Дата и время окончания срока действия токена в UTC.
    /// </summary>
    public DateTime ExpiresAtUtc { get; set; }
}