using Aspotus.Orders.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOrdersApiSwagger();

var app = builder.Build();

app.UseOrdersApiSwagger();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();