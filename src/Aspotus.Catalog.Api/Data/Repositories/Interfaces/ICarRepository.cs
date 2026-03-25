using Aspotus.Catalog.Api.Data.Entities;

namespace Aspotus.Catalog.Api.Data.Repositories.Interfaces;

/// <summary>
/// Предоставляет методы доступа к данным автомобилей.
/// </summary>
public interface ICarRepository
{
    /// <summary>
    /// Возвращает все автомобили.
    /// </summary>
    Task<IReadOnlyCollection<Car>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает автомобиль по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор автомобиля.</param>
    Task<Car?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет новый автомобиль.
    /// </summary>
    /// <param name="car">Сущность автомобиля.</param>
    Task AddAsync(Car car, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующий автомобиль.
    /// </summary>
    /// <param name="car">Сущность автомобиля с обновлёнными данными.</param>
    Task UpdateAsync(Car car, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет автомобиль по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор автомобиля.</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}