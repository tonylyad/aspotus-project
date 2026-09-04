namespace Aspotus.Gateway.Extensions;

/// <summary>
/// Методы расширения для настройки middleware Gateway.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Подключает Swagger и Swagger UI для Gateway.
    /// </summary>
    /// <param name="app">Построитель приложения.</param>
    /// <returns>Построитель приложения.</returns>
    public static IApplicationBuilder UseGatewaySwagger(this IApplicationBuilder app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.DocumentTitle = "Aspotus API Gateway";

            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Gateway (Auth / Users / Roles)");
            options.SwaggerEndpoint("/catalog/swagger/v1/swagger.json", "Catalog (через /catalog)");
            options.SwaggerEndpoint("/orders/swagger/v1/swagger.json", "Orders (через /orders)");
            options.SwaggerEndpoint("/files/swagger/v1/swagger.json", "Files (через /files)");

            options.RoutePrefix = "swagger";
            options.DefaultModelsExpandDepth(-1);
        });

        return app;
    }
}
