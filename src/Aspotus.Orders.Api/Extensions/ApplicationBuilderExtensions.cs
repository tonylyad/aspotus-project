using Aspotus.Orders.Api.Middlewares;
using Microsoft.OpenApi;

namespace Aspotus.Orders.Api.Extensions;

/// <summary>
/// Содержит методы расширения для настройки конвейера HTTP-запросов приложения.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Подключает middleware глобальной обработки исключений.
    /// </summary>
    /// <param name="app">Экземпляр веб-приложения.</param>
    /// <returns>Настроенный экземпляр веб-приложения.</returns>
    public static WebApplication UseExceptionHandling(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        return app;
    }

    /// <summary>
    /// Подключает обработку forwarded headers для корректной работы за gateway.
    /// </summary>
    /// <param name="app">Экземпляр веб-приложения.</param>
    /// <returns>Настроенный экземпляр веб-приложения.</returns>
    public static WebApplication UseOrdersApiForwardedHeaders(this WebApplication app)
    {
        app.UseForwardedHeaders();
        return app;
    }

    /// <summary>
    /// Подключает Swagger и Swagger UI для API заказов.
    /// При открытии через gateway указывает базовый адрес с префиксом /orders.
    /// </summary>
    /// <param name="app">Экземпляр веб-приложения.</param>
    /// <returns>Настроенный экземпляр веб-приложения.</returns>
    public static WebApplication UseOrdersApiSwagger(this WebApplication app)
    {
        app.UseSwagger(options =>
        {
            options.PreSerializeFilters.Add((swagger, httpReq) =>
            {
                swagger.Servers = new List<OpenApiServer>
                {
                    new()
                    {
                        Url = $"{httpReq.Scheme}://{httpReq.Host}/orders"
                    }
                };
            });
        });

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("v1/swagger.json", "Aspotus Orders API v1");
        });

        return app;
    }
}