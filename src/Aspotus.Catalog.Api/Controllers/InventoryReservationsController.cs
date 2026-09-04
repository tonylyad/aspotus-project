using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Aspotus.Catalog.Api.Controllers;

[ApiController]
[Route("api/inventory-reservations")]
public class InventoryReservationsController : ControllerBase
{
    private const string ApiKeyHeader = "X-Internal-Api-Key";
    private readonly IInventoryReservationService _service;
    private readonly IConfiguration _configuration;

    public InventoryReservationsController(IInventoryReservationService service, IConfiguration configuration)
    {
        _service = service;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> Reserve(ReserveInventoryRequest request, CancellationToken cancellationToken)
    {
        if (!HasValidApiKey()) return Unauthorized();
        return Ok(await _service.ReserveAsync(request, cancellationToken));
    }

    [HttpDelete("{orderId:guid}")]
    public async Task<IActionResult> Release(Guid orderId, CancellationToken cancellationToken)
    {
        if (!HasValidApiKey()) return Unauthorized();
        await _service.ReleaseAsync(orderId, cancellationToken);
        return NoContent();
    }

    private bool HasValidApiKey()
    {
        var configured = _configuration["InternalApiKey"];
        return !string.IsNullOrWhiteSpace(configured) &&
               string.Equals(Request.Headers[ApiKeyHeader], configured, StringComparison.Ordinal);
    }
}
