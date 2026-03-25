using Aspotus.Catalog.Api.Data.Entities;

namespace Aspotus.Catalog.Api.Data.Repositories.Interfaces;

/// <summary>
/// Предоставляет методы доступа к данным моделей автомобилей.
/// </summary>
public interface ICarModelRepository
{
    /// <summary>
    /// Возвращает все модели автомобилей.
    /// </summary>
    Task<IReadOnlyCollection<CarModel>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает модель автомобиля по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор модели автомобиля.</param>
    Task<CarModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает модель автомобиля по названию внутри конкретной марки.
    /// </summary>
    /// <param name="name">Название модели автомобиля.</param>
    /// <param name="brandId">Идентификатор марки автомобиля.</param>
    Task<CarModel?> GetByNameAsync(string name, Guid brandId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет новую модель автомобиля.
    /// </summary>
    /// <param name="model">Сущность модели автомобиля.</param>
    Task AddAsync(CarModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующую модель автомобиля.
    /// </summary>
    /// <param name="model">Сущность модели автомобиля с обновлёнными данными.</param>
    Task UpdateAsync(CarModel model, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет модель автомобиля по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор модели автомобиля.</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает список моделей автомобилей для указанной марки.
    /// </summary>
    /// <param name="brandId">Идентификатор марки автомобиля.</param>
    Task<IReadOnlyCollection<CarModel>> GetByBrandIdAsync(Guid brandId, CancellationToken cancellationToken = default);
}