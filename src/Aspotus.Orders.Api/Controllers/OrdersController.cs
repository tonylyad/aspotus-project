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
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _orderService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Возвращает список заказов указанного пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    [HttpGet("by-user/{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByUserId(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _orderService.GetByUserIdAsync(userId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Возвращает заказ по идентификатору.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _orderService.GetByIdAsync(id, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Создаёт новый заказ на запчасти.
    /// </summary>
    [HttpPost("parts")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePartOrder(
        [FromBody] CreatePartOrderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _orderService.CreatePartOrderAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Создаёт новый заказ на автомобиль.
    /// </summary>
    [HttpPost("cars")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCarOrder(
        [FromBody] CreateCarOrderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _orderService.CreateCarOrderAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Удаляет заказ по идентификатору.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _orderService.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}