using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryService.Application.DTOs.Responses
{
    public class TokenValidationResultResponse
    {
        public bool IsValid { get; set; }
        public string? UserId { get; set; }
        public string? Roles { get; set; }
        public string? Error { get; set; }
    }
}
