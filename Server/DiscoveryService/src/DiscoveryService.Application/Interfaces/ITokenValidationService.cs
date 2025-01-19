using DiscoveryService.Application.DTOs.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscoveryService.Application.Interfaces
{
    public interface ITokenValidationService
    {
        TokenValidationResultResponse ValidateToken(string token);
    }
}
