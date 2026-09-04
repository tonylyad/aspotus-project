using Aspotus.Catalog.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aspotus.Catalog.Api.Data.Configurations;

public class InventoryReservationItemConfiguration : IEntityTypeConfiguration<InventoryReservationItem>
{
    public void Configure(EntityTypeBuilder<InventoryReservationItem> builder)
    {
        builder.ToTable("InventoryReservationItems");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProductType).HasMaxLength(20).IsRequired();
        builder.Property(x => x.UnitPrice).HasPrecision(18, 2);
        builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
        builder.Property(x => x.Article).HasMaxLength(100);
        builder.Property(x => x.BrandName).HasMaxLength(100);
        builder.Property(x => x.ModelName).HasMaxLength(100);
        builder.Property(x => x.GenerationName).HasMaxLength(100);
        builder.HasIndex(x => new { x.ProductType, x.ProductId });
    }
}
