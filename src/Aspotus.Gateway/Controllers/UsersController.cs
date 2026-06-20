using Aspotus.Gateway.Data.Entities;
using Aspotus.Gateway.Models.Requests;
using Aspotus.Gateway.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aspotus.Gateway.Controllers;

/// <summary>
/// Методы управления пользователями.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    /// <summary>
    /// Инициализирует новый экземпляр контроллера пользователей.
    /// </summary>
    public UsersController(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Возвращает список пользователей с возможностью фильтрации.
    /// </summary>
    /// <param name="search">Поиск по логину, email или ФИО.</param>
    /// <param name="role">Фильтр по названию роли.</param>
    /// <returns>Список пользователей.</returns>
    [HttpGet]
    [ProducesResponseType<List<UserResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserResponse>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? role)
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(u =>
                (u.UserName != null && u.UserName.ToLower().Contains(term)) ||
                (u.Email != null && u.Email.ToLower().Contains(term)) ||
                (u.FullName != null && u.FullName.ToLower().Contains(term)));
        }

        var users = await query.ToListAsync();
        var result = new List<UserResponse>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            if (!string.IsNullOrWhiteSpace(role) &&
                !roles.Contains(role, StringComparer.OrdinalIgnoreCase))
            {
                continue;
            }

            result.Add(new UserResponse
            {
                Id = user.Id,
                Login = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Roles = roles.ToList()
            });
        }

        return Ok(result);
    }

    /// <summary>
    /// Создаёт нового пользователя от имени администратора.
    /// </summary>
    /// <param name="request">Данные нового пользователя.</param>
    /// <returns>Созданный пользователь.</returns>
    [HttpPost]
    [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request)
    {
        var allowedRoles = new[] { "Customer", "ContentModerator", "Operator", "Admin" };

        if (!allowedRoles.Contains(request.RoleName, StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                message = "Указана недопустимая роль."
            });
        }

        var roleExists = await _roleManager.RoleExistsAsync(request.RoleName);
        if (!roleExists)
        {
            return BadRequest(new
            {
                message = "Указанная роль не существует."
            });
        }

        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return BadRequest(new
            {
                message = "Пользователь с таким адресом электронной почты уже существует."
            });
        }

        var existingLogin = await _userManager.FindByNameAsync(request.Login);
        if (existingLogin is not null)
        {
            return BadRequest(new
            {
                message = "Пользователь с таким логином уже существует."
            });
        }

        var user = new ApplicationUser
        {
            UserName = request.Login,
            Email = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);

        if (!createResult.Succeeded)
        {
            return BadRequest(new
            {
                message = "Не удалось создать пользователя.",
                errors = createResult.Errors.Select(x => x.Description)
            });
        }

        var addToRoleResult = await _userManager.AddToRoleAsync(user, request.RoleName);

        if (!addToRoleResult.Succeeded)
        {
            return BadRequest(new
            {
                message = "Пользователь создан, но не удалось назначить роль.",
                errors = addToRoleResult.Errors.Select(x => x.Description)
            });
        }

        return CreatedAtAction(nameof(GetAll), new UserResponse
        {
            Id = user.Id,
            Login = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Roles = new List<string> { request.RoleName }
        });
    }

    /// <summary>
    /// Обновляет данные пользователя.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <param name="request">Новые данные пользователя.</param>
    /// <returns>Обновлённый пользователь.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> Update(
        Guid id, [FromBody] UpdateUserRequest request)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return NotFound(new { message = "Пользователь не найден." });
        }

        var allowedRoles = new[] { "Customer", "ContentModerator", "Operator", "Admin" };
        if (!allowedRoles.Contains(request.RoleName, StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest(new { message = "Указана недопустимая роль." });
        }

        var roleExists = await _roleManager.RoleExistsAsync(request.RoleName);
        if (!roleExists)
        {
            return BadRequest(new { message = "Указанная роль не существует." });
        }

        var emailOwner = await _userManager.FindByEmailAsync(request.Email);
        if (emailOwner is not null && emailOwner.Id != user.Id)
        {
            return BadRequest(new
            {
                message = "Пользователь с таким адресом электронной почты уже существует."
            });
        }

        user.Email = request.Email;
        user.FullName = request.FullName;
        user.PhoneNumber = request.PhoneNumber;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            return BadRequest(new
            {
                message = "Не удалось обновить пользователя.",
                errors = updateResult.Errors.Select(x => x.Description)
            });
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        if (!currentRoles.Contains(request.RoleName, StringComparer.OrdinalIgnoreCase))
        {
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, request.RoleName);
        }

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new UserResponse
        {
            Id = user.Id,
            Login = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Roles = roles.ToList()
        });
    }

    /// <summary>
    /// Удаляет пользователя.
    /// </summary>
    /// <param name="id">Идентификатор пользователя.</param>
    /// <returns>Статус удаления.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user is null)
        {
            return NotFound(new { message = "Пользователь не найден." });
        }

        await _userManager.DeleteAsync(user);
        return NoContent();
    }
}
