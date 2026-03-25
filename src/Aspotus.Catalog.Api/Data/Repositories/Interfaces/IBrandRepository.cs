using Aspotus.Catalog.Api.Data.Entities;

namespace Aspotus.Catalog.Api.Data.Repositories.Interfaces;

/// <summary>
/// Предоставляет методы доступа к данным марок автомобилей.
/// </summary>
public interface IBrandRepository
{
    /// <summary>
    /// Возвращает все марки автомобилей.
    /// </summary>
    Task<IReadOnlyCollection<CarBrand>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает марку автомобиля по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор марки автомобиля.</param>
    Task<CarBrand?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает марку автомобиля по названию.
    /// </summary>
    /// <param name="name">Название марки автомобиля.</param>
    Task<CarBrand?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет новую марку автомобиля.
    /// </summary>
    /// <param name="brand">Сущность марки автомобиля.</param>
    Task AddAsync(CarBrand brand, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующую марку автомобиля.
    /// </summary>
    /// <param name="brand">Сущность марки автомобиля с обновлёнными данными.</param>
    Task UpdateAsync(CarBrand brand, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет марку автомобиля по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор марки автомобиля.</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}