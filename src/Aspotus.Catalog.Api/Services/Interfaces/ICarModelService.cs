using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Models.Responses;

namespace Aspotus.Catalog.Api.Services.Interfaces;

/// <summary>
/// Предоставляет методы бизнес-логики для работы с моделями автомобилей.
/// </summary>
public interface ICarModelService
{
    /// <summary>
    /// Возвращает список моделей автомобилей.
    /// </summary>
    Task<IReadOnlyCollection<CarModelResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает модель автомобиля по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор модели автомобиля.</param>
    Task<CarModelResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт новую модель автомобиля.
    /// </summary>
    /// <param name="request">Данные для создания модели автомобиля.</param>
    Task<CarModelResponse> CreateAsync(CreateCarModelRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующую модель автомобиля.
    /// </summary>
    /// <param name="id">Идентификатор модели автомобиля.</param>
    /// <param name="request">Новые данные модели автомобиля.</param>
    Task<CarModelResponse?> UpdateAsync(Guid id, UpdateCarModelRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет модель автомобиля по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор модели автомобиля.</param>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает список моделей автомобилей для указанной марки.
    /// </summary>
    /// <param name="brandId">Идентификатор марки автомобиля.</param>
    Task<IReadOnlyCollection<CarModelResponse>> GetByBrandIdAsync(Guid brandId, CancellationToken cancellationToken = default);
}