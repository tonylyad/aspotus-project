using Aspotus.Catalog.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aspotus.Catalog.Api.Data.Configurations;

public class InventoryReservationConfiguration : IEntityTypeConfiguration<InventoryReservation>
{
    public void Configure(EntityTypeBuilder<InventoryReservation> builder)
    {
        builder.ToTable("InventoryReservations");
        builder.HasKey(x => x.OrderId);
        builder.Property(x => x.CreatedAtUtc).IsRequired();
        builder.HasMany(x => x.Items)
            .WithOne(x => x.Reservation)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
