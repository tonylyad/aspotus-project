using Aspotus.Gateway.Data.Entities;
using Aspotus.Gateway.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Aspotus.Gateway.Controllers;

/// <summary>
/// Методы управления ролями пользователей.
/// </summary>
[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class RolesController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    /// <summary>
    /// Инициализирует новый экземпляр контроллера ролей.
    /// </summary>
    public RolesController(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Назначает роль пользователю.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    /// <param name="request">Данные для назначения роли.</param>
    [HttpPost("{userId}/roles")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignRole(string userId, [FromBody] AssignRoleRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return NotFound(new { message = "Пользователь не найден." });
        }

        var roleExists = await _roleManager.RoleExistsAsync(request.RoleName);
        if (!roleExists)
        {
            return BadRequest(new { message = "Указанная роль не существует." });
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        if (userRoles.Contains(request.RoleName))
        {
            return BadRequest(new { message = "У пользователя уже есть эта роль." });
        }

        var result = await _userManager.AddToRoleAsync(user, request.RoleName);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "Не удалось назначить роль пользователю.",
                errors = result.Errors.Select(x => x.Description)
            });
        }

        return Ok(new { message = "Роль успешно назначена пользователю." });
    }
}