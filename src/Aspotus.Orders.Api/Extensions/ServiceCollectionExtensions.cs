using Microsoft.OpenApi;

namespace Aspotus.Orders.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOrdersApiSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Aspotus Orders API",
                    Version = "v1",
                    Description = "Orders Api for Aspotus project"
                });
            });

            return services;
        }
    }
}
