using Microsoft.OpenApi;

namespace Aspotus.Catalog.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCatalogApiSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Aspotus Catalog API",
                    Version = "v1",
                    Description = "Catalog Api for Aspotus project"
                });
            });

            return services;
        }
    }
}
