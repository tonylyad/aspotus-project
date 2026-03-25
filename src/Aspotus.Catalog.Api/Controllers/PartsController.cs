using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aspotus.Catalog.Api.Controllers;

/// <summary>
/// Предоставляет endpoint'ы для работы с запчастями.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PartsController : ControllerBase
{
    private readonly IPartService _partService;

    /// <summary>
    /// Инициализирует новый экземпляр контроллера запчастей.
    /// </summary>
    public PartsController(IPartService partService)
    {
        _partService = partService;
    }

    /// <summary>
    /// Возвращает список запчастей.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _partService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Возвращает запчасть по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор запчасти.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _partService.GetByIdAsync(id, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Создаёт новую запчасть.
    /// </summary>
    /// <param name="request">Данные новой запчасти.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePartRequest request, CancellationToken cancellationToken)
    {
        var result = await _partService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Обновляет существующую запчасть.
    /// </summary>
    /// <param name="id">Идентификатор запчасти.</param>
    /// <param name="request">Новые данные запчасти.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdatePartRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _partService.UpdateAsync(id, request, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Удаляет запчасть по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор запчасти.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _partService.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Возвращает список запчастей для указанной категории.
    /// </summary>
    /// <param name="categoryId">Идентификатор категории запчастей.</param>
    [HttpGet("by-category/{categoryId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCategoryId(Guid categoryId, CancellationToken cancellationToken)
    {
        var result = await _partService.GetByCategoryIdAsync(categoryId, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Возвращает список запчастей, совместимых с указанным автомобилем.
    /// </summary>
    /// <param name="carId">Идентификатор автомобиля.</param>
    [HttpGet("by-car/{carId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCarId(Guid carId, CancellationToken cancellationToken)
    {
        var result = await _partService.GetByCarIdAsync(carId, cancellationToken);
        return Ok(result);
    }
}