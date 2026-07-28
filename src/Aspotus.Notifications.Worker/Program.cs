using Aspotus.Notifications.Worker.Data;
using Aspotus.Notifications.Worker.Messaging;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<NotificationsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("NotificationsDb")));
builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection(RabbitMqOptions.SectionName));
builder.Services.AddHostedService<OrderCreatedConsumer>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<NotificationsDbContext>();
    await context.Database.MigrateAsync();
}

await host.RunAsync();
