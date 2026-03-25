using Aspotus.Catalog.Api.Data.Entities;

namespace Aspotus.Catalog.Api.Data.Repositories.Interfaces;

/// <summary>
/// Предоставляет методы доступа к данным запчастей.
/// </summary>
public interface IPartRepository
{
    /// <summary>
    /// Возвращает все запчасти.
    /// </summary>
    Task<IReadOnlyCollection<Part>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает запчасть по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор запчасти.</param>
    Task<Part?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает запчасть по артикулу.
    /// </summary>
    /// <param name="article">Артикул запчасти.</param>
    Task<Part?> GetByArticleAsync(string article, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет новую запчасть.
    /// </summary>
    /// <param name="part">Сущность запчасти.</param>
    Task AddAsync(Part part, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующую запчасть.
    /// </summary>
    /// <param name="part">Сущность запчасти с обновлёнными данными.</param>
    Task UpdateAsync(Part part, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет запчасть по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор запчасти.</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает список запчастей для указанной категории.
    /// </summary>
    /// <param name="categoryId">Идентификатор категории запчастей.</param>
    Task<IReadOnlyCollection<Part>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает список запчастей, совместимых с указанным автомобилем.
    /// </summary>
    /// <param name="carId">Идентификатор автомобиля.</param>
    Task<IReadOnlyCollection<Part>> GetByCarIdAsync(Guid carId, CancellationToken cancellationToken = default);
}