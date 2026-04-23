using System.ComponentModel.DataAnnotations;

namespace Aspotus.Gateway.Models.Requests;

/// <summary>
/// Запрос на создание пользователя администратором.
/// </summary>
public class CreateUserRequest
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
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Полное имя пользователя.
    /// </summary>
    [Required]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Название роли, которую нужно назначить пользователю.
    /// </summary>
    [Required]
    public string RoleName { get; set; } = string.Empty;
}