using Aspotus.Catalog.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aspotus.Catalog.Api.Data.Configurations;

public class PartManufacturerConfiguration : IEntityTypeConfiguration<PartManufacturer>
{
    public void Configure(EntityTypeBuilder<PartManufacturer> builder)
    {
        builder.ToTable("PartManufacturers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasMany(x => x.Parts)
            .WithOne(x => x.Manufacturer)
            .HasForeignKey(x => x.ManufacturerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}