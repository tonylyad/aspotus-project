using Aspotus.Catalog.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aspotus.Catalog.Api.Data.Configurations;

public class CarGenerationConfiguration : IEntityTypeConfiguration<CarGeneration>
{
    public void Configure(EntityTypeBuilder<CarGeneration> builder)
    {
        builder.ToTable("CarGenerations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.YearFrom)
            .IsRequired();

        builder.HasMany(x => x.Cars)
            .WithOne(x => x.Generation)
            .HasForeignKey(x => x.GenerationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}