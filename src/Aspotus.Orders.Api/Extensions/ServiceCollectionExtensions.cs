using System.Reflection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi;

namespace Aspotus.Orders.Api.Extensions;

/// <summary>
/// Содержит методы расширения для регистрации сервисов приложения.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует Swagger для API заказов.
    /// </summary>
    /// <param name="services">Коллекция сервисов приложения.</param>
    /// <returns>Коллекция сервисов приложения.</returns>
    public static IServiceCollection AddOrdersApiSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Aspotus Orders API",
                Version = "v1",
                Description = """
API заказов.

⚠ Используется только через Gateway:

Префикс:
- /orders

Пример:
POST /orders/api/orders/parts
"""
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Введите JWT токен в формате: Bearer {token}"
            });
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