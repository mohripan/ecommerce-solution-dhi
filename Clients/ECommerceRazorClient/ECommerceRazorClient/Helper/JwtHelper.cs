using System.IdentityModel.Tokens.Jwt;

namespace ECommerceRazorClient.Helper
{
    public static class JwtHelper
    {
        public static string GetClaimFromToken(string token, string claimType)
        {
            var jwtHandler = new JwtSecurityTokenHandler();

            if (!jwtHandler.CanReadToken(token))
                throw new ArgumentException("Invalid JWT token");

            var jwtToken = jwtHandler.ReadJwtToken(token);
            var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == claimType);

            return claim?.Value;
        }
    }
}
