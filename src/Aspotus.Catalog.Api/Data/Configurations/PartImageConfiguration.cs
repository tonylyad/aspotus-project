using Aspotus.Catalog.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aspotus.Catalog.Api.Data.Configurations;

public class PartImageConfiguration : IEntityTypeConfiguration<PartImage>
{
    public void Configure(EntityTypeBuilder<PartImage> builder)
    {
        builder.ToTable("PartImages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FileKey).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Url).IsRequired().HasMaxLength(1000);
        builder.HasIndex(x => x.FileKey).IsUnique();
        builder.HasOne(x => x.Part)
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.PartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
