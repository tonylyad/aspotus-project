using Aspotus.Orders.Api.Extensions;
using Aspotus.Orders.Api.Models.Requests;
using Aspotus.Orders.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aspotus.Orders.Api.Controllers;

/// <summary>
/// Предоставляет endpoint'ы для работы с заказами.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    /// <summary>
    /// Инициализирует новый экземпляр контроллера заказов.
    /// </summary>
    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Возвращает список всех заказов.
    /// Доступно только оператору и администратору.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        if (!HttpContext.HasGatewayRole("Operator") &&
            !HttpContext.HasGatewayRole("Admin"))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var result = await _orderService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Возвращает список заказов указанного пользователя.
    /// Покупатель может смотреть только свои заказы.
    /// Оператор и администратор могут смотреть любые.
    /// </summary>
    [HttpGet("by-user/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var isOperator = HttpContext.HasGatewayRole("Operator");
        var isAdmin = HttpContext.HasGatewayRole("Admin");
        var isCustomer = HttpContext.HasGatewayRole("Customer");

        if (!isOperator && !isAdmin)
        {
            if (!isCustomer)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            var currentUserId = HttpContext.GetGatewayUserId();

            if (!Guid.TryParse(currentUserId, out var parsedCurrentUserId) || parsedCurrentUserId != userId)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
        }

        var result = await _orderService.GetByUserIdAsync(userId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Возвращает заказ по идентификатору.
    /// Доступно только оператору и администратору.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        if (!HttpContext.HasGatewayRole("Operator") &&
            !HttpContext.HasGatewayRole("Admin"))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var result = await _orderService.GetByIdAsync(id, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Создаёт новый заказ на запчасти.
    /// Доступно покупателю, оператору и администратору.
    /// </summary>
    [HttpPost("parts")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreatePartOrder(
        [FromBody] CreatePartOrderRequest request,
        CancellationToken cancellationToken)
    {
        if (!HttpContext.HasGatewayRole("Customer") &&
            !HttpContext.HasGatewayRole("Operator") &&
            !HttpContext.HasGatewayRole("Admin"))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var result = await _orderService.CreatePartOrderAsync(
            request,
            HttpContext.GetGatewayUserId(),
            HttpContext.GetGatewayUserEmail(),
            HttpContext.GetGatewayUserFullName(),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Создаёт новый заказ на автомобиль.
    /// Доступно покупателю, оператору и администратору.
    /// </summary>
    [HttpPost("cars")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateCarOrder(
        [FromBody] CreateCarOrderRequest request,
        CancellationToken cancellationToken)
    {
        if (!HttpContext.HasGatewayRole("Customer") &&
            !HttpContext.HasGatewayRole("Operator") &&
            !HttpContext.HasGatewayRole("Admin"))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var result = await _orderService.CreateCarOrderAsync(
            request,
            HttpContext.GetGatewayUserId(),
            HttpContext.GetGatewayUserEmail(),
            HttpContext.GetGatewayUserFullName(),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Удаляет заказ по идентификатору.
    /// Доступно только администратору.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        if (!HttpContext.HasGatewayRole("Admin"))
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        var deleted = await _orderService.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
