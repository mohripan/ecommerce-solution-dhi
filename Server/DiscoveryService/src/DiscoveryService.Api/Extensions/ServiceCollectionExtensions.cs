using DiscoveryService.Application.Interfaces;
using DiscoveryService.Application.Services;

namespace DiscoveryService.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserServiceClient, UserServiceClient>();
            return services;
        }
    }
}
