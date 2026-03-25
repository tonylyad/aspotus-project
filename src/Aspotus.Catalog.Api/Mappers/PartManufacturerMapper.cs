using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Models.Responses;

namespace Aspotus.Catalog.Api.Mappers;

/// <summary>
/// Содержит методы преобразования сущности производителя запчастей в DTO ответа.
/// </summary>
public static class PartManufacturerMapper
{
    /// <summary>
    /// Преобразует сущность производителя запчастей в DTO ответа.
    /// </summary>
    /// <param name="entity">Сущность производителя.</param>
    /// <returns>DTO ответа с информацией о производителе.</returns>
    public static PartManufacturerResponse ToResponse(PartManufacturer entity)
    {
        return new PartManufacturerResponse
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }
}