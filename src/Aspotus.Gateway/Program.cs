using Aspotus.Gateway.Data.Seed;
using Aspotus.Gateway.Extensions;

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
    await GatewaySeedData.SeedAsync(services);
}

app.UseGatewaySwagger();

app.UseHttpsRedirection();

app.UseCors("ReactPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGatewayProxy();

app.Run();