using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aspotus.Catalog.Api.Controllers;

/// <summary>
/// Предоставляет endpoint'ы для работы с марками автомобилей.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _brandService;

    /// <summary>
    /// Инициализирует новый экземпляр контроллера марок автомобилей.
    /// </summary>
    public BrandsController(IBrandService brandService)
    {
        _brandService = brandService;
    }

    /// <summary>
    /// Возвращает список марок автомобилей.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _brandService.GetAllAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Возвращает марку автомобиля по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор марки автомобиля.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _brandService.GetByIdAsync(id, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Создаёт новую марку автомобиля.
    /// </summary>
    /// <param name="request">Данные новой марки автомобиля.</param>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateBrandRequest request, CancellationToken cancellationToken)
    {
        var result = await _brandService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Обновляет существующую марку автомобиля.
    /// </summary>
    /// <param name="id">Идентификатор марки автомобиля.</param>
    /// <param name="request">Новые данные марки автомобиля.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateBrandRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _brandService.UpdateAsync(id, request, cancellationToken);

        if (result is null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    /// <summary>
    /// Удаляет марку автомобиля по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор марки автомобиля.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _brandService.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}