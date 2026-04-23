using Microsoft.AspNetCore.Identity;

namespace Aspotus.Gateway.Data.Entities;

/// <summary>
/// Роль пользователя в системе.
/// </summary>
public class ApplicationRole : IdentityRole
{
    /// <summary>
    /// Описание роли.
    /// </summary>
    public string? Description { get; set; }
}