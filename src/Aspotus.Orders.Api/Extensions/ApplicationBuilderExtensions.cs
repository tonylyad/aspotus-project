using Aspotus.Orders.Api.Middlewares;

namespace Aspotus.Orders.Api.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static WebApplication UseOrdersApiSwagger(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Aspotus Orders API v1");
                    options.RoutePrefix = "swagger";
                });
            }



            return app;
        }

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
}
