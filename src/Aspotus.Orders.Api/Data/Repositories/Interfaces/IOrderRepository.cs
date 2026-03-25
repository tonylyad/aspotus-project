using Aspotus.Orders.Api.Data.Entities;

namespace Aspotus.Orders.Api.Data.Repositories.Interfaces;

/// <summary>
/// Предоставляет методы доступа к данным заказов.
/// </summary>
public interface IOrderRepository
{
    /// <summary>
    /// Возвращает все заказы.
    /// </summary>
    Task<IReadOnlyCollection<Order>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает заказ по идентификатору.
    /// </summary>
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает список заказов указанного пользователя.
    /// </summary>
    /// <param name="userId">Идентификатор пользователя.</param>
    Task<IReadOnlyCollection<Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавляет новый заказ.
    /// </summary>
    Task AddAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет существующий заказ.
    /// </summary>
    Task UpdateAsync(Order order, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удаляет заказ по идентификатору.
    /// </summary>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}