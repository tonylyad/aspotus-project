using Aspotus.Catalog.Api.Data.Entities;

namespace Aspotus.Catalog.Api.Data.Repositories.Interfaces;

/// <summary>
/// Предоставляет методы доступа к данным поколений автомобилей.
/// </summary>
public interface ICarGenerationRepository
{
    /// <summary>
    /// Возвращает все поколения автомобилей.
    /// </summary>
    Task<IReadOnlyCollection<CarGeneration>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает поколение автомобиля по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор поколения автомобиля.</param>
    Task<CarGeneration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает поколение автомобиля по названию внутри конкретной модели.
    /// </summary>
    /// <param name="name">Название поколения.</param>
    /// <param name="modelId">Идентификатор модели автомобиля.</param>
    Task<CarGeneration?> GetByNameAsync(string name, Guid modelId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет новое поколение автомобиля.
    /// </summary>
    /// <param name="generation">Сущность поколения автомобиля.</param>
    Task AddAsync(CarGeneration generation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующее поколение автомобиля.
    /// </summary>
    /// <param name="generation">Сущность поколения автомобиля с обновлёнными данными.</param>
    Task UpdateAsync(CarGeneration generation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет поколение автомобиля по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор поколения автомобиля.</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает список поколений автомобилей для указанной модели.
    /// </summary>
    /// <param name="modelId">Идентификатор модели автомобиля.</param>
    Task<IReadOnlyCollection<CarGeneration>> GetByModelIdAsync(Guid modelId, CancellationToken cancellationToken = default);
}