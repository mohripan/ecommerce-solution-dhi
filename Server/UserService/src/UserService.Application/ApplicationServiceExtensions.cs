using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Application.Services;

namespace UserService.Application
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUserAppService, UserAppService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IBlacklistService, BlacklistService>();

            return services;
        }
    }
}
