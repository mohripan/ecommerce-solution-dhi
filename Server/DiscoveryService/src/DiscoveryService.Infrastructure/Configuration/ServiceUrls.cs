using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryService.Infrastructure.Configuration
{
    public class ServiceUrls
    {
        public string UserService { get; set; } = string.Empty;
        public string ProductService { get; set; } = string.Empty;
        public string TransactionService { get; set; } = string.Empty;
    }
}
