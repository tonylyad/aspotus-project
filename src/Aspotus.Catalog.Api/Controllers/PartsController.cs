using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Models.Responses;
using Aspotus.Catalog.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aspotus.Catalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PartsController : ControllerBase
{
    private readonly IPartService _partService;
    private readonly IInventoryReservationService? _reservationService;

    public PartsController(IPartService partService, IInventoryReservationService? reservationService = null)
    {
        _partService = partService;
        _reservationService = reservationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _partService.GetAllAsync(cancellationToken);
        await ApplyAvailabilityAsync(result, cancellationToken);
        return Ok(result);
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 9,
        [FromQuery] string? query = null,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var items = (await _partService.GetAllAsync(cancellationToken)).ToList();
        await ApplyAvailabilityAsync(items, cancellationToken);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var terms = query.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            items = items.Where(part => terms.All(term =>
                $"{part.Name} {part.Article} {part.CategoryName} {part.ManufacturerName} {part.Description} {string.Join(' ', part.ReplacementArticles)}"
                    .Contains(term, StringComparison.OrdinalIgnoreCase))).ToList();
        }

        return Ok(new PagedResponse<PartResponse>
        {
            Items = items.Skip((page - 1) * pageSize).Take(pageSize).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = items.Count
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _partService.GetByIdAsync(id, cancellationToken);
        if (result is null) return NotFound();
        await ApplyAvailabilityAsync(new[] { result }, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreatePartRequest request, CancellationToken cancellationToken)
    {
        var result = await _partService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdatePartRequest request, CancellationToken cancellationToken)
    {
        var result = await _partService.UpdateAsync(id, request, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        return await _partService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
    }

    [HttpGet("by-category/{categoryId:guid}")]
    public async Task<IActionResult> GetByCategoryId(Guid categoryId, CancellationToken cancellationToken)
    {
        var result = await _partService.GetByCategoryIdAsync(categoryId, cancellationToken);
        await ApplyAvailabilityAsync(result, cancellationToken);
        return Ok(result);
    }

    [HttpGet("by-car/{carId:guid}")]
    public async Task<IActionResult> GetByCarId(Guid carId, CancellationToken cancellationToken)
    {
        var result = await _partService.GetByCarIdAsync(carId, cancellationToken);
        await ApplyAvailabilityAsync(result, cancellationToken);
        return Ok(result);
    }

    private async Task ApplyAvailabilityAsync(IEnumerable<PartResponse> parts, CancellationToken cancellationToken)
    {
        if (_reservationService is null) return;
        var reserved = await _reservationService.GetReservedPartQuantitiesAsync(cancellationToken);
        foreach (var part in parts)
        {
            part.AvailableStockQuantity = Math.Max(0, part.StockQuantity - reserved.GetValueOrDefault(part.Id));
        }
    }
}
