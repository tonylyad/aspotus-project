using Aspotus.Gateway.Data.Entities;
using Aspotus.Gateway.Models.Requests;
using Aspotus.Gateway.Models.Responses;
using Aspotus.Gateway.Models.Settings;
using Aspotus.Gateway.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Aspotus.Gateway.Controllers;

/// <summary>
/// Методы авторизации и регистрации пользователей.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly JwtOptions _jwtOptions;

    /// <summary>
    /// Инициализирует новый экземпляр контроллера авторизации.
    /// </summary>
    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        IOptions<JwtOptions> jwtOptions)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _jwtOptions = jwtOptions.Value;
    }

    /// <summary>
    /// Регистрирует нового покупателя.
    /// Пользователь, зарегистрированный через этот метод, всегда получает роль Customer.
    /// </summary>
    /// <param name="request">Данные для регистрации покупателя.</param>
    /// <returns>JWT-токен и информация о пользователе.</returns>
    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
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
                message = "Не удалось зарегистрировать пользователя.",
                errors = createResult.Errors.Select(x => x.Description)
            });
        }

        var addToRoleResult = await _userManager.AddToRoleAsync(user, "Customer");

        if (!addToRoleResult.Succeeded)
        {
            return BadRequest(new
            {
                message = "Пользователь создан, но не удалось назначить роль Customer.",
                errors = addToRoleResult.Errors.Select(x => x.Description)
            });
        }

        var token = await _tokenService.GenerateTokenAsync(user);

        return Ok(new AuthResponse
        {
            Token = token,
            Login = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresInMinutes)
        });
    }

    /// <summary>
    /// Выполняет вход пользователя и возвращает JWT-токен.
    /// </summary>
    /// <param name="request">Данные для входа пользователя.</param>
    /// <returns>JWT-токен и информация об авторизованном пользователе.</returns>
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Login);
        if (user is null)
        {
            return Unauthorized(new
            {
                message = "Неверный адрес электронной почты или пароль."
            });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!result.Succeeded)
        {
            return Unauthorized(new
            {
                message = "Неверный адрес электронной почты или пароль."
            });
        }

        var token = await _tokenService.GenerateTokenAsync(user);

        return Ok(new AuthResponse
        {
            Token = token,
            Login = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresInMinutes)
        });
    }

    /// <summary>
    /// Выполняет вход администратора и возвращает JWT-токен.
    /// </summary>
    /// <param name="request">Данные для входа администратора.</param>
    /// <returns>JWT-токен и информация об авторизованном администраторе.</returns>
    [AllowAnonymous]
    [HttpPost("admin-login")]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> AdminLogin([FromBody] AdminLoginRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Login);
        if (user is null)
        {
            return Unauthorized(new
            {
                message = "Неверный логин или пароль."
            });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);

        if (!result.Succeeded)
        {
            return Unauthorized(new
            {
                message = "Неверный логин или пароль."
            });
        }

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
        var isOperator = await _userManager.IsInRoleAsync(user, "Operator");
        if (!isAdmin && !isOperator)
        {
            return Unauthorized(new
            {
                message = "Доступ разрешён только администраторам и операторам."
            });
        }

        var token = await _tokenService.GenerateTokenAsync(user);
        var roles = (await _userManager.GetRolesAsync(user)).ToList();

        return Ok(new AuthResponse
        {
            Token = token,
            Login = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName ?? string.Empty,
            PhoneNumber = user.PhoneNumber ?? string.Empty,
            Roles = roles,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiresInMinutes)
        });
    }

    /// <summary>
    /// Возвращает claims текущего пользователя.
    /// </summary>
    /// <returns>Список claims текущего пользователя.</returns>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Me()
    {
        var claims = User.Claims.Select(x => new
        {
            x.Type,
            x.Value
        });

        return Ok(claims);
    }
}
