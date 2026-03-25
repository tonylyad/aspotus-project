using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Models.Responses;

namespace Aspotus.Catalog.Api.Mappers;

/// <summary>
/// Содержит методы преобразования сущности модели автомобиля в DTO ответа.
/// </summary>
public static class CarModelMapper
{
    /// <summary>
    /// Преобразует сущность модели автомобиля в DTO ответа.
    /// </summary>
    /// <param name="entity">Сущность модели автомобиля.</param>
    /// <returns>DTO ответа с информацией о модели.</returns>
    public static CarModelResponse ToResponse(CarModel entity)
    {
        return new CarModelResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            BrandId = entity.BrandId,
            BrandName = entity.Brand.Name
        };
    }
}