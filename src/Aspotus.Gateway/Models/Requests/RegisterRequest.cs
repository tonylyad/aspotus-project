using System.ComponentModel.DataAnnotations;

namespace Aspotus.Gateway.Models.Requests;

/// <summary>
/// Запрос на регистрацию нового покупателя.
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// Логин пользователя.
    /// </summary>
    [Required]
    public string Login { get; set; } = string.Empty;

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
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Полное имя пользователя.
    /// </summary>
    [Required]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Номер телефона пользователя.
    /// </summary>
    public string? PhoneNumber { get; set; }
}