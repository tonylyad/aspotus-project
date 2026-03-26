using System.Reflection;
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
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                options.IncludeXmlComments(xmlPath);
            });


            return services;
        }
    }
}
