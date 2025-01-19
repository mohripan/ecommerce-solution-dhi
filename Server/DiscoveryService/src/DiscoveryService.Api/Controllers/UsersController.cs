using DiscoveryService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DiscoveryService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserServiceClient _userServiceClient;

        public UsersController(IUserServiceClient userServiceClient)
        {
            _userServiceClient = userServiceClient;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] int page = 1, [FromQuery] int sizePerPage = 10, [FromQuery] int? roleId = null)
        {
            var (statusCode, content) = await _userServiceClient.GetAllUsersAsync(page, sizePerPage, roleId);
            return CreateResponse(statusCode, content);
        }

        [HttpGet("users/{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var (statusCode, content) = await _userServiceClient.GetUserByIdAsync(id);
            return CreateResponse(statusCode, content);
        }

        [HttpPost("users/login")]
        public async Task<IActionResult> Login([FromBody] object loginRequestDto)
        {
            var (statusCode, content) = await _userServiceClient.LoginAsync(loginRequestDto);
            return CreateResponse(statusCode, content);
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] object userCreateDto)
        {
            var (statusCode, content) = await _userServiceClient.CreateUserAsync(userCreateDto);
            return CreateResponse(statusCode, content);
        }

        [HttpPut("users")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] object userUpdateDto)
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var (statusCode, content) = await _userServiceClient.UpdateUserAsync(userUpdateDto, authorizationHeader);
            return CreateResponse(statusCode, content);
        }

        [HttpDelete("users")]
        [Authorize]
        public async Task<IActionResult> DeleteUser()
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var (statusCode, content) = await _userServiceClient.DeleteUserAsync(authorizationHeader);
            return CreateResponse(statusCode, content);
        }

        [HttpPost("users/logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var authorizationHeader = Request.Headers["Authorization"].ToString();
            var (statusCode, content) = await _userServiceClient.LogoutUserAsync(authorizationHeader);
            return CreateResponse(statusCode, content);
        }

        private IActionResult CreateResponse(HttpStatusCode statusCode, string content)
        {
            return statusCode switch
            {
                HttpStatusCode.OK => Ok(content),
                HttpStatusCode.Created => Created("", content),
                HttpStatusCode.NoContent => NoContent(),
                HttpStatusCode.Unauthorized => Unauthorized(content),
                HttpStatusCode.Forbidden => Forbid(content),
                HttpStatusCode.NotFound => NotFound(content),
                _ => StatusCode((int)statusCode, content)
            };
        }
    }
}
