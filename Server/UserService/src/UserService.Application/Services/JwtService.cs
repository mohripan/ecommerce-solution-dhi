using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;

namespace UserService.Application.Services
{
    public class JwtService : IJwtService
    {
        public int GetUserIdFromToken(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User ID claim is missing from the token.");

            return int.Parse(userIdClaim.Value);
        }

        public string ExtractToken(string authorizationHeader)
        {
            if (string.IsNullOrEmpty(authorizationHeader))
                throw new ArgumentException("Authorization header is required.");

            var parts = authorizationHeader.Split(' ');
            if (parts.Length != 2 || parts[0] != "Bearer")
                throw new ArgumentException("Invalid Authorization header format.");

            return parts[1];
        }
    }
}
