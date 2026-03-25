using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aspotus.Catalog.Api.Controllers;

/// <summary>
/// Предоставляет endpoint'ы для работы с моделями автомобилей.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ModelsController : ControllerBase
{
    private readonly ICarModelService _carModelService;

    /// <summary>
    /// Инициализирует новый экземпляр контроллера моделей автомобилей.
    /// </summary>
    public ModelsController(ICarModelService carModelService)
    {
        _carModelService = carModelService;
    }

    /// <summary>
    /// Возвращает список моделей автомобилей.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _carModelService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Возвращает модель автомобиля по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор модели автомобиля.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _carModelService.GetByIdAsync(id, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Создаёт новую модель автомобиля.
    /// </summary>
    /// <param name="request">Данные новой модели автомобиля.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCarModelRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _carModelService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Обновляет существующую модель автомобиля.
    /// </summary>
    /// <param name="id">Идентификатор модели автомобиля.</param>
    /// <param name="request">Новые данные модели автомобиля.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCarModelRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _carModelService.UpdateAsync(id, request, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Удаляет модель автомобиля по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор модели автомобиля.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _carModelService.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Возвращает список моделей автомобилей для указанной марки.
    /// </summary>
    /// <param name="brandId">Идентификатор марки автомобиля.</param>
    [HttpGet("by-brand/{brandId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByBrandId(Guid brandId, CancellationToken cancellationToken)
    {
        var result = await _carModelService.GetByBrandIdAsync(brandId, cancellationToken);
        return Ok(result);
    }
}