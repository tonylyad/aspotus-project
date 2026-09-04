using Aspotus.Orders.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Aspotus.Orders.Api.Data.Configurations;

public class CustomerRequestConfiguration : IEntityTypeConfiguration<CustomerRequest>
{
    public void Configure(EntityTypeBuilder<CustomerRequest> builder)
    {
        builder.ToTable("CustomerRequests");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).HasMaxLength(20).IsRequired();
        builder.Property(x => x.CustomerName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.CustomerEmail).HasMaxLength(320).IsRequired();
        builder.Property(x => x.CustomerPhone).HasMaxLength(50).IsRequired();
        builder.Property(x => x.Comment).HasMaxLength(2000);
        builder.Property(x => x.DetailsJson).IsRequired();
        builder.Property(x => x.Status).HasMaxLength(30).IsRequired();
        builder.HasIndex(x => x.CreatedAtUtc);
    }
}
