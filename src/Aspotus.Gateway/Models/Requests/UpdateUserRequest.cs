using System.ComponentModel.DataAnnotations;

namespace Aspotus.Gateway.Models.Requests;

/// <summary>
/// Запрос на обновление пользователя.
/// </summary>
public class UpdateUserRequest
{
    /// <summary>
    /// Адрес электронной почты пользователя.
    /// </summary>
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Полное имя пользователя.
    /// </summary>
    [Required]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Номер телефона пользователя.
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Название роли, которую нужно назначить пользователю.
    /// </summary>
    [Required]
    public string RoleName { get; set; } = string.Empty;
}
