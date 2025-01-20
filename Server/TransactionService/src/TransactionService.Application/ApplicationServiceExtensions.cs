using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Application.Services.Impls;
using TransactionService.Application.Services.Interfaces;

namespace TransactionService.Application
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITransactionHistoryAppService, TransactionHistoryAppService>();
            services.AddHttpClient<IProductServiceClient, ProductServiceClient>(client =>
            {
                client.BaseAddress = new Uri(configuration["DiscoveryService:BaseUrl"]);
            });
            return services;
        }
    }
}
