namespace Aspotus.Gateway.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseGatewaySwagger(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Aspotus Gateway API v1");
                options.RoutePrefix = "swagger";
            });
        }

        return app;
    }
}
