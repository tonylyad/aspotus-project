using System.Reflection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi;

namespace Aspotus.Catalog.Api.Extensions;

/// <summary>
/// Содержит методы расширения для регистрации сервисов приложения.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует Swagger для API каталога.
    /// </summary>
    /// <param name="services">Коллекция сервисов приложения.</param>
    /// <returns>Коллекция сервисов приложения.</returns>
    public static IServiceCollection AddCatalogApiSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Aspotus Catalog API",
                Version = "v1",
                Description = """
API каталога.

⚠ Используется только через Gateway:

Префикс:
- /catalog

Пример:
GET /catalog/api/brands
"""
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor |
                ForwardedHeaders.XForwardedProto |
                ForwardedHeaders.XForwardedHost |
                ForwardedHeaders.XForwardedPrefix;

            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        return services;
    }
}