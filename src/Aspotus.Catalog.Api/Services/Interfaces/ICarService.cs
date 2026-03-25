using Aspotus.Catalog.Api.Models.Requests;
using Aspotus.Catalog.Api.Models.Responses;

namespace Aspotus.Catalog.Api.Services.Interfaces;

/// <summary>
/// Предоставляет методы бизнес-логики для работы с автомобилями.
/// </summary>
public interface ICarService
{
    /// <summary>
    /// Возвращает список автомобилей.
    /// </summary>
    Task<IReadOnlyCollection<CarResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает автомобиль по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор автомобиля.</param>
    Task<CarResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт новый автомобиль.
    /// </summary>
    /// <param name="request">Данные для создания автомобиля.</param>
    Task<CarResponse> CreateAsync(CreateCarRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующий автомобиль.
    /// </summary>
    /// <param name="id">Идентификатор автомобиля.</param>
    /// <param name="request">Новые данные автомобиля.</param>
    Task<CarResponse?> UpdateAsync(Guid id, UpdateCarRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет автомобиль по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор автомобиля.</param>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}