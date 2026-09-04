using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Models.Responses;

namespace Aspotus.Catalog.Api.Services.Interfaces;

public interface IInventoryReservationService
{
    Task<InventoryReservationResponse> ReserveAsync(ReserveInventoryRequest request, CancellationToken cancellationToken = default);
    Task ReleaseAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<HashSet<Guid>> GetReservedCarIdsAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, int>> GetReservedPartQuantitiesAsync(CancellationToken cancellationToken = default);
}
