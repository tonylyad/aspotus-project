using Aspotus.Gateway.Data.Context;
using Aspotus.Gateway.Data.Seed;
using Aspotus.Gateway.Extensions;
using Microsoft.EntityFrameworkCore;

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