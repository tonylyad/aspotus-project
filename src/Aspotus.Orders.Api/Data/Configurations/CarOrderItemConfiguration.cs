using Aspotus.Orders.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aspotus.Orders.Api.Data.Configurations;

/// <summary>
/// Конфигурация сущности позиции заказа автомобиля для Entity Framework Core.
/// </summary>
public class CarOrderItemConfiguration : IEntityTypeConfiguration<CarOrderItem>
{
    /// <summary>
    /// Выполняет настройку сущности позиции заказа автомобиля.
    /// </summary>
    /// <param name="builder">Построитель конфигурации сущности.</param>
    public void Configure(EntityTypeBuilder<CarOrderItem> builder)
    {
        builder.ToTable("CarOrderItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.BrandName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ModelName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.GenerationName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Year)
            .IsRequired();

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);
    }
}