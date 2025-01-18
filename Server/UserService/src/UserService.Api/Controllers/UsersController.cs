    using Microsoft.AspNetCore.Mvc;
using UserService.Api.Constants;
using UserService.Application.DTOs;
using UserService.Application.DTOs.Requests;
using UserService.Application.Exceptions;
using UserService.Application.Services;
namespace UserService.Api.Controllers
{
        [ApiController]
        [Route("api/[controller]")]
        public class UsersController : ControllerBase
        {
            private readonly IUserAppService _userAppService;

            public UsersController(IUserAppService userAppService)
            {
                _userAppService = userAppService;
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

            [HttpPut("{id:int}")]
            public async Task<IActionResult> Update(int id, [FromBody] UserRequestDto userDto)
            {
                var updatedUser = await _userAppService.UpdateUserAsync(id, userDto);
                if (updatedUser == null)
                    return NotFound();
                return Ok(updatedUser);
            }

            [HttpDelete("{id:int}")]
            public async Task<IActionResult> Delete(int id)
            {
                var success = await _userAppService.DeleteUserAsync(id);
                if (!success)
                    return NotFound();
                return NoContent();
            }
        }
    }
