using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserService.Api.Constants;
using UserService.Application.DTOs;
using UserService.Application.DTOs.Requests;
using UserService.Application.DTOs.Responses;
using UserService.Application.Exceptions;
using UserService.Application.Services;
namespace UserService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserAppService _userAppService;
        private readonly IJwtService _jwtService;
        public UsersController(IUserAppService userAppService, IJwtService jwtService)
        {
            _userAppService = userAppService;
            _jwtService = jwtService;
        }
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _userAppService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int sizePerPage = 10, [FromQuery] int? roleId = null)
        {
            try
            {
                var paginatedResponse = await _userAppService.GetAllUsersAsync(page, sizePerPage, roleId);
                return Ok(new
                {
                    code = ApiResponse.Success,
                    message = ApiResponse.Messages[ApiResponse.Success],
                    data = paginatedResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    code = ApiResponse.InternalServerError,
                    message = ApiResponse.Messages[ApiResponse.InternalServerError]
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var isValidUser = await _userAppService.AuthenticateUserAsync(request.Email, request.Password);
                if (!isValidUser.IsAuthenticated)
                {
                    return Unauthorized(new
                    {
                        code = ApiResponse.AuthenticationFailed,
                        message = "Invalid email or password."
                    });
                }

                return Ok(new
                {
                    code = ApiResponse.Success,
                    message = "Login successful.",
                    data = new LoginResponseDto
                    {
                        Token = isValidUser.Token,
                        Expiration = isValidUser.Expiration
                    }
                });
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, new
                {
                    code = ApiResponse.InternalServerError,
                    message = "An error occurred while processing your request."
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserRequestDto userDto)
        {
            try
            {
                var createdUser = await _userAppService.CreateUserAsync(userDto);
                return CreatedAtAction(nameof(Get), new { id = createdUser.Id }, new
                {
                    code = ApiResponse.Success,
                    message = ApiResponse.Messages[ApiResponse.Success],
                    data = createdUser
                });
            }
            catch (GlobalException ex)
            {
                return BadRequest(new
                {
                    code = ApiResponse.ValidationError,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    code = ApiResponse.InternalServerError,
                    message = ApiResponse.Messages[ApiResponse.InternalServerError]
                });
            }
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UserRequestDto userDto)
        {
            try
            {
                var userId = _jwtService.GetUserIdFromToken(User);
                var updatedUser = await _userAppService.UpdateUserAsync(userId, userDto);

                if (updatedUser == null)
                    return NotFound();

                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    code = ApiResponse.InternalServerError,
                    message = ex.Message
                });
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Delete()
        {
            try
            {
                var userId = _jwtService.GetUserIdFromToken(User);
                var success = await _userAppService.DeleteUserAsync(userId);

                if (!success)
                    return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    code = ApiResponse.InternalServerError,
                    message = ex.Message
                });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var token = _jwtService.ExtractToken(Request.Headers["Authorization"]);
                await _userAppService.LogoutAsync(token);

                return Ok(new
                {
                    code = ApiResponse.Success,
                    message = "Logout successful."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    code = ApiResponse.ValidationError,
                    message = ex.Message
                });
            }
        }
    }
}
