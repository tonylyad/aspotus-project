using Aspotus.Catalog.Api.Data.Context;
using Aspotus.Catalog.Api.Data.Repositories.Implementations;
using Aspotus.Catalog.Api.Data.Repositories.Interfaces;
using Aspotus.Catalog.Api.Data.Seed;
using Aspotus.Catalog.Api.Extensions;
using Aspotus.Catalog.Api.Services.Implementations;
using Aspotus.Catalog.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("CatalogDb")));

builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IBrandService, BrandService>();

builder.Services.AddScoped<ICarModelRepository, CarModelRepository>();
builder.Services.AddScoped<ICarModelService, CarModelService>();

builder.Services.AddScoped<ICarGenerationRepository, CarGenerationRepository>();
builder.Services.AddScoped<ICarGenerationService, CarGenerationService>();

builder.Services.AddScoped<ICarRepository, CarRepository>();
builder.Services.AddScoped<ICarService, CarService>();

builder.Services.AddScoped<IPartCategoryRepository, PartCategoryRepository>();
builder.Services.AddScoped<IPartCategoryService, PartCategoryService>();

builder.Services.AddScoped<IPartManufacturerRepository, PartManufacturerRepository>();
builder.Services.AddScoped<IPartManufacturerService, PartManufacturerService>();

builder.Services.AddScoped<IPartRepository, PartRepository>();
builder.Services.AddScoped<IPartService, PartService>();

var app = builder.Build();

app.UseExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await context.Database.MigrateAsync();
    await CatalogSeedData.SeedAsync(context);
}

app.Run();