using DiscoveryService.Application.DTOs.Responses;
using DiscoveryService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace DiscoveryService.Application.Services
{
    public class TokenValidationService : ITokenValidationService
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly IEnumerable<string> _audiences;

        public TokenValidationService(IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            _key = jwtSettings["Key"];
            _issuer = jwtSettings["Issuer"];
            _audiences = jwtSettings["Audience"].Split(',');
        }

        public TokenValidationResultResponse ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var keyBytes = Encoding.UTF8.GetBytes(_key);

                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudiences = _audiences,
                    ValidateLifetime = true
                };

                var claimsPrincipal = tokenHandler.ValidateToken(token, validationParams, out _);

                var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roles = claimsPrincipal.FindFirst(ClaimTypes.Role)?.Value;

                return new TokenValidationResultResponse
                {
                    IsValid = true,
                    UserId = userId,
                    Roles = roles
                };
            }
            catch (Exception ex)
            {
                return new TokenValidationResultResponse
                {
                    IsValid = false,
                    Error = ex.Message
                };
            }
        }
    }
}
