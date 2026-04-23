using System.ComponentModel.DataAnnotations;

namespace Aspotus.Gateway.Models.Requests;

/// <summary>
/// Запрос на вход пользователя в систему.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// Адрес электронной почты пользователя.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Пароль пользователя.
    /// </summary>
    [Required]
    public string Password { get; set; } = string.Empty;
}