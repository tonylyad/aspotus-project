using Aspotus.Orders.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aspotus.Orders.Api.Data.Configurations;

/// <summary>
/// Конфигурация сущности позиции заказа запчастей для Entity Framework Core.
/// </summary>
public class PartOrderItemConfiguration : IEntityTypeConfiguration<PartOrderItem>
{
    /// <summary>
    /// Выполняет настройку сущности позиции заказа запчастей.
    /// </summary>
    /// <param name="builder">Построитель конфигурации сущности.</param>
    public void Configure(EntityTypeBuilder<PartOrderItem> builder)
    {
        builder.ToTable("PartOrderItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PartName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.PartArticle)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.TotalPrice)
            .HasPrecision(18, 2);
    }
}