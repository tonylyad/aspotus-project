namespace Aspotus.Gateway.Models.Responses;

/// <summary>
/// Ответ с информацией о пользователе.
/// </summary>
public class UserResponse
{
    /// <summary>
    /// Уникальный идентификатор пользователя.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Адрес электронной почты пользователя.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Полное имя пользователя.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Список ролей пользователя.
    /// </summary>
    public List<string> Roles { get; set; } = new();
}