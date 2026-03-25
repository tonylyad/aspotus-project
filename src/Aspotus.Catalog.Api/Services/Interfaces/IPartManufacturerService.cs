using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Models.Responses;

namespace Aspotus.Catalog.Api.Services.Interfaces;

/// <summary>
/// Предоставляет методы бизнес-логики для работы с производителями запчастей.
/// </summary>
public interface IPartManufacturerService
{
    /// <summary>
    /// Возвращает список производителей запчастей.
    /// </summary>
    Task<IReadOnlyCollection<PartManufacturerResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает производителя запчастей по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор производителя.</param>
    Task<PartManufacturerResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт нового производителя запчастей.
    /// </summary>
    /// <param name="request">Данные для создания производителя.</param>
    Task<PartManufacturerResponse> CreateAsync(CreatePartManufacturerRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующего производителя запчастей.
    /// </summary>
    /// <param name="id">Идентификатор производителя.</param>
    /// <param name="request">Новые данные производителя.</param>
    Task<PartManufacturerResponse?> UpdateAsync(Guid id, UpdatePartManufacturerRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет производителя запчастей по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор производителя.</param>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}