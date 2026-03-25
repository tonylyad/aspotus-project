using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Models.Responses;

namespace Aspotus.Catalog.Api.Services.Interfaces;

/// <summary>
/// Предоставляет методы бизнес-логики для работы с категориями запчастей.
/// </summary>
public interface IPartCategoryService
{
    /// <summary>
    /// Возвращает список категорий запчастей.
    /// </summary>
    Task<IReadOnlyCollection<PartCategoryResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает категорию запчастей по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор категории.</param>
    Task<PartCategoryResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт новую категорию запчастей.
    /// </summary>
    /// <param name="request">Данные для создания категории.</param>
    Task<PartCategoryResponse> CreateAsync(CreatePartCategoryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующую категорию запчастей.
    /// </summary>
    /// <param name="id">Идентификатор категории.</param>
    /// <param name="request">Новые данные категории.</param>
    Task<PartCategoryResponse?> UpdateAsync(Guid id, UpdatePartCategoryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет категорию запчастей по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор категории.</param>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}