using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Models.Responses;

namespace Aspotus.Catalog.Api.Mappers;

/// <summary>
/// Содержит методы преобразования сущности категории запчастей в DTO ответа.
/// </summary>
public static class PartCategoryMapper
{
    /// <summary>
    /// Преобразует сущность категории запчастей в DTO ответа.
    /// </summary>
    /// <param name="entity">Сущность категории.</param>
    /// <returns>DTO ответа с информацией о категории.</returns>
    public static PartCategoryResponse ToResponse(PartCategory entity)
    {
        return new PartCategoryResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            ParentCategoryId = entity.ParentCategoryId
        };
    }
}