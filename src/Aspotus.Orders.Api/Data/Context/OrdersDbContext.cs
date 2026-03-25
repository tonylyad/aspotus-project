using System.Reflection.Emit;
using Aspotus.Orders.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Aspotus.Orders.Api.Data.Context;

/// <summary>
/// Контекст базы данных сервиса заказов.
/// </summary>
public class OrdersDbContext : DbContext
{
    /// <summary>
    /// Инициализирует новый экземпляр контекста базы данных заказов.
    /// </summary>
    /// <param name="options">Параметры конфигурации контекста.</param>
    public OrdersDbContext(DbContextOptions<OrdersDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Заказы клиентов.
    /// </summary>
    public DbSet<Order> Orders => Set<Order>();

    /// <summary>
    /// Позиции заказов запчастей.
    /// </summary>
    public DbSet<PartOrderItem> PartOrderItems => Set<PartOrderItem>();

    /// <summary>
    /// Позиции заказов автомобилей.
    /// </summary>
    public DbSet<CarOrderItem> CarOrderItems => Set<CarOrderItem>();

    /// <summary>
    /// Настраивает модель базы данных и применяет конфигурации сущностей из сборки.
    /// </summary>
    /// <param name="modelBuilder">Построитель модели.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}