using Aspotus.Catalog.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddCatalogApiSwagger();

var app = builder.Build();

app.UseCatalogApiSwagger();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();