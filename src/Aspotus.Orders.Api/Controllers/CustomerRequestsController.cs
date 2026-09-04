using System.Text.Json;
using Aspotus.Orders.Api.Data.Context;
using Aspotus.Orders.Api.Data.Entities;
using Aspotus.Orders.Api.Extensions;
using Aspotus.Orders.Api.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aspotus.Orders.Api.Controllers;

[ApiController]
[Route("api/requests")]
public class CustomerRequestsController : ControllerBase
{
    private readonly OrdersDbContext _context;

    public CustomerRequestsController(OrdersDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCustomerRequest request, CancellationToken cancellationToken)
    {
        var entity = new CustomerRequest
        {
            Id = Guid.NewGuid(),
            Type = request.Type,
            CustomerName = request.CustomerName.Trim(),
            CustomerEmail = request.CustomerEmail.Trim(),
            CustomerPhone = request.CustomerPhone.Trim(),
            Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim(),
            DetailsJson = JsonSerializer.Serialize(request.Details),
            CreatedAtUtc = DateTime.UtcNow
        };

        await _context.CustomerRequests.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = entity.Id }, entity);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        if (!HttpContext.HasGatewayRole("Operator") && !HttpContext.HasGatewayRole("Admin"))
            return StatusCode(StatusCodes.Status403Forbidden);

        return Ok(await _context.CustomerRequests
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken));
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        UpdateCustomerRequestStatusRequest request,
        CancellationToken cancellationToken)
    {
        if (!HttpContext.HasGatewayRole("Operator") && !HttpContext.HasGatewayRole("Admin"))
            return StatusCode(StatusCodes.Status403Forbidden);

        var entity = await _context.CustomerRequests.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return NotFound();

        entity.Status = request.Status;
        await _context.SaveChangesAsync(cancellationToken);
        return Ok(entity);
    }
}
