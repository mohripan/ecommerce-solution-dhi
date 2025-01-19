using DiscoveryService.Application.Interfaces;
using DiscoveryService.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DiscoveryService.Application.DTOs.Requests;

namespace DiscoveryService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ITokenValidationService _tokenValidationService;

        public TokenController(ITokenValidationService tokenValidationService)
        {
            _tokenValidationService = tokenValidationService;
        }

        [HttpPost("validate")]
        public IActionResult ValidateToken([FromBody] TokenValidationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
            {
                return BadRequest(new { IsValid = false, Error = "Token is required" });
            }

            var validationResult = _tokenValidationService.ValidateToken(request.Token);
            if (!validationResult.IsValid)
            {
                return Unauthorized(new { validationResult.IsValid, validationResult.Error });
            }

            return Ok(new
            {
                validationResult.IsValid,
                validationResult.UserId,
                validationResult.Roles
            });
        }
    }
}
