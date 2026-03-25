using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Models.Responses;

namespace Aspotus.Catalog.Api.Services.Interfaces;

/// <summary>
/// Предоставляет методы бизнес-логики для работы с поколениями автомобилей.
/// </summary>
public interface ICarGenerationService
{
    /// <summary>
    /// Возвращает список поколений автомобилей.
    /// </summary>
    Task<IReadOnlyCollection<CarGenerationResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает поколение автомобиля по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор поколения автомобиля.</param>
    Task<CarGenerationResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт новое поколение автомобиля.
    /// </summary>
    /// <param name="request">Данные для создания поколения автомобиля.</param>
    Task<CarGenerationResponse> CreateAsync(CreateCarGenerationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующее поколение автомобиля.
    /// </summary>
    /// <param name="id">Идентификатор поколения автомобиля.</param>
    /// <param name="request">Новые данные поколения автомобиля.</param>
    Task<CarGenerationResponse?> UpdateAsync(Guid id, UpdateCarGenerationRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет поколение автомобиля по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор поколения автомобиля.</param>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает список поколений автомобилей для указанной модели.
    /// </summary>
    /// <param name="modelId">Идентификатор модели автомобиля.</param>
    Task<IReadOnlyCollection<CarGenerationResponse>> GetByModelIdAsync(Guid modelId, CancellationToken cancellationToken = default);
}