using Microsoft.EntityFrameworkCore;

namespace Aspotus.Notifications.Worker.Data;

public sealed class NotificationsDbContext(
    DbContextOptions<NotificationsDbContext> options) : DbContext(options)
{
    public DbSet<ReceivedMessage> ReceivedMessages => Set<ReceivedMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReceivedMessage>(builder =>
        {
            builder.ToTable("ReceivedMessages");
            builder.HasKey(x => x.Id);
        });
    }
}
