using Aspotus.Catalog.Api.Data.Entities;

namespace Aspotus.Catalog.Api.Data.Repositories.Interfaces;

/// <summary>
/// Предоставляет методы доступа к данным категорий запчастей.
/// </summary>
public interface IPartCategoryRepository
{
    /// <summary>
    /// Возвращает все категории запчастей.
    /// </summary>
    Task<IReadOnlyCollection<PartCategory>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает категорию запчастей по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор категории.</param>
    Task<PartCategory?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает категорию запчастей по названию.
    /// </summary>
    /// <param name="name">Название категории.</param>
    Task<PartCategory?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет новую категорию запчастей.
    /// </summary>
    /// <param name="category">Сущность категории.</param>
    Task AddAsync(PartCategory category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующую категорию запчастей.
    /// </summary>
    /// <param name="category">Сущность категории с обновлёнными данными.</param>
    Task UpdateAsync(PartCategory category, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет категорию запчастей по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор категории.</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}