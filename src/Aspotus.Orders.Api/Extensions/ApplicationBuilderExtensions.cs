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
    }
}
