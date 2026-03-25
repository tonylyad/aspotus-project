using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Models.Responses;

namespace Aspotus.Catalog.Api.Mappers;

/// <summary>
/// Содержит методы преобразования сущности поколения автомобиля в DTO ответа.
/// </summary>
public static class CarGenerationMapper
{
    /// <summary>
    /// Преобразует сущность поколения автомобиля в DTO ответа.
    /// </summary>
    /// <param name="entity">Сущность поколения автомобиля.</param>
    /// <returns>DTO ответа с информацией о поколении.</returns>
    public static CarGenerationResponse ToResponse(CarGeneration entity)
    {
        return new CarGenerationResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            YearFrom = entity.YearFrom,
            YearTo = entity.YearTo,
            ModelId = entity.ModelId,
            ModelName = entity.Model.Name
        };
    }
}