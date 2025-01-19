using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Application.Services.Interfaces
{
    public interface IJwtService
    {
        int GetUserIdFromToken(ClaimsPrincipal user);
        string ExtractToken(string authorizationHeader);
    }
}
