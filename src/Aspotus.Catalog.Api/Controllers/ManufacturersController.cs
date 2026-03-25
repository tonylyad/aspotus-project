using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aspotus.Catalog.Api.Controllers;

/// <summary>
/// Предоставляет endpoint'ы для работы с производителями запчастей.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ManufacturersController : ControllerBase
{
    private readonly IPartManufacturerService _partManufacturerService;

    /// <summary>
    /// Инициализирует новый экземпляр контроллера производителей запчастей.
    /// </summary>
    public ManufacturersController(IPartManufacturerService partManufacturerService)
    {
        _partManufacturerService = partManufacturerService;
    }

    /// <summary>
    /// Возвращает список производителей запчастей.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _partManufacturerService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Возвращает производителя запчастей по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор производителя.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _partManufacturerService.GetByIdAsync(id, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Создаёт нового производителя запчастей.
    /// </summary>
    /// <param name="request">Данные нового производителя.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePartManufacturerRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _partManufacturerService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Обновляет существующего производителя запчастей.
    /// </summary>
    /// <param name="id">Идентификатор производителя.</param>
    /// <param name="request">Новые данные производителя.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdatePartManufacturerRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _partManufacturerService.UpdateAsync(id, request, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Удаляет производителя запчастей по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор производителя.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _partManufacturerService.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}