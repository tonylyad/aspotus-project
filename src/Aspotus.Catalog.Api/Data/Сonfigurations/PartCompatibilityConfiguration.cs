using Aspotus.Catalog.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aspotus.Catalog.Api.Data.Configurations;

public class PartCompatibilityConfiguration : IEntityTypeConfiguration<PartCompatibility>
{
    public void Configure(EntityTypeBuilder<PartCompatibility> builder)
    {
        builder.ToTable("PartCompatibilities");

        builder.HasKey(x => new { x.PartId, x.CarId });

        builder.HasOne(x => x.Part)
            .WithMany(x => x.PartCompatibilities)
            .HasForeignKey(x => x.PartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Car)
            .WithMany(x => x.PartCompatibilities)
            .HasForeignKey(x => x.CarId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}