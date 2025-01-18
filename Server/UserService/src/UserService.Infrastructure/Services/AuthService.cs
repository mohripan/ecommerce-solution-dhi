using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using UserService.Contracts.Interfaces;
using UserService.Infrastructure.UnitOfWorks;

namespace UserService.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IConfiguration configuration,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> ValidateUserAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return false;

            return _passwordHasher.VerifyPassword(user.Password, password);
        }
        public async Task<string> GenerateJwtTokenAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found.");

            user.RefreshSecurityStamp();
            await _unitOfWork.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "User"),

                new Claim("SecurityStamp", user.SecurityStamp)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
            );
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            token.Payload["aud"] = new string[] { "UserService", "DiscoveryService" };

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public ClaimsPrincipal ValidateToken(string tokenString, bool validateLifetime = false)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var validationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),

                ValidateIssuer = false,
                ValidateAudience = false,

                ValidateLifetime = validateLifetime
            };

            var principal = tokenHandler.ValidateToken(
                tokenString,
                validationParams,
                out SecurityToken _);

            return principal;
        }
    }
}
