using Aspotus.Catalog.Api.Middlewares;

namespace Aspotus.Catalog.Api.Extensions;

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
}