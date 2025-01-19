using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionService.Domain.Factories;
using TransactionService.Domain.Services;

namespace TransactionService.Domain
{
    public static class DomainServiceExtensions
    {
        public static IServiceCollection AddDomainServices(this IServiceCollection services)
        {
            services.AddScoped<ITransactionHistoryDomainService, TransactionDomainHistoryService>();
            services.AddScoped<ITransactionHistoryFactory, TransactionHistoryFactory>();
            return services;
        }
    }
}
