using System.ComponentModel.DataAnnotations;

namespace Aspotus.Gateway.Models.Requests;

/// <summary>
/// Запрос на назначение роли пользователю.
/// </summary>
public class AssignRoleRequest
{
    /// <summary>
    /// Название роли, которую нужно назначить пользователю.
    /// </summary>
    [Required]
    public string RoleName { get; set; } = string.Empty;
}