using Aspotus.Gateway.Data.Entities;
using Aspotus.Gateway.Models.Requests;
using Aspotus.Gateway.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
    /// Возвращает список пользователей.
    /// </summary>
    /// <returns>Список пользователей.</returns>
    [HttpGet]
    [ProducesResponseType<List<UserResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserResponse>>> GetAll()
    {
        var users = _userManager.Users.ToList();
        var result = new List<UserResponse>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            result.Add(new UserResponse
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
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

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName
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
            Email = user.Email ?? string.Empty,
            FullName = user.FullName ?? string.Empty,
            Roles = new List<string> { request.RoleName }
        });
    }
}