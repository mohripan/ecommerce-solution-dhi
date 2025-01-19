using ProductService.Domain.Factories;
using ProductService.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Domain
{
    public static class DomainServiceExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IProductDomainService, ProductDomainService>();
            services.AddScoped<IProductFactory, ProductFactory>();
            return services;
        }
    }
}
