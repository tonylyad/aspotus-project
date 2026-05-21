using Aspotus.Orders.Api.Models.Requests;
using Aspotus.Orders.Api.Models.Responses;

namespace Aspotus.Orders.Api.Services.Interfaces;

/// <summary>
/// Предоставляет методы бизнес-логики для работы с заказами.
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Возвращает список всех заказов.
    /// </summary>
    Task<IReadOnlyCollection<OrderResponse>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает заказ по идентификатору.
    /// </summary>
    Task<OrderResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает список заказов указанного пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    Task<IReadOnlyCollection<OrderResponse>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт новый заказ на запчасти.
    /// </summary>
    /// <param name="request">Данные для создания заказа.</param>
    /// <param name="userId">Идентификатор авторизованного пользователя из gateway.</param>
    /// <param name="userEmail">Электронная почта авторизованного пользователя из gateway.</param>
    /// <param name="userFullName">Полное имя авторизованного пользователя из gateway.</param>
    Task<OrderResponse> CreatePartOrderAsync(
        CreatePartOrderRequest request,
        string? userId,
        string? userEmail,
        string? userFullName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Создаёт новый заказ на автомобиль.
    /// </summary>
    /// <param name="request">Данные для создания заказа.</param>
    /// <param name="userId">Идентификатор авторизованного пользователя из gateway.</param>
    /// <param name="userEmail">Электронная почта авторизованного пользователя из gateway.</param>
    /// <param name="userFullName">Полное имя авторизованного пользователя из gateway.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task<OrderResponse> CreateCarOrderAsync(
        CreateCarOrderRequest request,
        string? userId,
        string? userEmail,
        string? userFullName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет заказ по идентификатору.
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}