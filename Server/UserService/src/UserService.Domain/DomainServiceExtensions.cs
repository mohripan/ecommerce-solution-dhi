using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Domain.Factories;
using UserService.Domain.Services;

namespace UserService.Domain
{
    public static class DomainServiceExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IUserDomainService, UserDomainService>();
            services.AddScoped<IUserFactory, UserFactory>();

            return services;
        }
    }
}
