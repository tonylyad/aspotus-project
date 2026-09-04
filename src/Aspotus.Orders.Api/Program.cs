using Aspotus.Orders.Api.Data.Context;
using Aspotus.Orders.Api.Clients;
using Aspotus.Orders.Api.Data.Repositories.Implementations;
using Aspotus.Orders.Api.Data.Repositories.Interfaces;
using Aspotus.Orders.Api.Extensions;
using Aspotus.Orders.Api.Messaging;
using Aspotus.Orders.Api.Options;
using Aspotus.Orders.Api.Services.Implementations;
using Aspotus.Orders.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOrdersApiSwagger();

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddHttpClient<ICatalogInventoryClient, CatalogInventoryClient>((services, client) =>
{
    var configuration = services.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(configuration["Catalog:BaseUrl"] ?? "http://localhost:5299/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddDbContext<OrdersDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("OrdersDb")));

builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection(RabbitMqOptions.SectionName));
builder.Services.AddHostedService<OutboxPublisher>();

// програмный поиск сертификата для запуска службы через docker
builder.WebHost.ConfigureKestrel(options =>
{
    options.ConfigureHttpsDefaults(https =>
    {
        https.ServerCertificateSelector = (_, __) =>
        {
            using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            return store.Certificates.Find(X509FindType.FindBySubjectName, "localhost", true).OfType<X509Certificate2>().First();
        };
    });
});

var app = builder.Build();

app.UseOrdersApiForwardedHeaders();
app.UseExceptionHandling();
app.UseOrdersApiSwagger();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();
    await context.Database.MigrateAsync();
}

app.Run();
