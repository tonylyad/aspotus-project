using Aspotus.Catalog.Api.Data.Entities;

namespace Aspotus.Catalog.Api.Data.Repositories.Interfaces;

/// <summary>
/// Предоставляет методы доступа к данным производителей запчастей.
/// </summary>
public interface IPartManufacturerRepository
{
    /// <summary>
    /// Возвращает всех производителей запчастей.
    /// </summary>
    Task<IReadOnlyCollection<PartManufacturer>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает производителя запчастей по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор производителя.</param>
    Task<PartManufacturer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает производителя запчастей по названию.
    /// </summary>
    /// <param name="name">Название производителя.</param>
    Task<PartManufacturer?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет нового производителя запчастей.
    /// </summary>
    /// <param name="manufacturer">Сущность производителя.</param>
    Task AddAsync(PartManufacturer manufacturer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующего производителя запчастей.
    /// </summary>
    /// <param name="manufacturer">Сущность производителя с обновлёнными данными.</param>
    Task UpdateAsync(PartManufacturer manufacturer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет производителя запчастей по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор производителя.</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}