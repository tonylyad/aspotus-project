using Aspotus.Catalog.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aspotus.Catalog.Api.Data.Configurations;

public class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.ToTable("Cars");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Year)
            .IsRequired();

        builder.Property(x => x.BodyType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.EngineVolume)
            .HasPrecision(5, 2);

        builder.Property(x => x.FuelType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.TransmissionType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.DriveType)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasOne(x => x.Brand)
            .WithMany()
            .HasForeignKey(x => x.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Model)
            .WithMany()
            .HasForeignKey(x => x.ModelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Generation)
            .WithMany(x => x.Cars)
            .HasForeignKey(x => x.GenerationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.Mileage)
            .IsRequired();

        builder.Property(x => x.Price)
            .HasPrecision(18, 2);

        builder.Property(x => x.TrimLevelName)
            .HasMaxLength(100);

        builder.Property(x => x.TrimLevelDescription)
            .HasMaxLength(1000);
    }
}
