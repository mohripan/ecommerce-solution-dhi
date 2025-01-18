using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using UserService.Application.Services;
using UserService.Contracts.Interfaces;

namespace UserService.Api.Extensions
{
    public static class JwtBearerExtensions
    {
        public static IServiceCollection AddCustomJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudiences = configuration["Jwt:Audience"].Split(','),
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async context =>
                        {
                            var principal = context.Principal;
                            if (principal == null)
                            {
                                context.Fail("No principal found.");
                                return;
                            }

                            var tokenStamp = principal.FindFirst("SecurityStamp")?.Value;
                            if (string.IsNullOrEmpty(tokenStamp))
                            {
                                context.Fail("No security stamp in token.");
                                return;
                            }

                            var userIdStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                            if (!int.TryParse(userIdStr, out var userId))
                            {
                                context.Fail("Invalid user ID claim.");
                                return;
                            }

                            var userRepo = context.HttpContext.RequestServices
                                .GetRequiredService<IUserRepository>();
                            var user = await userRepo.GetByIdAsync(userId);
                            if (user == null)
                            {
                                context.Fail("User not found in DB.");
                                return;
                            }

                            if (user.SecurityStamp != tokenStamp)
                            {
                                context.Fail("Security stamp mismatch (token invalid).");
                                return;
                            }
                        },
                    };
                });

            return services;
        }
    }
}
