using DiscoveryService.Infrastructure.Configuration;

namespace DiscoveryService.Api.Extensions
{
    public static class HttpClientExtensions
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceUrls = configuration.GetSection("ServiceUrls").Get<ServiceUrls>();

            services.AddHttpClient("UserServiceClient", client =>
            {
                client.BaseAddress = new Uri(serviceUrls.UserService);
            });

            return services;
        }
    }
}
