using Aspotus.Gateway.Data.Entities;
using Aspotus.Gateway.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Aspotus.Gateway.Controllers;

/// <summary>
/// Методы для работы с покупателями (пользователями с ролью Customer).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Operator")]
public class CustomersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;

    /// <summary>
    /// Инициализирует новый экземпляр контроллера покупателей.
    /// </summary>
    public CustomersController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    /// <summary>
    /// Возвращает список всех покупателей (пользователей с ролью Customer).
    /// </summary>
    /// <returns>Список покупателей.</returns>
    [HttpGet]
    [ProducesResponseType<List<UserResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserResponse>>> GetAll()
    {
        var users = _userManager.Users.ToList();
        var result = new List<UserResponse>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            if (!roles.Contains("Customer"))
                continue;

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
}
