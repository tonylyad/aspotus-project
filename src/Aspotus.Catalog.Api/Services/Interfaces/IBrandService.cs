using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Models.Responses;

namespace Aspotus.Catalog.Api.Services.Interfaces;

/// <summary>
/// Предоставляет методы бизнес-логики для работы с марками автомобилей.
/// </summary>
public interface IBrandService
{
    /// <summary>
    /// Возвращает список марок автомобилей.
    /// </summary>
    Task<IReadOnlyCollection<BrandResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает марку автомобиля по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор марки автомобиля.</param>
    Task<BrandResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт новую марку автомобиля.
    /// </summary>
    /// <param name="request">Данные для создания марки автомобиля.</param>
    Task<BrandResponse> CreateAsync(CreateBrandRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующую марку автомобиля.
    /// </summary>
    /// <param name="id">Идентификатор марки автомобиля.</param>
    /// <param name="request">Новые данные марки автомобиля.</param>
    Task<BrandResponse?> UpdateAsync(Guid id, UpdateBrandRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет марку автомобиля по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор марки автомобиля.</param>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}