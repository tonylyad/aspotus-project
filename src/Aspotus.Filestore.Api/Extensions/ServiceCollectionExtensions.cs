using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi;

namespace Aspotus.Filestore.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFilestoreApiSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Aspotus Files API",
                Version = "v1",
                Description = """
API для работы с файлами.

Используется через Gateway с префиксом /files.
"""
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
