using Aspotus.Catalog.Api.Data.Entities;
using Aspotus.Catalog.Api.Models.Responses;

namespace Aspotus.Catalog.Api.Mappers;

/// <summary>
/// Содержит методы преобразования сущности автомобиля в DTO ответа.
/// </summary>
public static class CarMapper
{
    /// <summary>
    /// Преобразует сущность автомобиля в DTO ответа.
    /// </summary>
    /// <param name="entity">Сущность автомобиля.</param>
    /// <returns>DTO ответа с информацией об автомобиле.</returns>
    public static CarResponse ToResponse(Car entity)
    {
        return new CarResponse
        {
            Id = entity.Id,
            BrandId = entity.BrandId,
            BrandName = entity.Brand.Name,
            ModelId = entity.ModelId,
            ModelName = entity.Model.Name,
            GenerationId = entity.GenerationId,
            GenerationName = entity.Generation.Name,
            Year = entity.Year,
            Mileage = entity.Mileage,
            Price = entity.Price,
            BodyType = entity.BodyType,
            TrimLevelName = entity.TrimLevelName,
            TrimLevelDescription = entity.TrimLevelDescription,
            EngineVolume = entity.EngineVolume,
            FuelType = entity.FuelType,
            TransmissionType = entity.TransmissionType,
            DriveType = entity.DriveType,
            Images = entity.Images
                .OrderBy(x => x.SortOrder)
                .Select(x => new CatalogImageResponse
                {
                    Id = x.Id,
                    FileKey = x.FileKey,
                    Url = x.Url,
                    SortOrder = x.SortOrder,
                    IsPrimary = x.IsPrimary
                })
                .ToList()
        };
    }
}
