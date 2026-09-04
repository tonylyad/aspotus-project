using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Models.Responses;
using Aspotus.Catalog.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aspotus.Catalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly ICarService _carService;
    private readonly IInventoryReservationService? _reservationService;

    public CarsController(ICarService carService, IInventoryReservationService? reservationService = null)
    {
        _carService = carService;
        _reservationService = reservationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _carService.GetAllAsync(cancellationToken);
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
        var items = (await _carService.GetAllAsync(cancellationToken)).ToList();
        await ApplyAvailabilityAsync(items, cancellationToken);

        if (!string.IsNullOrWhiteSpace(query))
        {
            var terms = query.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            items = items.Where(car => terms.All(term =>
                $"{car.BrandName} {car.ModelName} {car.GenerationName} {car.BodyType} {car.FuelType} {car.TransmissionType} {car.DriveType} {car.Year}"
                    .Contains(term, StringComparison.OrdinalIgnoreCase))).ToList();
        }

        return Ok(new PagedResponse<CarResponse>
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
        var result = await _carService.GetByIdAsync(id, cancellationToken);
        if (result is null) return NotFound();
        await ApplyAvailabilityAsync(new[] { result }, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCarRequest request, CancellationToken cancellationToken)
    {
        var result = await _carService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateCarRequest request, CancellationToken cancellationToken)
    {
        var result = await _carService.UpdateAsync(id, request, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        return await _carService.DeleteAsync(id, cancellationToken) ? NoContent() : NotFound();
    }

    private async Task ApplyAvailabilityAsync(IEnumerable<CarResponse> cars, CancellationToken cancellationToken)
    {
        if (_reservationService is null) return;
        var reservedIds = await _reservationService.GetReservedCarIdsAsync(cancellationToken);
        foreach (var car in cars) car.IsAvailable = !reservedIds.Contains(car.Id);
    }
}
