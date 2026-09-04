using Microsoft.OpenApi;

namespace Aspotus.Filestore.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseFilestoreApiForwardedHeaders(this WebApplication app)
    {
        app.UseForwardedHeaders();
        return app;
    }

    public static WebApplication UseFilestoreApiSwagger(this WebApplication app)
    {
        app.UseSwagger(options =>
        {
            options.PreSerializeFilters.Add((swagger, request) =>
            {
                swagger.Servers = new List<OpenApiServer>
                {
                    new() { Url = $"{request.Scheme}://{request.Host}/files" }
                };
            });
        });

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("v1/swagger.json", "Aspotus Files API v1");
        });

        return app;
    }
}
