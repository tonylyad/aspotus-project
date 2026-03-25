using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aspotus.Catalog.Api.Controllers;

/// <summary>
/// Предоставляет endpoint'ы для работы с поколениями автомобилей.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GenerationsController : ControllerBase
{
    private readonly ICarGenerationService _carGenerationService;

    /// <summary>
    /// Инициализирует новый экземпляр контроллера поколений автомобилей.
    /// </summary>
    public GenerationsController(ICarGenerationService carGenerationService)
    {
        _carGenerationService = carGenerationService;
    }

    /// <summary>
    /// Возвращает список поколений автомобилей.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _carGenerationService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Возвращает поколение автомобиля по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор поколения автомобиля.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _carGenerationService.GetByIdAsync(id, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Создаёт новое поколение автомобиля.
    /// </summary>
    /// <param name="request">Данные нового поколения автомобиля.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCarGenerationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _carGenerationService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Обновляет существующее поколение автомобиля.
    /// </summary>
    /// <param name="id">Идентификатор поколения автомобиля.</param>
    /// <param name="request">Новые данные поколения автомобиля.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCarGenerationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _carGenerationService.UpdateAsync(id, request, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Удаляет поколение автомобиля по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор поколения автомобиля.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _carGenerationService.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }

    /// <summary>
    /// Возвращает список поколений автомобилей для указанной модели.
    /// </summary>
    /// <param name="modelId">Идентификатор модели автомобиля.</param>
    [HttpGet("by-model/{modelId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByModelId(Guid modelId, CancellationToken cancellationToken)
    {
        var result = await _carGenerationService.GetByModelIdAsync(modelId, cancellationToken);
        return Ok(result);
    }
}