using System.Reflection;
using Microsoft.OpenApi;

namespace Aspotus.Gateway.Extensions;

/// <summary>
/// Методы расширения для регистрации сервисов Gateway.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Регистрирует Swagger для Gateway.
    /// </summary>
    /// <param name="services">Коллекция сервисов приложения.</param>
    /// <returns>Коллекция сервисов приложения.</returns>
    public static IServiceCollection AddGatewaySwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Aspotus Gateway API",
                Version = "v1",
                Description = """
Главная точка входа в систему Aspotus.

Все запросы должны отправляться через Gateway.

Доступные префиксы:

- /catalog — API каталога
- /orders — API заказов
- /files — API файлов

Аутентификация:
- JWT Bearer Token

Роли:
- Customer
- ContentModerator
- Operator
- Admin
"""
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            const string schemeId = "Bearer";

            options.AddSecurityDefinition(schemeId, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Вставьте только JWT токен без слова Bearer. Swagger добавит Bearer автоматически."
            });

            options.AddSecurityRequirement(_ =>
            {
                var requirement = new OpenApiSecurityRequirement();

                requirement.Add(
                    new OpenApiSecuritySchemeReference(schemeId),
                    new List<string>()
                );

                return requirement;
            });
        });

        return services;
    }

    /// <summary>
    /// Регистрирует reverse proxy для Gateway.
    /// </summary>
    /// <param name="services">Коллекция сервисов приложения.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <returns>Коллекция сервисов приложения.</returns>
    public static IServiceCollection AddGatewayProxy(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"));

        return services;
    }
}
