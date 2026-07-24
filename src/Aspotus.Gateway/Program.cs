using Aspotus.Gateway.Data.Context;
using Aspotus.Gateway.Data.Seed;
using Aspotus.Gateway.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddGatewaySwagger();
builder.Services.AddGatewayProxy(builder.Configuration);
builder.Services.AddGatewayIdentity(builder.Configuration);
builder.Services.AddGatewayAuthentication(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddAuthorization();

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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = scope.ServiceProvider.GetRequiredService<GatewayDbContext>();
    
    await context.Database.MigrateAsync();
    await GatewaySeedData.SeedAsync(services);
}


app.UseGatewaySwagger();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("ReactPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGatewayProxy();

app.Run();