using Microsoft.OpenApi;

namespace Aspotus.Gateway.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGatewaySwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Aspotus Gateway API",
                Version = "v1",
                Description = "Gateway for Aspotus project"
            });
        });

        return services;
    }
}