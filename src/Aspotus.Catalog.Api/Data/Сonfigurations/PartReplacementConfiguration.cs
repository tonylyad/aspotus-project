using Aspotus.Catalog.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aspotus.Catalog.Api.Data.Configurations;

/// <summary>
/// Конфигурация сущности заменителя запчасти для Entity Framework Core.
/// </summary>
public class PartReplacementConfiguration : IEntityTypeConfiguration<PartReplacement>
{
    /// <summary>
    /// Выполняет настройку сущности заменителя запчасти.
    /// </summary>
    public void Configure(EntityTypeBuilder<PartReplacement> builder)
    {
        builder.ToTable("PartReplacements");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReplacementArticle)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(x => x.Part)
            .WithMany(x => x.ReplacementArticles)
            .HasForeignKey(x => x.PartId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}