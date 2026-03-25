using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Models.Responses;

namespace Aspotus.Catalog.Api.Services.Interfaces;

/// <summary>
/// Предоставляет методы бизнес-логики для работы с запчастями.
/// </summary>
public interface IPartService
{
    /// <summary>
    /// Возвращает список запчастей.
    /// </summary>
    Task<IReadOnlyCollection<PartResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает запчасть по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор запчасти.</param>
    Task<PartResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт новую запчасть.
    /// </summary>
    /// <param name="request">Данные для создания запчасти.</param>
    Task<PartResponse> CreateAsync(CreatePartRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующую запчасть.
    /// </summary>
    /// <param name="id">Идентификатор запчасти.</param>
    /// <param name="request">Новые данные запчасти.</param>
    Task<PartResponse?> UpdateAsync(Guid id, UpdatePartRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет запчасть по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор запчасти.</param>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает список запчастей для указанной категории.
    /// </summary>
    /// <param name="categoryId">Идентификатор категории запчастей.</param>
    Task<IReadOnlyCollection<PartResponse>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает список запчастей, совместимых с указанным автомобилем.
    /// </summary>
    /// <param name="carId">Идентификатор автомобиля.</param>
    Task<IReadOnlyCollection<PartResponse>> GetByCarIdAsync(Guid carId, CancellationToken cancellationToken = default);
}