using ApiSAPBridge.API.Mapping;
using Mapster;
using MapsterMapper;

namespace ApiSAPBridge.API.Extensions
{
    public static class MapsterExtensions
    {
        public static IServiceCollection AddMapsterConfiguration(this IServiceCollection services)
        {
            // Configurar Mapster
            MapsterConfig.Configure();

            // Registrar el mapper
            services.AddMapster();

            return services;
        }
    }
}